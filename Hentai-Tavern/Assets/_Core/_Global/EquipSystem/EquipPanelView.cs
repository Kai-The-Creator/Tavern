// Assets/_Core/_Global/UI/Inventory/EquipPanelView.cs
using System.Collections.Generic;
using _Core._Global.Equip;
using _Core._Global.Services;
using UnityEngine;

namespace _Core._Global.UI.Inventory
{
    /// <summary>Панель из всех ячеек экипировки внутри InventoryWindow.</summary>
    public sealed class EquipPanelView : MonoBehaviour
    {
        [SerializeField] private List<EquipSlotView> _slots;

        private IEquipService _equip;

        private void Awake()
        {
            _equip = GService.GetService<IEquipService>();
            _equip.OnChanged += HandleChanged;
            RefreshAll();
        }

        private void OnDestroy() => _equip.OnChanged -= HandleChanged;

        private void HandleChanged(EquipGroup g, int idx,
            EquippedEntry? oldE, EquippedEntry? newE)
        {
            foreach (var s in _slots)
                if (s.gameObject.activeInHierarchy &&
                    s.Group == g && s.Index == idx)
                {
                    s.UpdateContent(newE);
                    break;
                }
        }

        private void RefreshAll()
        {
            foreach (var s in _slots)
                s.UpdateContent(_equip.Get(s.Group, s.Index));
        }
    }
}