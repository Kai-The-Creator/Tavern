// CookPanel.cs
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Core.GameEvents.Cooking.Scripts.HUD
{
    public class CookPanel : CanvasPanel
    {
        [SerializeField] private Slider progressSlider;
        [SerializeField] private TMP_Text percentLabel;

        public void SetProgress(float normalized)
        {
            progressSlider.value = normalized;
            percentLabel.text    = $"{Mathf.RoundToInt(normalized * 100)} %";
        }
    }
}