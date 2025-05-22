using UnityEngine;
using UnityEngine.EventSystems;

namespace _Core._Global.UISystem
{
    public class DraggableUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [SerializeField] private RectTransform dragArea; 
        private RectTransform _rootCanvas;
        private Vector2 _offset;

        private void Awake()
        {
            _rootCanvas = GetComponentInParent<Canvas>().GetComponent<RectTransform>();
            if (dragArea == null)
                dragArea = GetComponent<RectTransform>();
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (_rootCanvas == null) return;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                (RectTransform)dragArea.parent,
                eventData.position,
                eventData.pressEventCamera,
                out _offset
            );
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (_rootCanvas == null) return;

            Vector2 localPoint;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    (RectTransform)dragArea.parent,
                    eventData.position,
                    eventData.pressEventCamera,
                    out localPoint))
            {
                Vector2 offsetPos = localPoint - _offset;
                dragArea.localPosition += (Vector3)offsetPos;
                _offset = localPoint;
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
        }
    }
}