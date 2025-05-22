using UnityEngine;

public enum WidgetType
{
    Dynamic,
    Static
}

public abstract class Widget : MonoBehaviour
{
    [field: SerializeField] public WidgetType type { get; private set; } = WidgetType.Dynamic;

    public Transform target;
    public Vector3 offset;

    protected bool isInitialized = false;

    public virtual void Initialize(Transform target, Vector3 offset)
    {
        this.target = target;
        this.offset = offset;
        isInitialized = true;
        OnStartWidget();
    }

    public virtual void OnStartWidget() {}

    public abstract void OnUpdateWidget();
}