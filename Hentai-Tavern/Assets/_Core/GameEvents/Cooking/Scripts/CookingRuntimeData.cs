// CookingRuntimeData.cs
using System.Collections.Generic;
using _Core._Global.ItemSystem;
using UnityEngine;

namespace _Core.GameEvents.Cooking.Scripts
{
    /// <summary>Локальный «срез» глобального инвентаря на момент старта мини-игры.</summary>
    public class CookingRuntimeData
    {
        public Dictionary<IngredientConfig,int> Stock { get; } 
            = new();

        public void InitFromGlobal(IItemService itemSvc,
            IEnumerable<IngredientConfig> neededIng)
        {
            Stock.Clear();
            foreach (var cfg in neededIng)
                Stock[cfg] = itemSvc.GetQuantity(cfg);
        }

        public bool TryConsume(IngredientConfig cfg, int amount,
            IItemService itemSvc)
        {
            if (!Stock.TryGetValue(cfg, out var have) || have < amount)
                return false;

            Stock[cfg] = have - amount;
            itemSvc.TryConsume(cfg, amount);
            return true;
        }
    }
}