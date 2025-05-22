using System.Collections.Generic;

namespace _Core._Global.Storage
{
    public class FlagStorage<TItem> : IFlagStorage<TItem>
    {
        private readonly HashSet<TItem> _set = new();

        public void Unlock(TItem item)
        {
            _set.Add(item);
        }

        public bool TryLock(TItem item)
        {
            return _set.Remove(item);
        }

        public bool IsUnlocked(TItem item)
        {
            return _set.Contains(item);
        }

        public IReadOnlyCollection<TItem> GetAllUnlocked()
        {
            return _set;
        }

        public void Clear()
        {
            _set.Clear();
        }
    }
}