using System.Collections.Generic;

namespace _Core._Global.Storage
{
    public interface IFlagStorage<TItem>
    {
        void Unlock(TItem item);
        bool TryLock(TItem item);
        bool IsUnlocked(TItem item);
        void Clear();

        IReadOnlyCollection<TItem> GetAllUnlocked();
    }
}