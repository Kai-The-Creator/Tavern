using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenLayerComponent : MonoBehaviour
{
    public LayerScreen layer;
    public bool OnStart;

    private void Awake()
    {
        ScreenLayersManager.instance.AddToList(this);
        
        ActionOnAwake();

        if(!OnStart)
            Deactivate();
    }

    private void OnDestroy()
    {
        ScreenLayersManager.instance.RemoveFromList(this);
    }

    public virtual void ActionOnAwake()
    {

    }

    public virtual void Activate()
    {
        gameObject.SetActive(true);
    }

    public virtual void Deactivate()
    {
        gameObject.SetActive(false);
    }
}
