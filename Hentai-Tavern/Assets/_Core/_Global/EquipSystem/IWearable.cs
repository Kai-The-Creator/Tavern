// Assets/_Core/_Global/Equip/IWearable.cs
using System.Collections.Generic;
using _Core._Global.Equip;
using _Core._Global.ItemSystem;

namespace _Core._Global.Equip
{
    public enum EquipGroup
    {
        Weapon,
        Armor,
        Ring,
        Amulet
        // Добавите Shield, Trinket … — просто допишите пункт.
    }
    
    /// <summary>Scriptable-конфиг, который можно надеть.</summary>
    public interface IWearable
    {
        /// К каким группам предмет подходит (обычно одна).
        IReadOnlyList<EquipGroup> AllowedGroups { get; }
    }
    
    public readonly struct EquippedEntry
    {
        public readonly ItemState State;
        public readonly Rarity    Tier;

        public EquippedEntry(ItemState st, Rarity tier)
        {
            State = st;
            Tier  = tier;
        }
    }
}