using System.Collections.Generic;
using _Core._Global.Categories.Filtering;
using UnityEngine;

namespace _Core._Global.Categories
{
    [CreateAssetMenu(fileName = "CategoryList", menuName = "GAME/Categories/CategoryList")]
    public class CategoryList : ScriptableObject
    {
        [Header("Root nodes of category tree")] [SerializeField]
        private List<CategoryNode> rootNodes = new List<CategoryNode>();
        public IReadOnlyList<CategoryNode> RootNodes => rootNodes;
    }
}