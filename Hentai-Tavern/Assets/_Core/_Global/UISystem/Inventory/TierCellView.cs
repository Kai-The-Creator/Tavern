using _Core._Global.ItemSystem;
using _Core._Global.Services;
using _Core._Global.UI.Tooltips;
using _Core._Global.UISystem;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace _Core._Global.UI.Inventory
{
    /// <summary>Визуал одной ячейки tier-а + обработка кликов.</summary>
    public sealed class TierCellView :
        MonoBehaviour,
        IPointerClickHandler, IPointerExitHandler
    {
        [Header("Refs")]
        [SerializeField] private Image      _background;
        [SerializeField] private Image      _icon;
        [SerializeField] private Transform  _starsRoot;
        [SerializeField] private Image      _lockOverlay;

        [Header("Assets")]
        [SerializeField] private GameObject _starPrefab;

        // ————— контекст
        private ItemState _parentState;
        private Rarity    _rarity;

        /*──────────── публичный Bind — вызывается из TierSlotView */
        public void Bind(ItemState parent, in TierMeta meta, bool unlocked)
        {
            _parentState = parent;
            _rarity      = meta.Rarity;

            _background.color = meta.FrameColor;
            _icon.sprite      = meta.Icon;

            SpawnStars((int)meta.Rarity + 1);
            _lockOverlay.gameObject.SetActive(!unlocked);
        }

        /*──────────── Pointer events */
        public void OnPointerClick(PointerEventData eventData)
        {
            var data = TooltipBuilder.FromTier(_parentState, _rarity);
            TooltipHelper.Show(data, (RectTransform)transform);
        }
        public void OnPointerExit(PointerEventData _)
        {
            // var ui = GService.GetService<IUIService>();
            // if (ui.IsWindowOpen(WindowType.InventoryTooltip))
            // {
            //     var tip = (TooltipWindow)ui.GetWindow(WindowType.InventoryTooltip);
            //     if (!tip.MouseOver)                           // курсор НЕ на тултипе
            //         TooltipHelper.Hide();
            // }
        }

        /*──────────── helpers */
        private void SpawnStars(int count)
        {
            for (int i = _starsRoot.childCount - 1; i >= 0; i--)
                Destroy(_starsRoot.GetChild(i).gameObject);
            for (int i = 0; i < count; i++)
                Instantiate(_starPrefab, _starsRoot, false);
        }
    }
}