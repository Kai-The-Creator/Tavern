// OrderButtonView.cs
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace _Core.GameEvents.Cooking.Scripts
{
    public class OrderButtonView : MonoBehaviour
    {
        [SerializeField] private Image   icon;
        [SerializeField] private TMP_Text title;
        [SerializeField] private TMP_Text countLabel;
        [SerializeField] private Button  btn;

        public OrderRecord Record { get; private set; }
        public event Action<OrderButtonView> Clicked;

        public void Init(OrderRecord rec)
        {
            Record     = rec;
            icon.sprite  = rec.Icon;
            title.text   = rec.DishName;
            countLabel.text = $"x{rec.Quantity}";
            btn.onClick.AddListener(()=>Clicked?.Invoke(this));
        }
        
        public void Refresh()
        {
            countLabel.text = $"x{Record.Quantity}";
            btn.interactable = Record.Quantity > 0;
        }
        
        public void UpdateQuantity() => countLabel.text = $"x{Record.Quantity}";
    }
}