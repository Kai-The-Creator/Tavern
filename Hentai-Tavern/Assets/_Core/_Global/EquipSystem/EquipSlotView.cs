﻿using _Core._Global.Equip;
using _Core._Global.ItemSystem;
using _Core._Global.Services;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace _Core._Global.UI.Inventory
{
    /// <summary>Одна ячейка панели экипировки.</summary>
    public sealed class EquipSlotView : MonoBehaviour, IPointerClickHandler
    {
        [Header("Setup")]
        [SerializeField] private EquipGroup _group;
        [SerializeField] private int        _index;        // 0-n внутри группы
        [SerializeField] private Sprite     _placeholder;

        [Header("UI")]
        [SerializeField] private Image _icon;
        [SerializeField] private Image _frame;

        private IEquipService _equipSvc;

        public EquipGroup Group => _group;
        public int        Index => _index;

        private void Awake() => _equipSvc = GService.GetService<IEquipService>();

        /// <summary>Вызывается панелью при любом изменении экипировки.</summary>
        public void UpdateContent(EquippedEntry? entry)
        {
            if (entry is not { } e)
            {
                _icon.sprite  = _placeholder;
                _icon.enabled = true;
                _frame.color  = Color.white;            // пустая рамка
                return;
            }

            // ─── Иконка и цвет рамки ─────────────────────────────────
            if (e.State.Config is ITieredItemConfig tc)
            {
                var meta = tc.Metas[(int)e.Tier];
                _icon.sprite = meta.Icon ? meta.Icon : e.State.Config.Icon;
                _frame.color = meta.FrameColor;
            }
            else
            {
                _icon.sprite = e.State.Config.Icon;
                _frame.color = RarityColor(e.Tier);
            }

            _icon.enabled = true;
        }

        public void OnPointerClick(PointerEventData _)
        {
            if (_equipSvc.Get(_group, _index) != null)
                _equipSvc.TryUnequip(_group, _index);   // снятие при клике
        }

        private static Color RarityColor(Rarity r) => r switch
        {
            Rarity.Uncommon  => new(0.18f,0.46f,0.23f),
            Rarity.Rare      => new(0.25f,0.4f ,0.65f),
            Rarity.Epic      => new(0.45f,0.18f,0.48f),
            Rarity.Legendary => new(0.65f,0.45f,0.05f),
            _                => Color.white
        };
    }
}
