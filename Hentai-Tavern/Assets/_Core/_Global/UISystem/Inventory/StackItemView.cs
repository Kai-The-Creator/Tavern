using _Core._Global.ItemSystem;
using _Core._Global.Services;
using _Core._Global.UISystem;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace _Core._Global.UI.Inventory
{
    [RequireComponent(typeof(CanvasGroup))]
    public sealed class StackItemView : MonoBehaviour,
        IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler, IPointerExitHandler
    {
        [SerializeField] private Image           _icon;
        [SerializeField] private TextMeshProUGUI _qty;

        private CanvasGroup      _cg;
        private RectTransform    _rt;
        private Canvas           _rootCanvas;
        private Vector2          _startLocalPos;
        private StackCellView    _parentCell;

        public ItemState State { get; private set; }

        void Awake()
        {
            _cg         = GetComponent<CanvasGroup>();
            _rt         = (RectTransform)transform;
            _rootCanvas = GetComponentInParent<Canvas>();
        }

        #region Bind / Unbind --------------------------------------------------
        public void Bind(ItemState st, StackCellView cell)
        {
            State           = st;
            _parentCell     = cell;

            _icon.sprite    = st.Config.Icon;
            _qty.text       = st.Quantity.ToString();

            cell.Attach(this);                // parent + stretch anchors
            _rt.anchoredPosition = Vector2.zero;
        }
        #endregion

        #region Drag & Drop ----------------------------------------------------
        public void OnBeginDrag(PointerEventData ev)
        {
            _cg.blocksRaycasts = false;       // пропускаем клик сквозь айтем
            _startLocalPos     = _rt.anchoredPosition;

            // кидаем в DragLayer, чтобы быть поверх
            var dragLayer = _parentCell.GetComponentInParent<InventoryWindow>().DragLayer;
            _rt.SetParent(dragLayer, true);   // world-pos
        }

        public void OnDrag(PointerEventData ev)
        {
            _rt.anchoredPosition += ev.delta / _rootCanvas.scaleFactor;
        }

        public void OnEndDrag(PointerEventData ev)
        {
            _cg.blocksRaycasts = true;

            // Куда уронили?
            var hitObj    = ev.pointerCurrentRaycast.gameObject;
            var targetCell = hitObj ? hitObj.GetComponentInParent<StackCellView>() : null;

            if (targetCell == null)
            {   // вернуться на место
                ReturnToParent();
                return;
            }

            // ------ swap или move ------
            if (!targetCell.HasItem)
            {   // просто перенос
                _parentCell.Detach();
                Bind(State, targetCell);
            }
            else if (targetCell != _parentCell)
            {   // SWAP
                SwapItems(_parentCell, targetCell);
            }
            else
            {   // отпустили в ту же ячейку
                ReturnToParent();
            }
        }

        private void ReturnToParent()
        {
            Bind(State, _parentCell);         // re-attach + zero pos
        }

        // статический обмен двух StackItemView
        private static void SwapItems(StackCellView a, StackCellView b)
        {
            var itemA = a.Item;
            var itemB = b.Item;

            a.Detach();
            b.Detach();

            if (itemA) itemA.Bind(itemA.State, b);
            if (itemB) itemB.Bind(itemB.State, a);
        }
        #endregion

        public void OnPointerClick(PointerEventData eventData)
        {
            var data = TooltipBuilder.FromStack(State);
            TooltipHelper.Show(data, (RectTransform)transform);
        }
        public void OnPointerExit(PointerEventData _)
        {
            // var ui = GService.GetService<IUIService>();
            // if (ui.IsWindowOpen(WindowType.InventoryTooltip))
            // {
            //     var tip = (TooltipWindow)ui.GetWindow(WindowType.InventoryTooltip);
            //     if (!tip.MouseOver)                           // курсор НЕ на тултипе
            //         TooltipHelper.Hide();
            // }
        }
    }
}
