// SelectedOrderView.cs

using System;
using _Core._Global.ItemSystem;
using UnityEngine;
using TMPro;

namespace _Core.GameEvents.Cooking.Scripts
{
    public class SelectedOrderView : MonoBehaviour
    {
        [SerializeField] private TMP_Text title;
        [SerializeField] private Transform ingredientsRoot;
        [SerializeField] private RequiredIngredientView reqPrefab;

        public OrderRecord Record { get; private set; }

        public void SetData(OrderRecord rec)
        {
            Record = rec;
            title.text = rec.DishName;

            foreach (Transform c in ingredientsRoot) Destroy(c.gameObject);
            foreach (var r in rec.RequiredIngredients)
            {
                var v = Instantiate(reqPrefab, ingredientsRoot);
                  v.Init(r.Ingredient.Icon, r.Quantity);
            }
        }

        public void ResetView()
        {
            title.text = string.Empty;
            foreach (Transform c in ingredientsRoot) Destroy(c.gameObject);
        }

        public void DecrementIngredient(IngredientConfig cfg)
        {
            foreach (RequiredIngredientView v in ingredientsRoot.GetComponentsInChildren<RequiredIngredientView>())
            {
                if (v.Matches(cfg))
                {
                    v.Decrement();
                    break;
                }
            }
        }
    }
}