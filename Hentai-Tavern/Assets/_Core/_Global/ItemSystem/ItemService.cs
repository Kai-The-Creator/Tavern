using System;
using System.Collections.Generic;
using System.Linq;
using _Core._Global.ItemSystem.Behaviours;
using Cysharp.Threading.Tasks;
using UnityEngine;
using _Core._Global.Services;

namespace _Core._Global.ItemSystem
{
    /// <summary>Фасад для работы с предметами, их количеством и доступностью.</summary>
    public sealed class ItemService : AService, IItemService
    {
        private const string SaveKey = "items_v2";

        public event Action<ItemState, ItemChangeReason> OnChanged;

        private readonly Dictionary<string, ItemState> _items = new();
        private readonly Dictionary<string, ItemConfig> _configs = new();
        private readonly List<IItemBehaviour> _behaviours = new();

        [SerializeField, Tooltip("Global list of all ItemConfig assets.")]
        private ItemCatalog _catalog;

        // ────────────────────────── lifecycle
        public override async UniTask OnStart()
        {
            RegisterDefaultBehaviours();
            LoadCatalog();
            await LoadAsync();
            SeedInitialGrants();
        }

        #region Seed

        private void SeedInitialGrants()
        {
            foreach (var cfg in _configs.Values)
            {
                if (_items.ContainsKey(cfg.Id)) continue;         // уже есть в save

                switch (cfg)
                {
                    // ─── tier-предметы ──────────────────────────
                    case ITieredItemConfig tiered:
                    {
                        var st = GetOrCreate(cfg);                // всегда создаём

                        foreach (var meta in tiered.Metas)
                            if (meta.StartUnlocked)
                                st.UnlockTier(meta.Rarity);       // автo-анлок
                        break;
                    }

                    // ─── ресурсы (stackable) ────────────────────
                    case { Stackable: true, StartQuantity: > 0 }:
                        TryAdd(cfg, cfg.StartQuantity);           // событие сгенерирует TryAdd
                        break;

                    // ─── простые non-stackable (ключи, рецепты…) ─
                    default:
                    {
                        var st = GetOrCreate(cfg);                // ← ❶ создаём всегда
                        if (cfg.StartUnlocked)
                            st.Unlock();                          // автo-анлок, как раньше
                        break;
                    }
                }
            }
        }

        #endregion

        #region Behaviour registration

        private void RegisterDefaultBehaviours()
        {
            _behaviours.AddRange(new IItemBehaviour[]
            {
                new StackableItemBehaviour(), // ресурсы
                new SimpleItemBehaviour(), // предметы без tier’ов
                new TieredItemBehaviour() // оружие с rarity-tier’ами
            });
        }

        #endregion

        #region Catalog load

        private void LoadCatalog()
        {
            if (_catalog == null)
            {
                Debug.LogError("ItemCatalog not found!");
                return;
            }

            foreach (var cfg in _catalog.Items.Where(i => i != null))
                if (!_configs.TryAdd(cfg.Id, cfg))
                    Debug.LogWarning($"Duplicate item id '{cfg.Id}'", cfg);
        }

        #endregion

        // ────────────────────────── public API
        public bool TryAdd(ItemConfig cfg, int amount = 1) =>
            UseBehaviour(cfg, ItemChangeReason.Added,
                b => b.TryAdd(GetOrCreate(cfg), amount));

        public bool TryConsume(ItemConfig cfg, int amount = 1) =>
            UseBehaviour(cfg, ItemChangeReason.Consumed,
                b => b.TryConsume(GetOrCreate(cfg), amount));

        public bool TryUnlock(ItemConfig cfg) =>
            UseBehaviour(cfg, ItemChangeReason.Unlocked,
                b => b.TryUnlock(GetOrCreate(cfg)));

        public bool TryUnlockTier(ItemConfig cfg, Rarity tier) =>
            UseTierBehaviour(cfg, ItemChangeReason.Unlocked,
                t => t.TryUnlockTier(GetOrCreate(cfg), tier));

        public bool TryLockTier(ItemConfig cfg, Rarity tier) =>
            UseTierBehaviour(cfg, ItemChangeReason.Locked,
                t => t.TryLockTier(GetOrCreate(cfg), tier));

        public bool IsTierUnlocked(ItemConfig cfg, Rarity tier) =>
            Get(cfg) is { } st && (cfg is ITieredItemConfig) && st.IsTierUnlocked(tier);

        public bool Has(ItemConfig cfg, int amount = 1)
        {
            var st = Get(cfg);
            return st != null && (cfg.Stackable ? st.Quantity >= amount : st.Unlocked);
        }

        public bool IsUnlocked(ItemConfig cfg) => Get(cfg)?.Unlocked ?? false;
        public int GetQuantity(ItemConfig cfg) => Get(cfg)?.Quantity ?? 0;

        public ItemState Get(ItemConfig cfg) =>
            _items.GetValueOrDefault(cfg.Id);

        public IReadOnlyCollection<ItemState> Query(Func<ItemState, bool> predicate) =>
            _items.Values.Where(predicate).ToList();

        // ────────────────────────── helpers
        private ItemState GetOrCreate(ItemConfig cfg)
        {
            if (!_items.TryGetValue(cfg.Id, out var s))
                _items[cfg.Id] = s = new ItemState(cfg);
            return s;
        }

        private bool UseBehaviour(ItemConfig cfg, ItemChangeReason reason,
            Func<IItemBehaviour, bool> op)
        {
            var beh = _behaviours.FirstOrDefault(b => b.Match(cfg));
            if (beh == null)
            {
                Debug.LogError($"No behaviour found for {cfg.name}");
                return false;
            }

            var st = GetOrCreate(cfg);
            if (!op(beh)) return false;

            OnChanged?.Invoke(st, reason);
            return true;
        }

        private bool UseTierBehaviour(ItemConfig cfg, ItemChangeReason reason,
            Func<ITieredItemBehaviour, bool> op)
        {
            var beh = _behaviours
                .OfType<ITieredItemBehaviour>()
                .FirstOrDefault(b => b.Match(cfg));
            if (beh == null) return false;

            var st = GetOrCreate(cfg);
            if (!op(beh)) return false;

            OnChanged?.Invoke(st, reason);
            return true;
        }
        
        public IReadOnlyList<TItem> GetUnlockedItems<TItem>() where TItem : ItemConfig
        {
            return _items.Values
                .Where(s => s.Unlocked && s.Config is TItem)
                .Select(s => (TItem)s.Config)
                .ToList()
                .AsReadOnly();
        }

        // ────────────────────────── save / load (v2 – без уровней)
        private async UniTask LoadAsync()
        {
            // при необходимости добавь код чтения save-файла
        }

        public async UniTask SaveAsync()
        {
            // при необходимости добавь код записи save-файла
        }
    }
}