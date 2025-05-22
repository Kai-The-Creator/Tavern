using System;
using _Core._Global.Services;
using UnityEngine;

public class Improved3DWidget : MonoBehaviour
{
    [Header("Widget Config")]
    [SerializeField] private ImprovedWidgetConfig config; 
    [SerializeField] private Vector3 offset = new Vector3(0, 2f, 0);

    private WidgetService widgetService;
    private Widget currentWidget;
    private bool widgetCreated = false;

    private void Start()
    {
        widgetService = GService.GetService<WidgetService>();
        if (widgetService == null)
        {
            Debug.LogError("Improved3DWidget: WidgetService not found!");
        }
    }

    private void OnEnable()
    {
        if (widgetService == null)
        {
            widgetService = GService.GetService<WidgetService>();
            if (widgetService == null)
            {
                Debug.LogError("Improved3DWidget: WidgetService not found!");
                return;
            }
        }

        if (!config || !config.widgetPrefab)
        {
            Debug.LogWarning("Improved3DWidget: No valid widget config/prefab assigned.");
            return;
        }

        if (!widgetCreated)
        {
            currentWidget = widgetService.CreateWidget(config.widgetPrefab, transform, offset);
            if (!currentWidget)
            {
                Debug.LogError("Improved3DWidget: Failed to create widget!");
                return;
            }
            if (currentWidget is ImprovedWidget iw)
            {
                iw.ApplyConfig(config);
            }
            widgetCreated = true;
        }
        else
        {
            if (currentWidget)
            {
                currentWidget.gameObject.SetActive(true);
            }
        }
    }

    private void OnDisable()
    {
        if (currentWidget)
        {
            currentWidget.gameObject.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        if (currentWidget && widgetService != null)
        {
            widgetService.RemoveWidget(currentWidget);
            currentWidget = null;
            widgetCreated = false;
        }
    }
}
