// Assets/_Core/_Global/Equip/EquipService.cs
using System;
using System.Collections.Generic;
using _Core._Global.ItemSystem;
using _Core._Global.Services;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Core._Global.Equip
{
    public sealed class EquipService : AService, IEquipService
    {
        /* вместимость групп */
        private readonly Dictionary<EquipGroup,int> _capacity = new()
        {
            { EquipGroup.Weapon, 1 },
            { EquipGroup.Armor,  1 },
            { EquipGroup.Ring,   2 },
            { EquipGroup.Amulet, 1 }
        };

        /* Group → array[capacity] of entries (null = свободно) */
        private readonly Dictionary<EquipGroup, EquippedEntry?[]> _groups = new();

        /*──────────────────── public API ───────────────────────────*/

        public EquippedEntry? Get(EquipGroup g, int idx) =>
            _groups.TryGetValue(g, out var arr) && idx < arr.Length
                ? arr[idx] : null;

        public bool TryEquip(ItemState item, Rarity tier, out EquipGroup outGroup, out int outIdx)
        {
            outGroup = default; outIdx = -1;

            if (item == null || item.Config is not IWearable wear) return false;

            bool isTiered = item.Config is ITieredItemConfig
                          && ((ITieredItemConfig)item.Config).Metas.Count > 0;

            // ── Проверка, открыт ли tier / предмет ──────────────────
            if (isTiered)
            {
                if (!item.IsTierUnlocked(tier)) return false;
            }
            else
            {
                if (!item.Unlocked) return false;
            }

            // ── 0. ищем, надет ли предмет уже (в любой группе/ячейке) ─
            if (TryFindEntry(item, out var grpFound, out var idxFound))
            {
                var arr = _groups[grpFound];
                var old = arr[idxFound];

                if (isTiered && old!.Value.Tier != tier)
                {   // заменить только tier
                    arr[idxFound] = new EquippedEntry(item, tier);
                    OnChanged?.Invoke(grpFound, idxFound, old, arr[idxFound]);

                    outGroup = grpFound; outIdx = idxFound;
                    return true;
                }
                return false; // тот же tier уже надет
            }

            // ── 1. выбираем слот для надевания -----------------------
            foreach (var g in wear.AllowedGroups)
            {
                int cap = _capacity.GetValueOrDefault(g, 0);
                if (cap == 0) continue;

                if (!_groups.TryGetValue(g, out var arr))
                {
                    arr = new EquippedEntry?[cap];
                    _groups[g] = arr;
                }

                // 1a) первая пустая ячейка
                for (int i = 0; i < cap; i++)
                    if (arr[i] == null)
                    {
                        arr[i] = new EquippedEntry(item, tier);
                        OnChanged?.Invoke(g, i, null, arr[i]);
                        outGroup = g; outIdx = i;
                        return true;
                    }

                // 1b) нет пустых → заменяем последнюю
                int last = cap - 1;
                var oldLast = arr[last];
                arr[last]   = new EquippedEntry(item, tier);
                OnChanged?.Invoke(g, last, oldLast, arr[last]);
                outGroup = g; outIdx = last;
                return true;
            }
            return false;
        }

        public bool TryUnequip(EquipGroup g, int idx)
        {
            if (!_groups.TryGetValue(g, out var arr) || idx >= arr.Length) return false;
            if (arr[idx] == null) return false;

            var old = arr[idx];
            arr[idx] = null;
            OnChanged?.Invoke(g, idx, old, null);
            return true;
        }

        public event Action<EquipGroup,int,EquippedEntry?,EquippedEntry?> OnChanged;
        
        public IReadOnlyList<ItemConfig> GetEquippedItems()
        {
            var list = new List<ItemConfig>(8);

            foreach (var arr in _groups.Values)          // _groups: Dictionary<EquipGroup,EquippedEntry[]>
            foreach (var entry in arr)
                if (entry is { } e)
                    list.Add(e.State.Config);

            return list.AsReadOnly();                    // ← неизменяемая коллекция
        }

        /*──────────────────── helpers ───────────────────────────────*/
        private bool TryFindEntry(ItemState st, out EquipGroup g, out int idx)
        {
            foreach (var kv in _groups)
            {
                var arr = kv.Value;
                for (int i = 0; i < arr.Length; i++)
                    if (arr[i]?.State == st)
                    { g = kv.Key; idx = i; return true; }
            }
            g = default; idx = -1; return false;
        }

        public override UniTask OnStart()
        {
            return UniTask.CompletedTask;
        }
    }
}
