using UnityEngine;

namespace _Core._Global.UISystem
{
    [CreateAssetMenu(fileName = "UIWindow", menuName = "GAME/UI/UIWindow")]
    public class UIWindowDefinitionConfig : ScriptableObject
    {
        public WindowType windowType;
        public UIWindow prefab;
        public WindowLayer defaultLayer = WindowLayer.Normal;
        public bool isDraggable = false;
        public bool showOnInit = false;
    }
}