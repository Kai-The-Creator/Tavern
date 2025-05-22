using _Core._Global.ItemSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Core._Global.UI.Inventory
{
    public class StackSlotView : InventorySlotView
    {
        [SerializeField] private Image            _icon;
        [SerializeField] private TextMeshProUGUI  _qtyText;
        [SerializeField] private GameObject       _lockOverlay;

        protected override void OnBind(ItemState s)
        {
            var cfg = s.Config;
            _icon.sprite          = cfg.Icon;
            _icon.enabled         = true;
            _lockOverlay.SetActive(false);

            _qtyText.text         = s.Quantity.ToString();
            _qtyText.enabled      = true;
        }
    }
}