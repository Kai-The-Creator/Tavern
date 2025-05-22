// ResourceConfig.cs
// ─────────────────────────────────────────────────────────────────────────

using UnityEngine;

namespace _Core._Global.ItemSystem
{
    /// <summary>Base class for any stackable resource (currency, material, ingredient).</summary>
    public abstract class ResourceConfig : ItemConfig
    {
        [Header("Resource")]
        [Tooltip("Vendor price in base currency.")]
        [Min(0)][SerializeField] private int _sellPrice = 1;

        public override bool Stackable   => true;
        public int SellPrice => _sellPrice;
    }
}