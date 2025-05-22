// CuttingPanel.cs
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using _Core._Global.ItemSystem;

namespace _Core.GameEvents.Cooking.Scripts.HUD
{
    public class CuttingPanel : CanvasPanel
    {
        [Header("Inventory Grid")]
        [SerializeField] private CookingInventoryItemView itemPrefab;
        [SerializeField] private Transform                gridRoot;

        [Header("Chopping Progress")]
        [SerializeField] private Slider    progressSlider;
        [SerializeField] private TMP_Text  percentLabel;

        private readonly List<CookingInventoryItemView> items = new();

        /// <summary>
        /// Построить сетку ингредиентов + сбросить прогресс.
        /// </summary>
        public void BuildInventory(
            Dictionary<IngredientConfig,int> stock,
            Action<IngredientConfig> onClick)
        {
            // очистка
            foreach (var it in items) Destroy(it.gameObject);
            items.Clear();

            // сброс прогресса
            SetProgress(0f);

            // инвентарь
            foreach (var kv in stock)
            {
                var view = Instantiate(itemPrefab, gridRoot);
                view.Init(kv.Key, kv.Value);
                view.Clicked += onClick;
                items.Add(view);
            }

            Show();
        }

        /// <summary>
        /// Обновить количество одного ингредиента.
        /// </summary>
        public void UpdateAmount(IngredientConfig cfg, int amount)
        {
            var view = items.Find(v => v.Config == cfg);
            if (view) view.SetAmount(amount);
        }

        /// <summary>
        /// Установить и отобразить прогресс нарезки [0…1].
        /// </summary>
        public void SetProgress(float normalized)
        {
            progressSlider.value = normalized;
            percentLabel.text    = $"{Mathf.RoundToInt(normalized * 100)} %";
        }
    }
}