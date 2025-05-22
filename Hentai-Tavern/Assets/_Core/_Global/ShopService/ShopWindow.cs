using System;
using System.Collections.Generic;
using System.Linq;
using _Core._Global.Categories.Filtering;
using _Core._Global.GConfig;
using _Core._Global.ItemSystem;
using _Core._Global.Services;
using _Core._Global.UISystem.Filtered;
using _Core._Global.UISystem.ItemsView;
using UnityEngine;

namespace _Core._Global.ShopService
{
    public struct ShopItemContext
    {
        public string ShopId;
        public int Index;
        public ShopRuntimeItem Item;
        public IShopService ShopService;
        public IItemService ItemService;
    }

    public class ShopWindow
        : BaseFilteredWindow<ShopRuntimeItem, ShopItemContext, ShopItemView>
    {
        [Header("Shop Settings")] [SerializeField]
        private string initialShopId;

        [SerializeField] private CategoryDefinition bundleCategory; // (2)

        private string _shopId;
        private IShopService _shopService;
        private IItemService _itemService;
        private IGlobalConfigService _gcfg;

        protected override void Awake()
        {
            base.Awake();
            if (itemPrefab == null)
                throw new InvalidOperationException($"{nameof(ShopWindow)}: itemPrefab not set");
            if (itemsContainer == null)
                throw new InvalidOperationException($"{nameof(ShopWindow)}: itemsContainer not set");
            if (bundleCategory == null)
                Debug.LogWarning($"{nameof(ShopWindow)}: bundleCategory is null – bundle-фильтр работать не будет");
        }

        protected override void InitializeServices()
        {
            _shopService = GService.GetService<IShopService>()
                           ?? throw new InvalidOperationException("ShopService not found");
            _itemService = GService.GetService<IItemService>();
            _gcfg = GService.GetService<IGlobalConfigService>();
            _shopId = initialShopId;
        }

        public void OpenShop(string shopId) => _shopId = shopId;

        protected override void SubscribeToDataEvents()
        {
            _shopService.OnShopUpdated += OnShopUpdated;
        }

        protected override void UnsubscribeFromDataEvents()
        {
            _shopService.OnShopUpdated -= OnShopUpdated;
        }

        private void OnShopUpdated(ShopUpdateInfo cfg)
        {
            RebuildUI();
        }

        protected override IEnumerable<ShopRuntimeItem> GetAllItems()
            => _shopService.GetItemsForShop(_shopId) ?? Enumerable.Empty<ShopRuntimeItem>();

        protected override ShopItemContext CreateContext(ShopRuntimeItem item)
        {
            var list = GetAllItems().ToList();
            int idx = list.IndexOf(item);

            return new ShopItemContext
            {
                ShopId = _shopId,
                Index = idx,
                Item = item,
                ShopService = _shopService,
                ItemService = _itemService,
            };
        }

        protected override IEnumerable<CategoryDefinition> GetCategories(ShopRuntimeItem item)
        {
            var cats = item.ShopItem.BundleItems.SelectMany(b => b.itemConfig.Categories);

            // влкючить категорию “Bundle”
            if (item.IsBundle && bundleCategory != null)
                cats = cats.Append(bundleCategory);

            return cats.Distinct();
        }

        protected override string GetSearchText(ShopRuntimeItem item)
            => item.DisplayName;

        protected override object GetSortKey(ShopRuntimeItem item)
            => item.FinalPrice.Amount;

        protected IEnumerable<(string label, Action action)> GetSortOptions()
        {
            yield return ("Price ↑", () => _presenter.SetSort(i => i.FinalPrice.Amount, true));
            yield return ("Price ↓", () => _presenter.SetSort(i => i.FinalPrice.Amount, false));
            yield return ("Name ↑", () => _presenter.SetSort(i => i.DisplayName, true));
            yield return ("Name ↓", () => _presenter.SetSort(i => i.DisplayName, false));
        }

        protected override List<CategoryNode> GetCategoryRoots()
            => _gcfg.Categories.RootNodes.ToList();

        protected override bool FilterPredicate(ShopRuntimeItem item)
            => true;
    }
}