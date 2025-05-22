// Assets/_Core/_Global/UI/Tooltips/StatRowView.cs
using _Core._Global.UI.Tooltips;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Core._Global.UI
{
    /// <summary>Отображает одну характеристику предмета.</summary>
    public sealed class StatRowView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _label;
        [SerializeField] private TextMeshProUGUI _value;
        [SerializeField] private Image           _icon;

        public void Bind(in StatPair stat)
        {
            _label.text  = stat.Label;
            _value.text  = stat.Value;
            _value.color = stat.Color;

            _icon.enabled = stat.Icon;
            if (stat.Icon) _icon.sprite = stat.Icon;
        }
    }
}