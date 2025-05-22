using System;
using System.Collections.Generic;
using _Core._Global.Services;
using Cysharp.Threading.Tasks;

namespace _Core._Global.ShopService
{
    public interface IShopService : IService
    {
        event Action<ShopUpdateInfo> OnShopUpdated;
        event Action<string, int> OnStockChanged;
        event Action<string, int> OnPurchased;

        IReadOnlyList<string> GetShopIds();
        IReadOnlyList<ShopRuntimeItem> GetItemsForShop(string shopId, ShopSortMode sortMode = ShopSortMode.None);

        bool TryBuyItem(string shopId, int index, out string failReason);
        UniTask<(bool success, string failReason)> TryBuyItemAsync(string shopId, int index);

        void RefreshShop(string shopId);
        void RefreshAllShops();
    }

    public class ShopUpdateInfo
    {
        public string ShopId;
        public bool IsFullRefresh;
        public List<int> ChangedIndices;
    }

    public enum ShopSortMode
    {
        None,
        PriceAsc,
        PriceDesc,
        Alphabetical
    }
}