using System;

namespace _Core._Global.Categories.Filtering
{
    public class SearchFilter<T> : IFilter<T>
    {
        public Func<T, string> TextSelector { get; }
        public string SearchTerm { get; set; } = string.Empty;

        public SearchFilter(Func<T, string> selector)
        {
            TextSelector = selector;
        }

        public bool Match(T item)
        {
            if (string.IsNullOrWhiteSpace(SearchTerm))
                return true;

            var text = TextSelector(item) ?? string.Empty;
            return text.IndexOf(SearchTerm, StringComparison.OrdinalIgnoreCase) >= 0;
        }
    }
}