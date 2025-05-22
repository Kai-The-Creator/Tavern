using System;
using _Core._Global.ItemSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Core.GameEvents.Enchantment.UI
{
    public sealed class EnchantmentRecipeButtonView : MonoBehaviour
    {
        [SerializeField] private Button   button;
        [SerializeField] private TMP_Text recipeName;
        [SerializeField] private Image    recipeIcon;

        public CraftRecipeConfig Recipe { get; private set; }

        public void Init(CraftRecipeConfig recipe, Action<CraftRecipeConfig> onClick)
        {
            Recipe = recipe;

            recipeName.text = recipe.DisplayName;
            if (recipe.Icon) recipeIcon.sprite = recipe.Icon;

            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => onClick?.Invoke(recipe));
        }
    }
}