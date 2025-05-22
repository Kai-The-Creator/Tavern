using System.Linq;
using _Core._Global.ShopService;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Core._Global.UISystem.ItemsView
{
    public class ShopItemView : BaseItemView<ShopItemContext>
    {
        [Header("UI Refs")] [SerializeField] private Image iconImage;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI priceText;
        [SerializeField] private TextMeshProUGUI stockText;
        [SerializeField] private TextMeshProUGUI ownedText;
        [SerializeField] private Button buyButton;

        private ShopItemContext _ctx;

        public override void OnSpawn(in ShopItemContext ctx)
        {
            _ctx = ctx;

            var cfg = ctx.Item.ShopItem;
            iconImage.sprite = cfg.Icon;
            nameText.text = cfg.DisplayName;

            ctx.ShopService.OnShopUpdated += OnShopUpdated;
            ctx.ShopService.OnStockChanged += OnStockChanged;

            buyButton.onClick.RemoveAllListeners();
            buyButton.onClick.AddListener(OnBuyClicked);

            UpdateView();
            AnimateIn();
        }

        public override void OnDespawn()
        {
            _ctx.ShopService.OnShopUpdated -= OnShopUpdated;
            _ctx.ShopService.OnStockChanged -= OnStockChanged;
            buyButton.onClick.RemoveAllListeners();
            CurrentConfig = null;
        }

        public override void UpdateView()
        {
            var r = _ctx.Item;
            priceText.text = $"{r.FinalPrice.Amount} {r.FinalPrice.CurrencyId}";
            stockText.text = r.IsUnlimited ? "∞" : r.Remaining.ToString();

            bool owned = r.IsBundle
                ? r.ShopItem.BundleItems.Select(b => b.itemConfig)
                    .Any(c => _ctx.ItemService.IsUnlocked(c))
                : _ctx.ItemService.IsUnlocked(r.ShopItem.BundleItems[0].itemConfig);

            ownedText.gameObject.SetActive(owned);

            bool soldOut = !r.IsUnlimited && r.Remaining <= 0;
            buyButton.interactable = !owned && !soldOut;
        }

        private void OnShopUpdated(ShopUpdateInfo info)
        {
            if (info.ShopId == _ctx.ShopId) UpdateView();
        }

        private void OnStockChanged(string shopId, int idx)
        {
            if (shopId == _ctx.ShopId && idx == _ctx.Index) UpdateView();
        }

        private async void OnBuyClicked()
        {
            var (success, reason) = await _ctx.ShopService.TryBuyItemAsync(_ctx.ShopId, _ctx.Index);
            if (success)
            {
                UpdateView();
            }
            else
            {
                Debug.LogWarning($"Purchase failed: {reason}");
            }
        }
    }
}