// OrdersPanel.cs
using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace _Core.GameEvents.Cooking.Scripts.HUD
{
    public class OrdersPanel : CanvasPanel
    {
        [SerializeField] private OrderButtonView  buttonPrefab;
        [SerializeField] private Transform        contentRoot;
        [SerializeField] private Button startButton;

        public void SetStartInteractable(bool interactable, UnityAction onClick)
        {
            startButton.interactable = interactable;
            startButton.onClick.RemoveAllListeners();
            if (interactable) startButton.onClick.AddListener(onClick);
        }
        private readonly List<OrderButtonView> pool = new();

        public void BuildButtons(IEnumerable<OrderRecord> orders,
            Action<OrderButtonView> onClick)
        {
            Hide();
            foreach (var btn in pool) Destroy(btn.gameObject);
            pool.Clear();

            foreach (var record in orders)
            {
                var btn = Instantiate(buttonPrefab, contentRoot);
                btn.Init(record);
                btn.Clicked += onClick;
                pool.Add(btn);
            }

            Show();
        }
    }
}