using _Core._Global.ShopService;
using UnityEngine;

namespace _Core._Global.WalletSystem
{
    public class PriceCalculator : IPriceCalculator
    {
        public Money GetFinalPrice(ShopRuntimeItem item, float shopMarkup)
        {
            int basePrice = item.ShopItem.BasePrice;
            float sum = basePrice * (1f + shopMarkup);
            int final = Mathf.RoundToInt(sum);

            string currency = item.ShopItem.CurrencyId;
            return new Money(currency, final);
        }
    }
}