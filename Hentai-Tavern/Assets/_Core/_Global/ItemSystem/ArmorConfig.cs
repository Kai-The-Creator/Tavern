using System;
using System.Collections.Generic;
using System.Linq;
using _Core._Global.Equip;
using _Core._Global.ItemSystem;
using _Core._Global.UI.Tooltips;
using UnityEngine;

namespace _Core._Global.ItemSystem
{
    [CreateAssetMenu(menuName = "GAME/Items/Armor", fileName = "Armor_")]
    public sealed class ArmorConfig : ItemConfig, ITieredItemConfig, IStatProvider, IWearable
    {
        [SerializeField] private EquipGroup[] _groups = { EquipGroup.Armor };
        public IReadOnlyList<EquipGroup> AllowedGroups => _groups;
        
        [SerializeField] private List<Tier<ArmorTierStats>> _tiers;

        IReadOnlyList<TierMeta> ITieredItemConfig.Metas =>
            _tiers.Select(t => t.Meta).ToList();
        public bool HasTier(Rarity r) => _tiers.Any(t => t.Meta.Rarity == r);

        public IReadOnlyList<Tier<ArmorTierStats>> Tiers => _tiers;

        public int   Defense    (Rarity r) => Find(r).Stats.Defense;
        public float Resistance (Rarity r) => Find(r).Stats.Resistance;
        private Tier<ArmorTierStats> Find(Rarity r) => _tiers[(int)r];

        public override bool Stackable => false;
        
        public IReadOnlyList<StatPair> GetStats(Rarity r)
        {
            return new[]
            {
                new StatPair("Защита",     Defense(r).ToString(),      null, Color.white),
                new StatPair("Сопротив.", $"{Resistance(r):0} %",      null, Color.white)
            };
        }
    }
}