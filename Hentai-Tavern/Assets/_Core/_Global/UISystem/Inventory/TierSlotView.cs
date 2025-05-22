using System.Collections.Generic;
using _Core._Global.ItemSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Core._Global.UI.Inventory
{
    public sealed class TierSlotView : InventorySlotView
    {
        [Header("Layout")]
        [SerializeField] private Image           _icon;
        [SerializeField] private TextMeshProUGUI _name;
        [SerializeField] private Transform       _cellsRoot;
        [SerializeField] private TierCellView    _cellPrefab;

        private readonly List<TierCellView> _cells = new();

        protected override void OnBind(ItemState st)
        {
            _icon.sprite = st.Config.Icon;
            _name.text   = st.Config.DisplayName;

            var tiered = (ITieredItemConfig)st.Config;
            var metas  = tiered.Metas;

            EnsureCellCount(metas.Count);

            for (int i = 0; i < metas.Count; i++)
            {
                var meta    = metas[i];
                bool opened = st.IsTierUnlocked(meta.Rarity);
                _cells[i].Bind(st, meta, opened);   // ← передаём родительский ItemState
            }
        }

        private void EnsureCellCount(int need)
        {
            while (_cells.Count < need)
                _cells.Add(Instantiate(_cellPrefab, _cellsRoot, false));
        }
    }
}