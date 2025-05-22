// CookRecipeConfig.cs

using System.Collections.Generic;
using _Core.GameEvents.Cooking.Scripts;
using UnityEngine;
// using _Core.GameEvents.Cooking.Scripts;

namespace _Core._Global.ItemSystem
{
    [CreateAssetMenu(menuName = "GAME/Recipes/Cook", fileName = "RecipeFood_")]
    public sealed class CookRecipeConfig : BaseRecipeConfig, ICookRecipe
    {
        [Header("Dish")]
        [SerializeField] private CookingDishObject _dish;

        [Header("Ingredients")]
        [SerializeField] private IngredientRequirement[] _ingredients;

        [Min(1)][SerializeField] private int _chopClicks = 10;

        public CookingDishObject                     ResultDish  => _dish;
        public IReadOnlyList<IngredientRequirement>  Ingredients => _ingredients;
    }
}