using System;
using System.Collections.Generic;
using System.Linq;
using _Core._Combat;
using _Core._Global.Equip;
using _Core._Global.ItemSystem;
using _Core._Global.UI.Tooltips;
using UnityEngine;

namespace _Core._Global.ItemSystem
{
    [CreateAssetMenu(menuName = "GAME/Items/Weapon", fileName = "Weapon_")]
    public sealed class WeaponConfig : ItemConfig, ITieredItemConfig, IStatProvider, IWearable, IAbilityProvider
    {
        [SerializeField] private EquipGroup[] _groups = { EquipGroup.Weapon };
        public IReadOnlyList<EquipGroup> AllowedGroups => _groups;

        [SerializeField] private List<Tier<WeaponTierStats>> _tiers;

        [Header("Abilities")]
        [SerializeField] private List<_Core._Combat.AbilitySO> _activeAbilities = new();
        [SerializeField] private List<_Core._Combat.PassiveAbilitySO> _passiveAbilities = new();
        
        // ------------- ITieredItemConfig -------------
        IReadOnlyList<TierMeta> ITieredItemConfig.Metas => _tiers.Select(t => t.Meta).ToList();
        public bool HasTier(Rarity r) => _tiers.Any(t => t.Meta.Rarity == r);

        public IReadOnlyList<Tier<WeaponTierStats>> Tiers => _tiers;

        // ------------- геймплей API -------------
        public int Damage(Rarity r) => Find(r).Stats.BaseDamage;
        public float Crit(Rarity r) => Find(r).Stats.CritChance;
        private Tier<WeaponTierStats> Find(Rarity r) => _tiers[(int)r];

        public override bool Stackable => false;

        public IReadOnlyList<StatPair> GetStats(Rarity r)
        {
            var t = _tiers[(int)r].Stats;
            return new[]
            {
                new StatPair("Шанс крита", $"{t.CritChance} %", null, Color.white),
                new StatPair("Урон", t.BaseDamage.ToString(), null, Color.white)
            };
        }

        public IReadOnlyList<_Core._Combat.AbilitySO> ActiveAbilities => _activeAbilities;
        public IReadOnlyList<_Core._Combat.PassiveAbilitySO> PassiveAbilities => _passiveAbilities;
    }
}