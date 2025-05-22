using System.Collections.Generic;
using System.Linq;

namespace _Core._Global.Categories.Filtering
{
    public class FilterGroup<T> : IFilter<T>
    {
        public enum Mode
        {
            And,
            Or
        }

        public Mode GroupMode { get; set; } = Mode.And;
        public List<IFilter<T>> Filters { get; } = new List<IFilter<T>>();

        public bool Match(T item)
        {
            return GroupMode == Mode.And
                ? Filters.All(f => f.Match(item))
                : Filters.Any(f => f.Match(item));
        }
    }
}