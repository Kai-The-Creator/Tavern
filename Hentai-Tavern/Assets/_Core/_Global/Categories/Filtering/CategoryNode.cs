using System.Collections.Generic;
using UnityEngine;

namespace _Core._Global.Categories.Filtering
{
    [CreateAssetMenu(fileName = "NewCategoryTree", menuName = "GAME/Categories/Filtering/CategoryTree")]
    public class CategoryNode : ScriptableObject
    {
        public string DisplayName;
        public CategoryDefinition Definition; // ваш SO‑тип категории
        public List<CategoryNode> Children = new List<CategoryNode>();

        [HideInInspector] public bool IsSelected = false;
    }
}