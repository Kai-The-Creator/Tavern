using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace _Core._Global.UISystem
{
    public enum WindowType
    {
        None,
        MainMenu,
        Inventory,
        Settings,
        ModalBlocker,
        Dialogue,
        Widget,
        Shop,
        Initializer,
        Collection,
        Equipment,
        WeaponInventory,
        InventoryTooltip
    }

    public enum WindowLayer
    {
        Background,
        Normal,
        Popup,
        Blocker,
        Modal
    }

    public class UIWindow : MonoBehaviour
    {
        public WindowType WindowType;
        public WindowLayer defaultWindowLayer = WindowLayer.Normal;

        [SerializeField] private Canvas _canvas;
        [SerializeField] private CanvasGroup _canvasGroup;

        private bool _isOpen;
        public bool IsOpen => _isOpen;

        public WindowLayer CurrentLayer { get; private set; }
        public int CurrentSortingOrder { get; private set; }

        protected virtual void Awake()
        {
            if (!_canvas)      _canvas = GetComponent<Canvas>();
            if (!_canvasGroup) _canvasGroup = GetComponent<CanvasGroup>();

            if (!_canvas)      _canvas = gameObject.AddComponent<Canvas>();
            if (!_canvasGroup) _canvasGroup = gameObject.AddComponent<CanvasGroup>();

            gameObject.SetActive(false);
        }

        public virtual void Initialize(bool draggable, object userData, Camera mainCamera)
        {
            _canvas.renderMode = RenderMode.ScreenSpaceCamera;
            _canvas.worldCamera = mainCamera;
            _canvas.overrideSorting = true;

            if (draggable)
            {
                var drag = GetComponent<DraggableUI>();
                if (!drag) drag = gameObject.AddComponent<DraggableUI>();
            }
        }

        public virtual async UniTask Show(WindowLayer layer)
        {
            if (_isOpen) return;
            _isOpen = true;
            CurrentLayer = layer;

            gameObject.SetActive(true);

            _canvasGroup.alpha = 0f;
            _canvasGroup.interactable = false;
            await DOTween.To(() => _canvasGroup.alpha, x => _canvasGroup.alpha = x, 1f, 0.2f)
                .SetEase(Ease.OutQuad)
                .AsyncWaitForCompletion();

            _canvasGroup.interactable = true;
        }

        public virtual async UniTask Close()
        {
            if (!_isOpen) return;
            _isOpen = false;

            _canvasGroup.interactable = false;
            await DOTween.To(() => _canvasGroup.alpha, x => _canvasGroup.alpha = x, 0f, 0.2f)
                .SetEase(Ease.InQuad)
                .AsyncWaitForCompletion();

            gameObject.SetActive(false);
        }

        public void SetCustomSortingOrder(int order)
        {
            CurrentSortingOrder = order;
            _canvas.sortingOrder = order;
        }

        public void SetOpenState(bool state)
        {
            _isOpen = state;
            gameObject.SetActive(state);
            _canvasGroup.alpha = state ? 1 : 0;
            _canvasGroup.interactable = state;
        }
    }
}