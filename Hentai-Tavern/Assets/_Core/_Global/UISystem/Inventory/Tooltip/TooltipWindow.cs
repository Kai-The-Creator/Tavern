// Assets/_Core/_Global/UI/Tooltips/TooltipWindow.cs

using _Core._Global.Services;
using Cysharp.Threading.Tasks;
using _Core._Global.UI.Tooltips;
using _Core._Global.UISystem;
using UnityEngine;
using UnityEngine.UI;

namespace _Core._Global.UI
{
    /// <summary>
    /// Окно-обёртка, которое выбирает нужный дочерний тултип-вью
    /// (Stack или Tier) и ставит его рядом с выбранной ячейкой.
    /// Вся визуализация находится внутри конкретного View-префаба.
    /// </summary>
    public sealed class TooltipWindow : UIWindow
    {
        [Header("Canvas / Root")] 
        [SerializeField] private RectTransform _canvasRt; // Rect канваса (this)

        [SerializeField] private GameObject _contentRoot; // корневой GO панели

        [Header("Subtype Views")] [SerializeField]
        private StackTooltipView _stackView; // наследует BaseTooltipView

        [SerializeField] private TierTooltipView _tierView;

        [Header("Behaviour")] [SerializeField] private Vector2 _offset = new(24, 24);

        [SerializeField] private Button _closeBtn;


        protected override void Awake()
        {
            _closeBtn.onClick.AddListener(() =>
                GService.GetService<IUIService>().CloseWindow(WindowType.InventoryTooltip));
            base.Awake();
        }

        /*──────────────────────────── UIWindow */

        public override async UniTask Show(WindowLayer layer)
        {
            _contentRoot.SetActive(true);
            await base.Show(layer); // fade-in из базового UIWindow
        }

        public override async UniTask Close()
        {
            await base.Close(); // fade-out
            _contentRoot.SetActive(false);
        }

        /*──────────────────────────── External API */

        public void Feed(in TooltipData data, RectTransform anchor)
        {
            // активируем нужный вью
            _stackView.gameObject.SetActive(data.Kind == TooltipKind.Stack);
            _tierView.gameObject.SetActive(data.Kind == TooltipKind.Tier);

            if (data.Kind == TooltipKind.Stack) _stackView.Bind(data);
            else _tierView.Bind(data);

            PlaceNearFixed(anchor);
        }

        /*──────────────────────────── Позиционирование */

        private void PlaceNearFixed(RectTransform anchor)
        {
            if (!anchor) return;

            // 1. Пересчитываем layout активного контента, чтобы узнать реальный размер
            var rtTip = (RectTransform)_contentRoot.transform;
            LayoutRebuilder.ForceRebuildLayoutImmediate(rtTip);

            // 2. Центр ячейки → локальные координаты Canvas
            Vector2 localAnchor;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _canvasRt,
                RectTransformUtility.WorldToScreenPoint(null, anchor.position),
                null,
                out localAnchor);

            Vector2 tipSize = rtTip.rect.size;
            Vector2 halfCv = _canvasRt.rect.size * 0.5f;

            // 3. Пивоты-кандидаты
            (Vector2 pivot, Vector2 shift)[] pivots =
            {
                (new Vector2(0, 1), new Vector2(+_offset.x, -_offset.y)), // л-в
                (new Vector2(1, 1), new Vector2(-_offset.x, -_offset.y)), // п-в
                (new Vector2(0, 0), new Vector2(+_offset.x, +_offset.y)), // л-н
                (new Vector2(1, 0), new Vector2(-_offset.x, +_offset.y)) // п-н
            };

            foreach (var (pv, sh) in pivots)
            {
                Vector2 pos = localAnchor + sh;
                float minX = pos.x - pv.x * tipSize.x;
                float maxX = pos.x + (1 - pv.x) * tipSize.x;
                float minY = pos.y - pv.y * tipSize.y;
                float maxY = pos.y + (1 - pv.y) * tipSize.y;

                if (minX >= -halfCv.x && maxX <= halfCv.x &&
                    minY >= -halfCv.y && maxY <= halfCv.y)
                {
                    rtTip.pivot = pv;
                    rtTip.anchoredPosition = pos;
                    return;
                }
            }

            // 4. Fallback: притягиваем внутрь Canvas-а
            rtTip.pivot = new Vector2(.5f, .5f);
            rtTip.anchoredPosition = Vector2.zero;
        }

        private void PlaceNear(RectTransform anchor)
        {
            if (!anchor) return;

            var rtTip = (RectTransform)_contentRoot.transform;
            var canvas = _canvasRt;

            // 1. пересчитываем Layout, чтобы rt.rect.size было актуально
            LayoutRebuilder.ForceRebuildLayoutImmediate(rtTip);

            // 2. позиция точки “центра ячейки” в локальных координатах Canvas
            Vector2 localAnchor;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas, RectTransformUtility.WorldToScreenPoint(null, anchor.position),
                null, out localAnchor);

            // 3. размеры
            Vector2 tipSize = rtTip.rect.size; // уже учтён padding + layout
            Vector2 halfCv = canvas.rect.size * 0.5f; // половина канваса
            const float OFFSET = 24f;

            // 4. кандидаты пивотов с отступом
            (Vector2 pivot, Vector2 shift)[] candidates =
            {
                (new Vector2(0, 1), new Vector2(+OFFSET, -OFFSET)), // лев-верх
                (new Vector2(1, 1), new Vector2(-OFFSET, -OFFSET)), // прав-верх
                (new Vector2(0, 0), new Vector2(+OFFSET, +OFFSET)), // лев-низ
                (new Vector2(1, 0), new Vector2(-OFFSET, +OFFSET)) // прав-низ
            };

            foreach (var c in candidates)
            {
                // позиция того угла, который совпадает с центром ячейки + отступ
                Vector2 pos = localAnchor + c.shift;

                // мин/макс для тултипа в локальных коорд-тах
                float minX = pos.x - c.pivot.x * tipSize.x;
                float maxX = pos.x + (1 - c.pivot.x) * tipSize.x;
                float minY = pos.y - c.pivot.y * tipSize.y;
                float maxY = pos.y + (1 - c.pivot.y) * tipSize.y;

                // проверяем, помещается ли рамка
                if (minX >= -halfCv.x && maxX <= halfCv.x &&
                    minY >= -halfCv.y && maxY <= halfCv.y)
                {
                    rtTip.pivot = c.pivot;
                    rtTip.anchoredPosition = pos;
                    return;
                }
            }

            // 5. Если сюда попали – ни один угол не влез. Берём последний кандидат и притягиваем внутрь.
            var last = candidates[^1]; // правый-низ
            Vector2 finalPos = localAnchor + last.shift;
            rtTip.pivot = last.pivot;

            // clamp
            float halfW = tipSize.x * last.pivot.x;
            float halfH = tipSize.y * last.pivot.y;
            finalPos.x = Mathf.Clamp(finalPos.x,
                -halfCv.x + halfW,
                halfCv.x - (tipSize.x - halfW));
            finalPos.y = Mathf.Clamp(finalPos.y,
                -halfCv.y + halfH,
                halfCv.y - (tipSize.y - halfH));

            rtTip.anchoredPosition = finalPos;
        }
    }
}