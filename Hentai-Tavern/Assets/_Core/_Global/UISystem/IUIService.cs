using System;
using _Core._Global.Services;

namespace _Core._Global.UISystem
{
    public interface IUIService : IService
    {
        event Action<WindowType> OnWindowOpened;
        event Action<WindowType> OnWindowClosed;

        void ShowWindow(WindowType windowType, WindowLayer? layerOverride = null);
        void CloseWindow(WindowType windowType);
        void CloseAllWindows();
        bool IsWindowOpen(WindowType windowType);
        UIWindow GetWindow(WindowType windowType);

        void FocusWindow(WindowType windowType);
        void MinimizeWindow(WindowType windowType);
        void MaximizeWindow(WindowType windowType);
    }
}