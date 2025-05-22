using System.Collections.Generic;

namespace _Core._Global.Storage
{
    public interface ICountStorage<TItem>
    {
        void Add(TItem item, int count = 1);
        bool TryRemove(TItem item, int count = 1);
        int GetCount(TItem item);
        void Clear();

        IReadOnlyCollection<TItem> GetAllItems();
    }
}