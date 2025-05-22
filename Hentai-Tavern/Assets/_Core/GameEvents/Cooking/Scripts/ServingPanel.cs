// ServingPanel.cs
using System;
using UnityEngine;
using UnityEngine.UI;

namespace _Core.GameEvents.Cooking.Scripts.HUD
{
    public class ServingPanel : CanvasPanel
    {
        [SerializeField] private Button serveButton;

        public event Action ServeClicked;

        protected override void Awake()
        {
            base.Awake();
            serveButton.onClick.AddListener(() => ServeClicked?.Invoke());
        }
    }
}