// SelectedOrderPanel.cs

using System;
using UnityEngine;
using UnityEngine.UI;

namespace _Core.GameEvents.Cooking.Scripts.HUD
{
    public class SelectedOrderPanel : CanvasPanel
    {
        [SerializeField] private SelectedOrderView view;
        public SelectedOrderView View => view;
        public void Bind(OrderRecord record)     => view.SetData(record);
        public OrderRecord CurrentRecord => view.Record;
        
        [SerializeField] private Button cancelBtn;
        public  event Action CancelClicked;

        protected override void Awake()
        {
            base.Awake();
            cancelBtn.onClick.AddListener(()=>CancelClicked?.Invoke());
        }

        public void Clear()
        {
            View.ResetView();      // добавьте в SelectedOrderView простой метод, который очищает UI
            Hide();
        }
    }
}