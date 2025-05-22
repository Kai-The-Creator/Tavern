using System.Collections.Generic;
// using _Core.GameEvents.Cooking.Scripts;
using UnityEngine;

namespace _Core._Global.ItemSystem
{
    public interface IWorkstationRecipe
    {
        IReadOnlyList<WorkstationTag> Workstations { get; }
    }

    public interface ICraftRecipe : IWorkstationRecipe
    {
        ItemConfig ResultItem { get; }
        IReadOnlyList<MaterialRequirement> Materials { get; }
    }

    public interface ICookRecipe : IWorkstationRecipe
    {
        // CookingDishObject ResultDish { get; }
        IReadOnlyList<IngredientRequirement> Ingredients { get; }
    }
    
    [System.Serializable]
    public struct MaterialRequirement
    {
        public MaterialConfig Resource;
        [Min(1)] public int   Count;

        // NEW — позволяет использовать синтаксис (res, cnt)
        public void Deconstruct(out MaterialConfig resource, out int count)
        {
            resource = Resource;
            count    = Count;
        }
    }
    
    [System.Serializable]
    public struct IngredientRequirement
    {
        public IngredientConfig Ingredient;
        [Min(1)] public int Quantity;
    }
}