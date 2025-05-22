using _Core._Global.ItemSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Core._Global.UI.Inventory
{
    public sealed class UnlockableSlotView : InventorySlotView
    {
        [SerializeField] private Image            _icon;
        [SerializeField] private GameObject       _lockOverlay;
        [SerializeField] private TextMeshProUGUI  _nameText;

        protected override void OnBind(ItemState s)
        {
            var cfg        = s.Config;
            _icon.sprite   = cfg.Icon;
            _nameText.text = cfg.DisplayName;

            _lockOverlay.SetActive(!s.Unlocked);
        }
    }
}