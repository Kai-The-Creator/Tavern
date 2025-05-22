using System.Collections.Generic;
using _Core._Global.Categories.Filtering;
using UnityEngine;
using UnityEngine.UI;

namespace _Core._Global.UI.Inventory
{
    [RequireComponent(typeof(Toggle))]
    public sealed class InventoryTab : MonoBehaviour
    {
        [Tooltip("Какие категории показывать.")]
        [SerializeField] private List<CategoryDefinition> _categories = new();
        [Tooltip("Какой sub-view включать.")]
        [SerializeField] private ViewKind _viewType = ViewKind.Stack;

        public IReadOnlyList<CategoryDefinition> Categories => _categories;
        public ViewKind                         ViewType   => _viewType;
        public Toggle                           Toggle     { get; private set; }

        private void Awake() => Toggle = GetComponent<Toggle>();

        public enum ViewKind { Stack, Tier, Unlockable }
    }
}