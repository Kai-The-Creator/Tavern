using _Core._Global.ItemSystem;

namespace _Core._Global.ItemSystem.Behaviours
{
    /// <summary>Простое поведение для ресурса, который можно стакать.</summary>
    public sealed class StackableItemBehaviour : IItemBehaviour
    {
        public bool Match(ItemConfig cfg) => cfg.Stackable;
        public bool TryAdd(ItemState s, int amount)
        {
            if (amount <= 0) return false;

            s.Add(amount);
            return true;
        }

        public bool TryConsume(ItemState s, int amount) => s.Consume(amount);
        public bool TryUnlock(ItemState s) => false;

    }
}