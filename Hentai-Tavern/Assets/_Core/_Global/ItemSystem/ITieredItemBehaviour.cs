using _Core._Global.ItemSystem.Behaviours;

namespace _Core._Global.ItemSystem
{
    public interface ITieredItemBehaviour : IItemBehaviour
    {
        bool TryUnlockTier (ItemState state, Rarity tier);
        bool TryLockTier   (ItemState state, Rarity tier);
        bool IsTierUnlocked(ItemState state, Rarity tier);
    }

    /// <summary>Стратегия для ЛЮБОГО предмета с редкостями.</summary>
    public sealed class TieredItemBehaviour : ITieredItemBehaviour
    {
        public bool Match(ItemConfig cfg) => cfg is ITieredItemConfig;

        public bool TryAdd   (ItemState s, int amt) => false;
        public bool TryConsume(ItemState s, int amt) => false;
        public bool TryUnlock(ItemState s) => false;

        public bool TryUnlockTier (ItemState s, Rarity r) => s.UnlockTier(r);
        public bool TryLockTier   (ItemState s, Rarity r) => s.LockTier(r);
        public bool IsTierUnlocked(ItemState s, Rarity r)=> s.IsTierUnlocked(r);
    }
}