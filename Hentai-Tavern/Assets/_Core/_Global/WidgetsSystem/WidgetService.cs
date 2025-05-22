using _Core._Global.Services;
using _Core._Global.UISystem;
using _Core._Global.WidgetsSystem;
using Cysharp.Threading.Tasks;
using UnityEngine;

[DependsOn(typeof(IUIService))]
public class WidgetService : AService, IWidgetService
{
    private IUIService _uiService;
    private WidgetWindow _widgetWindow;

    public override async UniTask OnStart()
    {
        await UniTask.Yield();

        _uiService = GService.GetService<IUIService>();
        if (_uiService == null)
        {
            Debug.LogError("WidgetService: UIService not found!");
            return;
        }

        if (_uiService is UIService realUIService)
        {
            var baseWin = realUIService.GetWindow(WindowType.Widget);
            _widgetWindow = baseWin as WidgetWindow;
        }

        if (_widgetWindow == null)
        {
            Debug.LogError("WidgetService: WidgetWindow not found! Check your UIList config.");
        }
    }

    public Widget CreateWidget(Widget prefab, Transform target, Vector3 offset)
    {
        if (prefab == null)
        {
            Debug.LogWarning("WidgetService: prefab == null");
            return null;
        }
        if (_widgetWindow == null)
        {
            Debug.LogWarning("WidgetService: WidgetWindow is not initialized.");
            return null;
        }

        if (!_uiService.IsWindowOpen(WindowType.Widget))
        {
            _uiService.ShowWindow(WindowType.Widget);
        }

        var w = GameObject.Instantiate(prefab);
        w.Initialize(target, offset);

        _widgetWindow.AddWidget(w);
        return w;
    }

    public void RemoveWidget(Widget w)
    {
        if (w == null || _widgetWindow == null) return;
        _widgetWindow.RemoveWidget(w);
    }
}
