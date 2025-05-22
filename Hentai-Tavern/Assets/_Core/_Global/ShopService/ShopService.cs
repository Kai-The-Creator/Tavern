using System;
using System.Collections.Generic;
using System.Linq;
using _Core._Global.ItemSystem;
using _Core._Global.Services;
using _Core._Global.WalletSystem;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Core._Global.ShopService
{
    [DependsOn(typeof(IWalletService), typeof(IItemService))]
    public class ShopService : AService, IShopService
    {
        [Header("All shop configs")] [SerializeField]
        private List<ShopConfig> allShopConfigs;

        [Header("Price calculator")] [SerializeField]
        private ScriptableObject priceCalculatorAsset;

        public event Action<ShopUpdateInfo> OnShopUpdated;
        public event Action<string, int> OnStockChanged;
        public event Action<string, int> OnPurchased;

        private readonly Dictionary<string, ShopRuntimeData> _shops = new();

        public override async UniTask OnStart()
        {
            await UniTask.Yield();
            var calculator = priceCalculatorAsset as IPriceCalculator ?? new PriceCalculator();
            foreach (var cfg in allShopConfigs.Where(c => c != null))
                _shops[cfg.ShopId] = new ShopRuntimeData(cfg, calculator);
        }

        public IReadOnlyList<string> GetShopIds() => _shops.Keys.ToList();

        public IReadOnlyList<ShopRuntimeItem> GetItemsForShop(string shopId, ShopSortMode sort = ShopSortMode.None)
        {
            if (!_shops.TryGetValue(shopId, out var data))
                return Array.Empty<ShopRuntimeItem>();
            return sort == ShopSortMode.None
                ? data.Items
                : data.GetSorted(sort);
        }

        public bool TryBuyItem(string shopId, int idx, out string failReason)
        {
            failReason = null;
            if (!_shops.TryGetValue(shopId, out var data))
            {
                failReason = "Shop not found";
                return false;
            }

            if (idx < 0 || idx >= data.Items.Count)
            {
                failReason = "Invalid index";
                return false;
            }

            var item = data.Items[idx];
            if (!item.IsUnlimited && item.Remaining <= 0)
            {
                failReason = "Sold out";
                return false;
            }

            var wallet = GService.GetService<IWalletService>();
            if (wallet == null)
            {
                failReason = "Wallet unavailable";
                return false;
            }

            if (!wallet.Spend(item.FinalPrice))
            {
                failReason = $"Not enough {item.FinalPrice.CurrencyId}";
                return false;
            }

            var itemService = GService.GetService<IItemService>();
            item.GrantToPlayer(itemService);

            if (!item.IsUnlimited)
            {
                item.Remaining--;
                OnStockChanged?.Invoke(shopId, idx);
            }

            OnPurchased?.Invoke(shopId, idx);
            OnShopUpdated?.Invoke(new ShopUpdateInfo
            {
                ShopId = shopId,
                IsFullRefresh = false,
                ChangedIndices = new List<int> { idx }
            });
            return true;
        }

        public async UniTask<(bool success, string failReason)> TryBuyItemAsync(string shopId, int index)
        {
            var result = TryBuyItem(shopId, index, out var reason);
            return (result, reason);
        }

        public void RefreshShop(string shopId)
        {
            if (_shops.TryGetValue(shopId, out var data))
                data.ResetToDefault();
            OnShopUpdated?.Invoke(new ShopUpdateInfo { ShopId = shopId, IsFullRefresh = true });
        }

        public void RefreshAllShops()
        {
            foreach (var d in _shops.Values) d.ResetToDefault();
            OnShopUpdated?.Invoke(new ShopUpdateInfo { ShopId = null, IsFullRefresh = true });
        }
    }

    internal class ShopRuntimeData
    {
        public ShopConfig Config { get; }
        public List<ShopRuntimeItem> Items { get; } = new();

        private readonly IPriceCalculator _calc;

        private readonly Dictionary<ShopSortMode, List<ShopRuntimeItem>> _sortedCache
            = new Dictionary<ShopSortMode, List<ShopRuntimeItem>>();

        public ShopRuntimeData(ShopConfig cfg, IPriceCalculator calc)
        {
            Config = cfg;
            _calc = calc;
            ResetToDefault();
        }

        public void ResetToDefault()
        {
            Items.Clear();
            if (Config == null) return;
            foreach (var rec in Config.Items)
                Items.Add(new ShopRuntimeItem(rec, Config.PriceMarkup, _calc));
            _sortedCache.Clear();
        }

        public List<ShopRuntimeItem> GetSorted(ShopSortMode mode)
        {
            if (_sortedCache.TryGetValue(mode, out var cached))
                return cached;

            var copy = new List<ShopRuntimeItem>(Items);
            switch (mode)
            {
                case ShopSortMode.PriceAsc:
                    copy.Sort((a, b) => a.FinalPrice.Amount.CompareTo(b.FinalPrice.Amount));
                    break;
                case ShopSortMode.PriceDesc:
                    copy.Sort((a, b) => b.FinalPrice.Amount.CompareTo(a.FinalPrice.Amount));
                    break;
                case ShopSortMode.Alphabetical:
                    copy.Sort((a, b) => string.Compare(a.DisplayName, b.DisplayName, StringComparison.Ordinal));
                    break;
            }

            _sortedCache[mode] = copy;
            return copy;
        }
    }
}