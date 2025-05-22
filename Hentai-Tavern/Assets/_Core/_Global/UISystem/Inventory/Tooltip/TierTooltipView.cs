// Assets/_Core/_Global/UI/Tooltips/TierTooltipView.cs
using System.Collections.Generic;
using _Core._Global.Equip;          // IWearable, IEquipService
using _Core._Global.ItemSystem;
using _Core._Global.Services;
using _Core._Global.UI.Tooltips;
using UnityEngine;
using UnityEngine.UI;

namespace _Core._Global.UI
{
    public sealed class TierTooltipView : BaseTooltipView
    {
        [Header("Header")]
        [SerializeField] private Image header;

        [Header("Stars & Stats")]
        [SerializeField] private Transform  _starsRoot;
        [SerializeField] private GameObject _starPrefab;
        [SerializeField] private Transform  _statsRoot;
        [SerializeField] private GameObject _statRowPrefab;

        [Header("Action")]
        [SerializeField] private Button _equipBtn;          // ← кнопка «Надеть»

        /* runtime */
        private readonly List<GameObject> _rows = new();
        private ItemState _state;
        private Rarity    _tier;

        private void Awake() => _equipBtn.onClick.AddListener(OnEquip);

        protected override void BindSpecific(in TooltipData d)
        {
            _state = d.State;
            _tier  = d.Tier;

            // Header цвет
            if (d.RareColor != null)
                header.color = (Color)d.RareColor;

            // Звёзды
            SpawnStars((int)d.Rarity + 1);

            // Характеристики
            foreach (var g in _rows) Destroy(g);
            _rows.Clear();

            bool hasStats = d.Stats != null && d.Stats.Count > 0;
            _statsRoot.gameObject.SetActive(hasStats);
            if (hasStats)
            {
                foreach (var s in d.Stats)
                {
                    var row = Instantiate(_statRowPrefab, _statsRoot, false)
                              .GetComponent<StatRowView>();
                    row.Bind(s);
                    _rows.Add(row.gameObject);
                }
            }

            // Кнопка «Надеть» показывается только для IWearable
            bool wearable = _state?.Config is IWearable;
            _equipBtn.gameObject.SetActive(wearable);
        }

        private void OnEquip()
        {
            if (_state == null) return;

            var equipSvc = GService.GetService<IEquipService>();
            equipSvc.TryEquip(_state, _tier, out _, out _);

            // Закрыть тултип после экипировки
            TooltipHelper.Hide();
        }

        private void SpawnStars(int n)
        {
            for (int i = _starsRoot.childCount - 1; i >= 0; i--)
                Destroy(_starsRoot.GetChild(i).gameObject);
            for (int i = 0; i < n; i++)
                Instantiate(_starPrefab, _starsRoot, false);
        }
    }
}
