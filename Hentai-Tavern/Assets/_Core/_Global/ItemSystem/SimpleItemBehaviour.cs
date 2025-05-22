using _Core._Global.ItemSystem;

namespace _Core._Global.ItemSystem.Behaviours
{
    /// <summary>Не-стекаемый предмет, у которого нет rarity-tier’ов.</summary>
    public sealed class SimpleItemBehaviour : IItemBehaviour
    {
        public bool Match(ItemConfig cfg) => !cfg.Stackable && !(cfg is WeaponConfig);

        public bool TryAdd(ItemState s, int amount)  => false;   // нельзя добавлять копии
        public bool TryConsume(ItemState s, int amt) => false;   // и расходовать тоже

        public bool TryUnlock(ItemState s) => s.Unlock();        // один единственный флаг
    }
}