// Assets/_Core/_Global/Equip/IEquipService.cs
using System;
using System.Collections.Generic;
using _Core._Global.ItemSystem;
using _Core._Global.Services;

namespace _Core._Global.Equip
{
    public interface IEquipService : IService
    {
        bool TryEquip(ItemState item, Rarity tier, out EquipGroup group, out int index);
        bool TryUnequip(EquipGroup group, int index);

        EquippedEntry? Get(EquipGroup group, int index);
        IReadOnlyList<ItemConfig> GetEquippedItems();  

        /// group, index, old?, new?
        event Action<EquipGroup,int,EquippedEntry?,EquippedEntry?> OnChanged;
    }
}