using System.Collections.Generic;
using System.Linq;
using _Core._Global.Categories.Filtering;
using _Core._Global.ItemSystem;
using UnityEngine.UI;

namespace _Core._Global.UI.Inventory
{
    /// <summary>Связывает InventoryWindow c ItemService и вкладками-тоглами.</summary>
    public sealed class InventoryPresenter
    {
        private readonly InventoryWindow _view;
        private readonly IItemService    _items;          // фасад к предметам

        private readonly HashSet<CategoryDefinition> _activeCats = new();
        private InventoryTab.ViewKind                _activeView;

        public InventoryPresenter(InventoryWindow v, IItemService svc)
        {
            _view  = v;
            _items = svc;

            _items.OnChanged += (_, __) => Rebuild();    // авто-рефреш
        }

        // ───────────────────────── init
        public void Init(ToggleGroup group)
        {
            var tabs = group.GetComponentsInChildren<InventoryTab>(true);
            foreach (var tab in tabs)
            {
                tab.Toggle.onValueChanged.AddListener(val =>
                {
                    if (!val) return;

                    _activeCats.Clear();
                    _activeCats.UnionWith(tab.Categories);
                    _activeView = tab.ViewType;
                    Rebuild();
                });
            }
        }

        public void ShowFirstTab(ToggleGroup group)
        {
            var first = group.GetFirstActiveToggle()
                       ?? group.GetComponentsInChildren<Toggle>(true).First();
            first.isOn = true;                  // вызовет Rebuild() через listener
            first.Select();
            Rebuild();
        }

        // ───────────────────────── core
        private void Rebuild()
        {
            // 1) предметы нужных категорий
            var pool = _items.Query(HasActiveCategory);

            // 2) строгая фильтрация под текущий sub-view
            var filtered = pool.Where(IsCompatibleWithView).ToList();

            // 3) отдаём во View
            switch (_activeView)
            {
                case InventoryTab.ViewKind.Stack:
                    _view.ShowStack(filtered);
                    break;

                case InventoryTab.ViewKind.Tier:
                    _view.ShowTier(filtered);
                    break;

                default: /* Unlockable */
                    _view.ShowUnlockable(filtered);
                    break;
            }
        }

        // ───────────────────────── helpers
        private bool HasActiveCategory(ItemState s) =>
            s.Config.Categories.Any(_activeCats.Contains);

        private bool IsCompatibleWithView(ItemState s)
        {
            return _activeView switch
            {
                // Stack: только stackable c qty>0
                InventoryTab.ViewKind.Stack =>
                    s.Config.Stackable && s.Quantity > 0,

                // Tier: любой ITieredItemConfig с непустым списком Metas
                InventoryTab.ViewKind.Tier =>
                    s.Config is ITieredItemConfig { Metas: { Count: > 0 } },

                // Unlockable: non-stackable, без tier'ов, при этом
                // либо открыт, либо showWhenLocked = true
                InventoryTab.ViewKind.Unlockable =>
                    !s.Config.Stackable &&
                    s.Config is not ITieredItemConfig &&
                    (s.Unlocked || s.Config.ShowWhenLocked)
            };
        }
    }
}
