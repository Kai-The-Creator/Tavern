using System.Collections.Generic;
using System.Linq;
using _Core._Global.UI.Tooltips;
using UnityEngine;

namespace _Core._Global.ItemSystem
{
    [CreateAssetMenu(menuName = "GAME/Items/Potion", fileName = "Potion_")]
    public class PotionConfig : ItemConfig, ITieredItemConfig, IStatProvider
    {
        [SerializeField] private List<Tier<PotionStats>> _tiers;

        // ------------- ITieredItemConfig -------------
        IReadOnlyList<TierMeta> ITieredItemConfig.Metas => _tiers.Select(t => t.Meta).ToList();
        public bool HasTier(Rarity r) => _tiers.Any(t => t.Meta.Rarity == r);

        public IReadOnlyList<Tier<PotionStats>> Tiers => _tiers;

        // ------------- геймплей API -------------
        public int   Heal (Rarity r) => Find(r).Stats.Heal;
        private Tier<PotionStats> Find(Rarity r) => _tiers[(int)r];

        public override bool Stackable => false;
        
        public IReadOnlyList<StatPair> GetStats(Rarity r)
        {
            return new[]
            {
                new StatPair("+Здоровье",     Heal(r).ToString(),      null, Color.white),
            };
        }
    }
}
