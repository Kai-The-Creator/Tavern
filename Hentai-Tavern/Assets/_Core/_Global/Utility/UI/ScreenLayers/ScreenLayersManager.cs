using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenLayersManager : Singleton<ScreenLayersManager>
{
    private List<ScreenLayerComponent> layersList = new();

    public void AddToList(ScreenLayerComponent layer) => layersList.Add(layer);
    public void RemoveFromList(ScreenLayerComponent layer) => layersList.Remove(layer);

    public void SetLayer(LayerScreen layer)
    {
        foreach (ScreenLayerComponent layerComponent in layersList)
        {
            layerComponent.Deactivate();
        }

        List<ScreenLayerComponent> list = new();

        foreach (ScreenLayerComponent layerComponent in layersList)
        {
            if (layerComponent == null) continue;

            if (layerComponent.layer == layer)
            {
                list.Add(layerComponent);
            }
        }

        foreach (ScreenLayerComponent layerComponent in list)
        {
            layerComponent.Activate();
        }
    }
}
