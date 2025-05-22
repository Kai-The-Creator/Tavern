using System.Collections.Generic;
using UnityEngine;

namespace _Core._Global.ShopService
{
    [CreateAssetMenu(fileName = "NewShopConfig", menuName = "GAME/Shop/ShopConfig")]
    public class ShopConfig : ScriptableObject
    {
        [SerializeField] private string shopId;
        [SerializeField] private float priceMarkup;
        [SerializeField] private List<ShopRecord> items;

        public string ShopId => shopId;
        public float PriceMarkup => priceMarkup;
        public IReadOnlyList<ShopRecord> Items => items;
    }

    [System.Serializable]
    public class ShopRecord
    {
        public ShopItemConfig shopItem;
        public int startQuantity = 5;
        public bool unlimited = false;
    }
}