using System.Collections.Generic;
using UnityEngine;

namespace _Core._Global.ItemSystem
{
    [CreateAssetMenu(menuName = "GAME/Items/ItemCatalog", fileName = "ItemCatalog")]
    public class ItemCatalog : ScriptableObject
    {
        [Tooltip("All item ScriptableObjects referenced by the game.")]
        [SerializeField] private List<ItemConfig> _items = new();

        public IReadOnlyList<ItemConfig> Items => _items;
    }
}