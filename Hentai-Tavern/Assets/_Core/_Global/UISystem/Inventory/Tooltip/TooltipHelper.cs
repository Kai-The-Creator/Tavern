using _Core._Global.Services;
using _Core._Global.UI;
using _Core._Global.UI.Tooltips;
using _Core._Global.UISystem;
using UnityEngine;

public static class TooltipHelper
{
    private static IUIService UI => GService.GetService<IUIService>();

    public static void Show(TooltipData data, RectTransform anchor)
    {
        var win = (TooltipWindow)UI.GetWindow(WindowType.InventoryTooltip);
        win.Feed(data, anchor);                       // передали данные
        UI.ShowWindow(WindowType.InventoryTooltip);
    }

    public static void Hide()
    {
        if (UI.IsWindowOpen(WindowType.InventoryTooltip))
            UI.CloseWindow(WindowType.InventoryTooltip);
    }
}