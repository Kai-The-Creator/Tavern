using _Core._Global.ItemSystem;
using _Core._Global.WalletSystem;
using UnityEngine;

namespace _Core._Global.ShopService
{
    public class ShopRuntimeItem
    {
        private readonly ShopRecord _record;
        private readonly float _shopMarkup;
        private readonly IPriceCalculator _priceCalc;
        public Rarity BundleRarity { get; private set; } = Rarity.Common;

        public ShopRuntimeItem(ShopRecord record, float shopMarkup, IPriceCalculator priceCalc)
        {
            _record = record;
            _shopMarkup = shopMarkup;
            _priceCalc = priceCalc;

            Remaining = record.startQuantity;
            IsUnlimited = record.unlimited;

            RecalculatePrice();
            CalcBundleRarity();
        }

        private void CalcBundleRarity()
        {
            Rarity max = Rarity.Common;

            foreach (var b in ShopItem.BundleItems)
            {
                if (b.itemConfig is ItemConfig gCfg)
                    max = (Rarity)Mathf.Max((int)max, (int)gCfg.BaseRarity);
            }

            BundleRarity = max;
        }


        public ShopItemConfig ShopItem => _record.shopItem;
        public bool IsBundle => ShopItem && ShopItem.BundleItems.Count > 1;
        public bool IsUnlimited { get; }
        public int Remaining { get; set; }
        public Money FinalPrice { get; private set; }

        public string DisplayName => ShopItem ? ShopItem.DisplayName : "Unknown";

        public void RecalculatePrice() => FinalPrice = _priceCalc.GetFinalPrice(this, _shopMarkup);

        public void GrantToPlayer(IItemService itemService)
        {
            foreach (var b in ShopItem.BundleItems)
            {
                var cfg = b.itemConfig;
                if (cfg == null) continue;

                // ― ресурсы (Material / Ingredient) стекаются → прибавляем quantity
                if (cfg.Stackable)
                {
                    itemService.TryAdd(cfg, b.quantity);
                }
                // ― всё, что не стекается (оружие, броня, рецепты…) → просто открываем
                else
                {
                    if (!itemService.IsUnlocked(cfg))
                        itemService.TryUnlock(cfg);
                }
            }
        }
    }
}