
using System.Collections.Generic;
using _Core._Global.ItemSystem;
using _Core._Global.UI.Tooltips;
using UnityEngine;

namespace _Core._Global.UI
{
    internal static class TooltipBuilder
    {
        public static TooltipData FromStack(ItemState st) =>
            new(TooltipKind.Stack, st, Rarity.Common, // State & Tier
                st.Config.Icon, st.Config.DisplayName, st.Config.Description,
                Rarity.Common, null, st.Quantity);

        public static TooltipData FromTier(ItemState st, Rarity r)
        {
            var meta = ((ITieredItemConfig)st.Config).Metas[(int)r];
            Sprite icon = meta.Icon ? meta.Icon : st.Config.Icon;

            return new TooltipData(
                TooltipKind.Tier, st, r,
                icon,
                st.Config.DisplayName,
                st.Config.Description,
                r,
                BuildStats(st.Config, r),
                color: meta.FrameColor);
        }

        private static IReadOnlyList<StatPair> BuildStats(ItemConfig cfg, Rarity r)
        {
            return cfg is IStatProvider sp ? sp.GetStats(r) : null;
        }
    }
}