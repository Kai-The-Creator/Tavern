using System;
using System.Collections.Generic;
using System.Linq;

namespace _Core._Global.Categories.Filtering
{
    public class CategoryFilter<T> : IFilter<T>
    {
        public Func<T, IEnumerable<CategoryDefinition>> CategorySelector { get; }
        public HashSet<CategoryNode> SelectedNodes { get; } = new HashSet<CategoryNode>();

        public CategoryFilter(Func<T, IEnumerable<CategoryDefinition>> selector)
        {
            CategorySelector = selector;
        }

        public bool Match(T item)
        {
            if (SelectedNodes.Count == 0)
                return true; // ничего не выбрано — пропускаем всё

            var itemCats = CategorySelector(item) ?? Enumerable.Empty<CategoryDefinition>();
            var defs = SelectedNodes
                .SelectMany(Traverse)
                .Select(n => n.Definition)
                .ToHashSet();

            return itemCats.Any(defs.Contains);
        }

        private IEnumerable<CategoryNode> Traverse(CategoryNode node)
        {
            yield return node;
            foreach (var child in node.Children)
            foreach (var nested in Traverse(child))
                yield return nested;
        }
    }
}