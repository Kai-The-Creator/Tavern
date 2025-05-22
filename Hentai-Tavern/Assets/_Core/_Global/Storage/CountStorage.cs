using System.Collections.Generic;

namespace _Core._Global.Storage
{
    public class CountStorage<TItem> : ICountStorage<TItem>
    {
        private readonly Dictionary<TItem, int> _dict = new();

        public void Add(TItem item, int count = 1)
        {
            if (count <= 0) return;
            _dict[item] = _dict.TryGetValue(item, out var v) ? v + count : count;
        }

        public bool TryRemove(TItem item, int count = 1)
        {
            if (count <= 0 || !_dict.TryGetValue(item, out var v) || v < count)
                return false;

            int newV = v - count;
            if (newV > 0) _dict[item] = newV;
            else _dict.Remove(item);

            return true;
        }

        public int GetCount(TItem item)
        {
            return _dict.GetValueOrDefault(item, 0);
        }

        public IReadOnlyCollection<TItem> GetAllItems()
        {
            return _dict.Keys;
        }

        public void Clear()
        {
            _dict.Clear();
        }
    }
}