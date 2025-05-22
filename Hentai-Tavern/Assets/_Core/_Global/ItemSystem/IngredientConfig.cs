// using _Core.GameEvents.Cooking.Scripts;
using _Core.GameEvents.Cooking.Scripts;
using UnityEngine;

namespace _Core._Global.ItemSystem
{
    [CreateAssetMenu(menuName = "GAME/Items/Ingredient", fileName = "Ingredient_")]
    public class IngredientConfig : ResourceConfig
    {
        [Header("Cooking")]
        [Tooltip("Prefab shown on cutting board.")]
        [SerializeField] private CookingIngredientObject  _renderObject;

        [Tooltip("Clicks required to chop / prepare.")]
        [Min(1)][SerializeField] private int _chopClicksRequired = 5;

        public CookingIngredientObject  RenderObject   => _renderObject;
        public int                     ChopClickCount => _chopClicksRequired;
    }
}