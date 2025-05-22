using System;
using System.Collections.Generic;
using _Core._Global.Categories.Filtering;
using UnityEngine;
using UnityEngine.UI;

namespace _Core._Global.Categories
{
    public class CategoryTreeView : MonoBehaviour
    {
        [SerializeField] private Toggle togglePrefab;
        [SerializeField] private Transform container;
        [SerializeField] private ToggleGroup toggleGroup;
        [SerializeField] private float indentPerLevel = 20f;

        public event Action<CategoryNode, bool> OnNodeToggled;
        private readonly Dictionary<CategoryNode, Toggle> _toggles = new();

        public void Build(IReadOnlyList<CategoryNode> rootNodes)
        {
            Clear();
            _toggles.Clear();

            foreach (var node in rootNodes)
                BuildNode(node, 0);
        }

        private void BuildNode(CategoryNode node, int level)
        {
            var toggle = Instantiate(togglePrefab, container);

            toggle.GetComponent<CategoryFilterToggleView>()
                .Init(toggleGroup, node.Definition.DisplayName, node.IsSelected);

            toggle.onValueChanged.AddListener(isOn =>
            {
                node.IsSelected = isOn;
                OnNodeToggled?.Invoke(node, isOn);
            });

            // var rt = toggle.GetComponent<RectTransform>();
            // rt.anchoredPosition += new Vector2(indentPerLevel * level, 0);

            _toggles[node] = toggle;

            if (node.Children != null)
                foreach (var child in node.Children)
                    BuildNode(child, level + 1);
        }

        private void Clear()
        {
            foreach (Transform t in container)
                Destroy(t.gameObject);
        }

        public void SetAvailableCategories(IEnumerable<CategoryDefinition> available)
        {
            var set = new HashSet<CategoryDefinition>(available);

            foreach (var kv in _toggles)
            {
                var node = kv.Key;
                var toggle = kv.Value;

                bool shouldShow = NodeOrDescendantMatches(node, set);

                if (!shouldShow && toggle.isOn)
                {
                    toggle.isOn = false;
                }

                toggle.gameObject.SetActive(shouldShow);
            }
        }

        private bool NodeOrDescendantMatches(CategoryNode node, HashSet<CategoryDefinition> set)
        {
            if (node.Definition != null && set.Contains(node.Definition))
                return true;

            if (node.Children != null)
            {
                foreach (var child in node.Children)
                    if (NodeOrDescendantMatches(child, set))
                        return true;
            }

            return false;
        }
    }
}
