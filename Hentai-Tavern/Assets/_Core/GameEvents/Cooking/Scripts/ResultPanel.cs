// ResultPanel.cs
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Core.GameEvents.Cooking.Scripts.HUD
{
    public class ResultPanel : CanvasPanel
    {
        [SerializeField] private TMP_Text resultLabel;
        [SerializeField] private Button   continueBtn;
        [SerializeField] private Button   exitBtn;

        public event Action ContinueClicked;
        public event Action ExitClicked;

        protected override void Awake()
        {
            base.Awake();
            continueBtn.onClick.AddListener(()=>ContinueClicked?.Invoke());
            exitBtn.onClick.AddListener(()=>ExitClicked?.Invoke());
        }

        public void Setup(string dishName, bool hasMoreOrders)
        {
            resultLabel.text      = $"{dishName} готово!";
            continueBtn.gameObject.SetActive(hasMoreOrders);
        }
    }
}