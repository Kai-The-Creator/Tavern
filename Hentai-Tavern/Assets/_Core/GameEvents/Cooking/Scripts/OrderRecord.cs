// OrderRecord.cs
using System.Collections.Generic;
using _Core._Global.ItemSystem;
using UnityEngine;

namespace _Core.GameEvents.Cooking.Scripts
{
    [System.Serializable]
    public class OrderRecord
    {
        public CookRecipeConfig Recipe { get; }
        public int              Quantity { get; set; }

        public OrderRecord(CookRecipeConfig recipe, int qty)
        {
            Recipe   = recipe;
            Quantity = qty;
        }

        public string                  DishName            => Recipe.DisplayName;
        public Sprite                  Icon                => Recipe.Icon;
        public CookingDishObject       DishPrefab          => Recipe.ResultDish;
        public IReadOnlyList<IngredientRequirement> RequiredIngredients => Recipe.Ingredients;
    }
}