// StackTooltipView.cs
using _Core._Global.UI.Tooltips;
using TMPro;
using UnityEngine;

namespace _Core._Global.UI
{
    public sealed class StackTooltipView : BaseTooltipView
    {
        [SerializeField] private TextMeshProUGUI _qty;

        protected override void BindSpecific(in TooltipData d)
        {
            _qty.gameObject.SetActive(d.Quantity.HasValue);
            if (d.Quantity.HasValue) _qty.text = $"x{d.Quantity.Value}";
        }
    }
}