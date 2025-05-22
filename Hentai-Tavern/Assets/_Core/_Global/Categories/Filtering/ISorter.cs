using System.Collections.Generic;

namespace _Core._Global.Categories.Filtering
{
    public interface ISorter<T>
    {
        IEnumerable<T> Sort(IEnumerable<T> items);
    }
}