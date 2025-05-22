using System;
using System.Collections.Generic;
using System.Linq;

namespace _Core._Global.Categories.Filtering
{

    public class FilterPresenter<T>
    {
        public event Action OnCriteriaChanged;

        private readonly IFilter<T> _filter;
        private readonly ISorter<T> _sorter;
        private readonly CategoryFilter<T> _catFilter;

        public FilterPresenter(FilterGroup<T> group, ISorter<T> sorter)
        {
            _filter = group;
            _sorter = sorter;
            _catFilter = group.Filters.OfType<CategoryFilter<T>>().FirstOrDefault() ??
                         throw new InvalidOperationException("CategoryFilter<T> not found!");
        }

        protected void FireChanged() => OnCriteriaChanged?.Invoke();

        public IEnumerable<T> Apply(IEnumerable<T> source)
        {
            var filtered = source.Where(item => _filter.Match(item));
            return _sorter.Sort(filtered);
        }

        public CategoryFilter<T> AsCategoryFilter() => _filter as CategoryFilter<T>;
        public SearchFilter<T> AsSearchFilter() => _filter as SearchFilter<T>;
        public Sorter<T, object> AsSorter() => _sorter as Sorter<T, object>;

        public void SetSearchTerm(string term)
        {
            if (_filter is SearchFilter<T> sf)
            {
                sf.SearchTerm = term;
                FireChanged();
            }
        }

        public void SetCategoryNode(CategoryNode node, bool isSelected)
        {
            node.IsSelected = isSelected;

            if (isSelected) _catFilter.SelectedNodes.Add(node);
            else _catFilter.SelectedNodes.Remove(node);

            FireChanged();
        }

        public void SetSort<TKey>(Func<T, TKey> keySelector, bool ascending)
        {
            if (_sorter is Sorter<T, TKey> s)
            {
                s.KeySelector = keySelector;
                s.Ascending = ascending;
                FireChanged();
            }
        }
    }
}
