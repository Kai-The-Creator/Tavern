// ItemState.cs  ─ обновлённая версия
using System.Collections.Generic;

namespace _Core._Global.ItemSystem
{
    /// <summary>Рантаймовое состояние одного <see cref="ItemConfig"/>.</summary>
    public sealed class ItemState
    {
        public ItemConfig                Config         { get; }
        public int                       Quantity       { get; private set; }
        private bool _explicitUnlocked;
        /// <remarks>
        /// Предмет «открыт», если открыт хотя бы один tier
        /// ИЛИ если он <see cref="ItemConfig.Stackable"/>.
        /// </remarks>
        public bool Unlocked =>
            _explicitUnlocked ||                       // обычный предмет
            Config.Stackable   ||                      // ресурсы считаются «открыты» всегда
            _unlockedTiers.Count > 0;                

        private readonly HashSet<Rarity> _unlockedTiers = new();

        public IReadOnlyCollection<Rarity> UnlockedTiers => _unlockedTiers;

        public ItemState(ItemConfig config) => Config = config;
        
        internal bool Unlock()
        {
            if (_explicitUnlocked) return false;
            _explicitUnlocked = true;
            return true;
        }

        internal bool Lock()            // если вдруг понадобится
        {
            if (!_explicitUnlocked) return false;
            _explicitUnlocked = false;
            return true;
        }

        // ───────────── tier ops
        internal bool UnlockTier(Rarity r) => _unlockedTiers.Add(r);
        internal bool LockTier  (Rarity r) => _unlockedTiers.Remove(r);
        internal bool IsTierUnlocked(Rarity r) => _unlockedTiers.Contains(r);

        // ───────────── quantity ops  (для Stackable)
        internal void Add(int amount)       { if (amount > 0) Quantity += amount; }
        internal bool Consume(int amount)
        {
            if (amount <= 0 || Quantity < amount) return false;
            Quantity -= amount;
            return true;
        }
    }
}