// CraftRecipeConfig.cs

using System.Collections.Generic;
using _Core.GameEvents.Common;
using _Core.GameEvents.Forging.Scripts;
using UnityEngine;

namespace _Core._Global.ItemSystem
{
    [CreateAssetMenu(menuName = "GAME/Recipes/Craft", fileName = "RecipeCraft_")]
    public class CraftRecipeConfig : BaseRecipeConfig, ICraftRecipe
    {
        [Header("Result Item")]
        [SerializeField] private ItemConfig _resultItem;

        [Header("Materials")]
        [SerializeField] private MaterialRequirement[] _materials;
        
        [Header("Forge FX")]
        [SerializeField] private ProductModel  _productPrefab;
        
        public ProductModel ProductPrefab => _productPrefab;
        public ItemConfig                       ResultItem => _resultItem;
        public IReadOnlyList<MaterialRequirement> Materials   => _materials;
    }
}