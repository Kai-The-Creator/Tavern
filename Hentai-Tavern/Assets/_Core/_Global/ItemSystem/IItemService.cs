using System;
using System.Collections.Generic;
using _Core._Global.Services;

namespace _Core._Global.ItemSystem
{
    public interface IItemService : IService
    {
        event Action<ItemState, ItemChangeReason> OnChanged;

        bool TryAdd(ItemConfig cfg, int amount = 1);
        bool TryConsume(ItemConfig cfg, int amount = 1);
        bool TryUnlock(ItemConfig cfg);

        public bool TryUnlockTier(ItemConfig cfg, Rarity tier);

        public bool TryLockTier(ItemConfig cfg, Rarity tier);

        public bool IsTierUnlocked(ItemConfig cfg, Rarity tier);
        
        bool Has(ItemConfig cfg, int amount = 1);
        bool IsUnlocked(ItemConfig cfg);
        int  GetQuantity(ItemConfig cfg);

        ItemState Get(ItemConfig cfg);
        IReadOnlyCollection<ItemState> Query(Func<ItemState, bool> filter);
        
        IReadOnlyList<TItem> GetUnlockedItems<TItem>() where TItem : ItemConfig;
    }

    public enum ItemChangeReason { Added, Consumed, Unlocked, Locked, Upgraded }
}