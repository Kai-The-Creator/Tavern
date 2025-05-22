using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace _Core._Global.UISystem
{
    public class WidgetWindow : UIWindow
    {
        [Header("Container для виджетов")]
        [SerializeField] private RectTransform widgetContainer;

        private List<Widget> _widgets = new List<Widget>();

        private List<Widget> _widgetsToAdd = new List<Widget>();
        private List<Widget> _widgetsToRemove = new List<Widget>();

        private Camera mainCamera;
        private Camera uiCamera;
        private RectTransform rootRect;

        public void Init(Camera mainCam, Camera uiCam)
        {
            this.mainCamera = mainCam;
            this.uiCamera = uiCam;
            rootRect = widgetContainer ? widgetContainer : (RectTransform)transform;
        }

        public void AddWidget(Widget w)
        {
            if (!w) return;
            if (!_widgetsToAdd.Contains(w) && !_widgets.Contains(w))
            {
                _widgetsToAdd.Add(w);
            }

            w.transform.SetParent(widgetContainer ? widgetContainer : transform, false);
        }

        public void RemoveWidget(Widget w)
        {
            if (!w) return;
            if (!_widgetsToRemove.Contains(w))
            {
                _widgetsToRemove.Add(w);
            }
        }

        public override async UniTask Show(WindowLayer layer)
        {
            await base.Show(layer);
        }

        public override async UniTask Close()
        {
            await base.Close();
        }

        private void LateUpdate()
        {
            if (!gameObject.activeInHierarchy) 
                return;

            if (_widgetsToAdd.Count > 0)
            {
                foreach (var w in _widgetsToAdd)
                {
                    if (w && !_widgets.Contains(w))
                    {
                        _widgets.Add(w);
                    }
                }
                _widgetsToAdd.Clear();
            }

            for (int i = 0; i < _widgets.Count; i++)
            {
                var widget = _widgets[i];
                if (!widget) 
                    continue;

                if (!widget.gameObject.activeInHierarchy && widget.type == WidgetType.Dynamic)
                {
                    UpdateWidgetPosition(widget);
                    continue;
                }

                if (widget.type == WidgetType.Dynamic)
                {
                    UpdateWidgetPosition(widget);
                }

                widget.OnUpdateWidget();
            }

            if (_widgetsToRemove.Count > 0)
            {
                foreach (var w in _widgetsToRemove)
                {
                    if (w)
                    {
                        _widgets.Remove(w);
                        Destroy(w.gameObject);
                    }
                }
                _widgetsToRemove.Clear();
            }
        }

        private void UpdateWidgetPosition(Widget widget)
        {
            if (mainCamera == null)
            {
                Debug.LogWarning($"WidgetWindow: mainCamera == null, cannot update widget {widget.name} position.");
                return;
            }
            if (uiCamera == null)
            {
                Debug.LogWarning($"WidgetWindow: uiCamera == null, cannot update widget {widget.name} position.");
                return;
            }
            if (!widget.target)
            {
                widget.gameObject.SetActive(false);
                return;
            }

            Vector3 worldPos = widget.target.position + widget.offset;

            Vector3 screenPos = mainCamera.WorldToScreenPoint(worldPos);

            if (screenPos.z < 0)
            {
                if (widget.gameObject.activeSelf)
                    widget.gameObject.SetActive(false);
                return;
            }

            bool isOutsideScreen = 
                (screenPos.x < 0 || screenPos.x > Screen.width ||
                 screenPos.y < 0 || screenPos.y > Screen.height);

            if (isOutsideScreen)
            {
                if (widget.gameObject.activeSelf)
                    widget.gameObject.SetActive(false);

                return;
            }
            else
            {
                if (!widget.gameObject.activeSelf)
                    widget.gameObject.SetActive(true);
            }

            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    rootRect,
                    screenPos,
                    uiCamera,
                    out Vector2 localPoint))
            {
                var rect = widget.GetComponent<RectTransform>();
                if (rect)
                {
                    rect.anchoredPosition = localPoint;
                }
            }
        }
    }
}
