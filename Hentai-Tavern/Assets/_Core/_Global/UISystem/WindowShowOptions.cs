namespace _Core._Global.UISystem
{
    [System.Serializable]
    public struct WindowShowOptions
    {
        public WindowLayer Layer;
        public bool IsDraggable;
        public bool IsResizable;
        public object UserData;

        public WindowShowOptions(WindowLayer layer = WindowLayer.Normal, bool isDraggable = false, bool isResizable = false, object userData = null)
        {
            Layer = layer;
            IsDraggable = isDraggable;
            IsResizable = isResizable;
            UserData = userData;
        }
    }
}