using UnityEngine;

namespace _Core._Global.UI.Inventory
{
    /// <summary>Пустая ячейка-контейнер для Stack-предмета.</summary>
    public sealed class StackCellView : MonoBehaviour
    {
        [Tooltip("Куда вписывается StackItemView. Если null, берётся первый ребёнок.")]
        [SerializeField] private RectTransform _contentRoot;

        public RectTransform ContentRoot
        {
            get
            {
                if (_contentRoot == null)
                    _contentRoot = transform.GetChild(0) as RectTransform;
                return _contentRoot;
            }
        }

        public StackItemView Item { get; private set; }
        public bool HasItem => Item != null;

        public void Attach(StackItemView item)
        {
            Item = item;
            if (!item) return;

            var rt = (RectTransform)item.transform;
            rt.SetParent(ContentRoot, false);

            // растянуть на всю ячейку
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = rt.offsetMax = Vector2.zero;
        }

        public void Detach() => Item = null;
    }
}