using System;
using System.Collections.Generic;
using _Core._Global.CameraService;
using _Core._Global.Equip;
using _Core._Global.GConfig;
using _Core._Global.Services;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Core._Global.UISystem
{
    [DependsOn(typeof(ICameraService), typeof(IGlobalConfigService), typeof(IEquipService))]
    public class UIService : AService, IUIService
    {
        private readonly Dictionary<WindowLayer, int> _baseOrder = new()
        {
            { WindowLayer.Background, 0 },
            { WindowLayer.Normal,     100 },
            { WindowLayer.Popup,      200 },
            { WindowLayer.Blocker,    998 },
            { WindowLayer.Modal,      999 },
        };

        private readonly Dictionary<WindowLayer, List<UIWindow>> _windowsByLayer = new();
        private Dictionary<WindowType, UIWindow> _windows = new();
        private UIWindow _blockerWindow;

        public event Action<WindowType> OnWindowOpened;
        public event Action<WindowType> OnWindowClosed;


        public WindowType FromInspectorWindow;
        
        [ContextMenu("OpenWindow")]
        private void OpenWindow()
        {
            ShowWindow(FromInspectorWindow);
        }
        
        [ContextMenu("Close Window")]
        private void HideWindow()
        {
            CloseWindow(FromInspectorWindow);
        }

        public override async UniTask OnStart()
        {
            await UniTask.Yield();

            var cameraService = GService.GetService<ICameraService>();
            if (cameraService == null)
            {
                Debug.LogError("UIService: CameraService not found!");
                return;
            }

            foreach (WindowLayer layer in Enum.GetValues(typeof(WindowLayer)))
            {
                if (!_windowsByLayer.ContainsKey(layer))
                {
                    _windowsByLayer[layer] = new List<UIWindow>();
                }
            }

            var gConfig = GService.GetService<IGlobalConfigService>();

            var uiList = gConfig.Windows;
            if (uiList == null || uiList.List == null || uiList.List.Count == 0)
            {
                Debug.LogWarning("UIService: No UI windows in GConfig.Windows.");
            }
            else
            {
                foreach (var def in uiList.List)
                {
                    if (!def) continue;
                    if (_windows.ContainsKey(def.windowType))
                    {
                        Debug.LogWarning($"UIService: Duplicate window type {def.windowType}");
                        continue;
                    }

                    var existing = FindWindowInRoot(def.windowType);
                    UIWindow finalWin = null;

                    if (existing)
                    {
                        finalWin = existing;
                    }
                    else
                    {
                        if (!def.prefab)
                        {
                            Debug.LogError($"UIService: No prefab for {def.windowType}");
                            continue;
                        }

                        finalWin = Instantiate(def.prefab, transform);
                        finalWin.WindowType = def.windowType;
                        finalWin.defaultWindowLayer = def.defaultLayer;
                    }
                    
                    Camera mainCam = cameraService.GetMainCamera();
                    Camera uiCam   = cameraService.GetUICamera();

                    finalWin.gameObject.SetActive(false);

                    finalWin.Initialize(def.isDraggable, null, uiCam);

                    if (def.windowType == WindowType.Widget)
                    {
                        if (finalWin is WidgetWindow widgetWin)
                        {
                            widgetWin.Init(mainCam, uiCam);
                        }
                    }

                    _windows.Add(def.windowType, finalWin);

                    if (def.windowType == WindowType.ModalBlocker)
                    {
                        _blockerWindow = finalWin;
                    }
                }

                foreach (var def in uiList.List)
                {
                    if (def && def.showOnInit)
                    {
                        ShowWindow(def.windowType, def.defaultLayer);
                    }
                }
            }

            Debug.Log("UIService: InitializeAsync finished!");
        }

        private UIWindow FindWindowInRoot(WindowType t)
        {
            var all = transform.GetComponentsInChildren<UIWindow>(true);
            foreach (var w in all)
            {
                if (w.WindowType == t) 
                    return w;
            }
            return null;
        }

        #region Открытие/закрытие/фокус окон

        public async void ShowWindow(WindowType windowType, WindowLayer? layerOverride = null)
        {
            if (!_windows.TryGetValue(windowType, out var window))
            {
                Debug.LogError($"UIService: windowType {windowType} not found!");
                return;
            }

            if (window.IsOpen)
            {
                FocusWindow(windowType);
                return;
            }

            var layer = layerOverride ?? window.defaultWindowLayer;

            AddWindowToLayerList(window, layer);
            await window.Show(layer);

            if (layer == WindowLayer.Modal)
                UpdateBlockerVisibility();

            OnWindowOpened?.Invoke(windowType);
        }

        public async void CloseWindow(WindowType windowType)
        {
            if (!_windows.TryGetValue(windowType, out var window))
            {
                Debug.LogError($"UIService: WindowType {windowType} not found!");
                return;
            }

            if (!window.IsOpen)
            {
                Debug.Log($"UIService: Window {windowType} is already closed.");
                return;
            }

            var layer = window.CurrentLayer;

            await window.Close();

            RemoveWindowFromLayerList(window, layer);

            if (layer == WindowLayer.Modal)
                UpdateBlockerVisibility();

            OnWindowClosed?.Invoke(windowType);
        }

        public void FocusWindow(WindowType windowType)
        {
            if (!_windows.TryGetValue(windowType, out var window))
            {
                Debug.LogError($"UIService: Window {windowType} not found!");
                return;
            }
            if (!window.IsOpen)
            {
                Debug.Log($"UIService: Window {windowType} is not open.");
                return;
            }

            var layer = window.CurrentLayer;
            MoveWindowToTop(window, layer);

            if (layer == WindowLayer.Modal)
                UpdateBlockerVisibility();
        }

        public void CloseAllWindows()
        {
            foreach (var kvp in _windows)
            {
                var w = kvp.Value;
                if (w.IsOpen)
                    w.SetOpenState(false);
            }

            foreach (var layer in _windowsByLayer.Keys)
            {
                _windowsByLayer[layer].Clear();
            }

            if (_blockerWindow && _blockerWindow.IsOpen)
                _blockerWindow.SetOpenState(false);
        }

        #endregion

        #region Слои и сортировка

        private void AddWindowToLayerList(UIWindow window, WindowLayer layer)
        {
            RemoveWindowFromLayerList(window, layer);
            _windowsByLayer[layer].Add(window);
            ReorderLayer(layer);
        }

        private void RemoveWindowFromLayerList(UIWindow window, WindowLayer layer)
        {
            var list = _windowsByLayer[layer];
            if (list.Contains(window))
            {
                list.Remove(window);
                ReorderLayer(layer);
            }
        }

        private void MoveWindowToTop(UIWindow window, WindowLayer layer)
        {
            var list = _windowsByLayer[layer];
            if (list.Remove(window))
            {
                list.Add(window);
                ReorderLayer(layer);
            }
        }

        private void ReorderLayer(WindowLayer layer)
        {
            if (!_baseOrder.TryGetValue(layer, out int baseOrder))
                baseOrder = 100;

            var list = _windowsByLayer[layer];
            for (int i = 0; i < list.Count; i++)
            {
                int order = baseOrder + i;
                list[i].SetCustomSortingOrder(order);
            }
        }

        private void UpdateBlockerVisibility()
        {
            if (!_blockerWindow) 
                return;

            var modalList = _windowsByLayer[WindowLayer.Modal];
            if (modalList.Count == 0)
            {
                if (_blockerWindow.IsOpen)
                    _blockerWindow.SetOpenState(false);
                return;
            }

            var topModal = modalList[^1];
            int topModalOrder = topModal.CurrentSortingOrder;

            int blockerOrder = Mathf.Max(0, topModalOrder - 1);
            if (_baseOrder.TryGetValue(WindowLayer.Blocker, out int baseBlockerOrder))
            {
                blockerOrder = Mathf.Max(baseBlockerOrder, blockerOrder);
            }

            _blockerWindow.SetCustomSortingOrder(blockerOrder);

            if (!_blockerWindow.IsOpen)
            {
                _blockerWindow.SetOpenState(true);
            }
        }

        #endregion

        #region Публичные методы получения окна

        public UIWindow GetWindow(WindowType windowType)
        {
            _windows.TryGetValue(windowType, out var w);
            return w;
        }

        public bool IsWindowOpen(WindowType windowType)
        {
            return _windows.TryGetValue(windowType, out var w) && w.IsOpen;
        }

        public void MinimizeWindow(WindowType windowType)
        {
        }

        public void MaximizeWindow(WindowType windowType)
        {
        }
        #endregion
    }
}
