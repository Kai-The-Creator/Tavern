// BaseTooltipView.cs
using _Core._Global.UI.Tooltips;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Core._Global.UI
{
    /// <summary>Базовый тултип: имя, иконка, рарность, описание.</summary>
    public abstract class BaseTooltipView : MonoBehaviour
    {
        [Header("Common")]
        [SerializeField] protected Image           _icon;
        [SerializeField] protected TextMeshProUGUI _name;
        [SerializeField] protected TextMeshProUGUI _rarity;
        [SerializeField] protected TextMeshProUGUI _description;

        /// <summary>UI-заполнение. Вызывает специфическую реализацию.</summary>
        public void Bind(in TooltipData d)
        {
            // общие поля
            _icon.enabled = d.Icon;
            if (d.Icon) _icon.sprite = d.Icon;
            _name.text        = d.Name;
            _rarity.text      = d.Rarity.ToString();
            _description.text = d.Description;

            BindSpecific(d);                 // индивидуальные данные
        }

        protected abstract void BindSpecific(in TooltipData d);
    }
}
