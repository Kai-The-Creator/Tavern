using System.Collections.Generic;
using _Core._Global.ItemSystem;
using UnityEngine;

namespace _Core._Global.ShopService
{
    [CreateAssetMenu(fileName = "NewShopItemConfig", menuName = "GAME/Shop/ShopItem")]
    public class ShopItemConfig : ScriptableObject
    {
        [SerializeField] private string displayName;
        [SerializeField] private Sprite icon;

        [TextArea] [SerializeField] private string description;

        [Header("Base Price (ignored money, but let's keep it)")] [SerializeField]
        private int basePrice;

        [SerializeField] private string currencyId = "Gold";


        [Header("If more than 1 => this is a bundle")] [SerializeField]
        private List<ShopBundleItem> bundleItems;

        public string DisplayName => displayName;
        public Sprite Icon => icon;
        public string Description => description;
        public int BasePrice => basePrice;
        public string CurrencyId => currencyId;
        public IReadOnlyList<ShopBundleItem> BundleItems => bundleItems;
    }

    [System.Serializable]
    public class ShopBundleItem
    {
        public ItemConfig itemConfig;
        public int quantity = 1;
    }
}