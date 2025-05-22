using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    public static T instance;
    public bool isDestroyed;

    private void Awake()
    {
        instance = this as T;
        AwakeAction();
    }

    private void OnDestroy()
    {
        if (!isDestroyed) return;

        instance = null;
    }

    public virtual void AwakeAction()
    {

    }
}
