// ─────────────────────────────────────────────────────────────────────────
// CookingInventoryItemView.cs  (кэш + очистка слушателей)
// ─────────────────────────────────────────────────────────────────────────
using System;
using _Core._Global.ItemSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Core.GameEvents.Cooking.Scripts
{
    public class CookingInventoryItemView : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private Image   _icon;
        [SerializeField] private TMP_Text _amountLabel;
        [SerializeField] private Button  _btn;

        public IngredientConfig Config { get; private set; }
        public int              Amount { get; private set; }

        public event Action<IngredientConfig> Clicked;

        private void Awake() =>
            _btn.onClick.AddListener(() => Clicked?.Invoke(Config));

        public void Init(IngredientConfig cfg, int amount)
        {
            Config        = cfg;
            _icon.sprite  = cfg.Icon;
            SetAmount(amount);
        }

        public void SetAmount(int amount)
        {
            Amount              = amount;
            _amountLabel.text   = $"x{amount}";
            _btn.interactable   = amount > 0;
        }
    }
}