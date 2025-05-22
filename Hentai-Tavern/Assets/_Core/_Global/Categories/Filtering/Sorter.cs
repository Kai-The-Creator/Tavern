using System;
using System.Collections.Generic;
using System.Linq;

namespace _Core._Global.Categories.Filtering
{
    public class Sorter<T, TKey> : ISorter<T>
    {
        public Func<T, TKey> KeySelector { get; set; }
        public bool Ascending { get; set; } = true;

        public IEnumerable<T> Sort(IEnumerable<T> items)
        {
            if (KeySelector == null) return items;
            return Ascending
                ? items.OrderBy(KeySelector)
                : items.OrderByDescending(KeySelector);
        }
    }
}