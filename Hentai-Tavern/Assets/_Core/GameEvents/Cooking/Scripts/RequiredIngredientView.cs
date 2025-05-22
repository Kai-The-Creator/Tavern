// RequiredIngredientView.cs

using _Core._Global.ItemSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Core.GameEvents.Cooking.Scripts
{
    public class RequiredIngredientView : MonoBehaviour
    {
        [SerializeField] private Image   icon;
        [SerializeField] private TMP_Text amountLabel;

        private int amount;

        public void Init(Sprite spr, int amt)
        {
            icon.sprite = spr;
            amount      = amt;
            UpdateLabel();
        }

        public bool Matches(IngredientConfig cfg) => icon.sprite == cfg.Icon;

        public void Decrement()
        {
            amount = Mathf.Max(0, amount-1);
            UpdateLabel();
        }

        private void UpdateLabel() => amountLabel.text = $"x{amount}";
    }
}