using System;
using System.Collections.Generic;
using System.Linq;
using _Core._Global.Equip;
using _Core._Global.UI.Tooltips;
using UnityEngine;

namespace _Core._Global.ItemSystem
{
    [CreateAssetMenu(menuName = "GAME/Items/Amulet", fileName = "Amulet_")]
    public class AmuletConfig : ItemConfig, ITieredItemConfig, IStatProvider, IWearable
    {
        [SerializeField] private EquipGroup[] _groups = { EquipGroup.Amulet };
        public IReadOnlyList<EquipGroup> AllowedGroups => _groups;
        
        [SerializeField] private List<Tier<AmuletStats>> _tiers;

        // ------------- ITieredItemConfig -------------
        IReadOnlyList<TierMeta> ITieredItemConfig.Metas => _tiers.Select(t => t.Meta).ToList();
        public bool HasTier(Rarity r) => _tiers.Any(t => t.Meta.Rarity == r);

        public IReadOnlyList<Tier<AmuletStats>> Tiers => _tiers;

        // ------------- геймплей API -------------
        public int   Heal (Rarity r) => Find(r).Stats.Heal;
        private Tier<AmuletStats> Find(Rarity r) => _tiers[(int)r];

        public override bool Stackable => false;
        
        public IReadOnlyList<StatPair> GetStats(Rarity r)
        {
            return new[]
            {
                new StatPair("+ХП",     Heal(r).ToString(),      null, Color.white),
            };
        }
    }
}
