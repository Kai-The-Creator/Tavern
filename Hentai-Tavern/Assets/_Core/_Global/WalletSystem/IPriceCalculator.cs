using _Core._Global.ShopService;

namespace _Core._Global.WalletSystem
{
    public interface IPriceCalculator
    {
        Money GetFinalPrice(ShopRuntimeItem item, float shopMarkup);
    }
}