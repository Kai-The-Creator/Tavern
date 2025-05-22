// Assets/_Core/_Global/UI/Tooltips/TooltipData.cs
using System.Collections.Generic;
using _Core._Global.ItemSystem;
using UnityEngine;

namespace _Core._Global.UI.Tooltips
{
    public enum TooltipKind { Stack, Tier }
    
    /// <summary>Данные, которыми заполняется TooltipWindow.</summary>
    public readonly struct TooltipData
    {
        public readonly TooltipKind Kind;        //  NEW
        public readonly ItemState   State;     // ← NEW
        public readonly Rarity      Tier;      // ← NEW (Valid только для Kind.Tier)
        public readonly Sprite      Icon;
        public readonly string      Name;
        public readonly string      Description;
        public readonly Rarity      Rarity;
        public readonly int?        Quantity;
        public readonly Color?        RareColor;
        public readonly IReadOnlyList<StatPair> Stats;

        public TooltipData(TooltipKind kind, ItemState state, Rarity tier, Sprite icon, string name, string desc,
            Rarity rarity,
            IReadOnlyList<StatPair> stats = null,
            int? quantity = null, Color? color = null)
        {
            Kind        = kind;
            State       = state;
            Tier        = tier;
            Icon        = icon;
            RareColor        = color;
            Name        = name;
            Description = desc;
            Rarity      = rarity;
            Stats       = stats;
            Quantity    = quantity;
        }
    }

    public readonly struct StatPair
    {
        public readonly string Label;
        public readonly string Value;
        public readonly Sprite Icon;
        public readonly Color  Color;

        public StatPair(string label, string value, Sprite icon, Color color)
        {
            Label = label;
            Value = value;
            Icon  = icon;
            Color = color;
        }
    }
}