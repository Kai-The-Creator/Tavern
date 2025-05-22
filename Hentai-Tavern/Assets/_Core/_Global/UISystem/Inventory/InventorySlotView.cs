using _Core._Global.ItemSystem;
using UnityEngine;

namespace _Core._Global.UI.Inventory
{
    /// <summary>Базовый Mono для любого визуального слота.</summary>
    public abstract class InventorySlotView : MonoBehaviour
    {
        public ItemState State { get; private set; }

        public void Bind(ItemState state)
        {
            State = state;
            OnBind(state);
        }

        protected abstract void OnBind(ItemState state);
    }
}