using System.Collections.Generic;
using UnityEngine;

namespace _Core._Global.UISystem
{
    [CreateAssetMenu(fileName = "UIList", menuName = "GAME/UI/UIList")]
    public class UIList : ScriptableObject
    {
        [SerializeField] private List<UIWindowDefinitionConfig> _list = new List<UIWindowDefinitionConfig>();
        public IReadOnlyList<UIWindowDefinitionConfig> List => _list;
        public UIWindowDefinitionConfig GetWindow(WindowType type) => _list.Find(w => w.windowType == type);
    }
}