using _Core._Global.Categories.Filtering;
using UnityEngine;

namespace _Core._Global.ItemSystem
{
    public enum Rarity { Common, Uncommon, Rare, Epic, Legendary }

    /// <summary>Base ScriptableObject for every collectable / resource item.</summary>
    public abstract class ItemConfig : ScriptableObject
    {
        [Header("Identification")]
        [Tooltip("Unique item ID (Addressables key or GUID string).")]
        [SerializeField] private string _id;

        [Header("Presentation")]
        [Tooltip("Item name shown to player (localized in UI layer).")]
        [SerializeField] private string _displayName;
        [SerializeField, TextArea(4, 10)] private string _description;

        [Tooltip("Icon displayed in inventory / UI.")]
        [SerializeField] private Sprite _icon;

        [Header("Meta")]
        [SerializeField] private Rarity _baseRarity = Rarity.Common;

        [Tooltip("Category tags used for filtering in UI & logic.")]
        [SerializeField] private CategoryDefinition[] _categories;
        
        [Header("Start grant")]
        [Tooltip("If true – item is unlocked at the very first launch, before any saves exist.")]
        [SerializeField] private bool startUnlocked = false;
        
        [SerializeField] private bool _showWhenLocked = true;
        
        [Tooltip("Positive number = starting stack. Works only for stackable resources.")]
        [Min(0)][SerializeField] private int startQuantity = 0;
        
        public bool StartUnlocked  => startUnlocked;
        public int  StartQuantity  => startQuantity;

        // ───────────── public API
        public  bool ShowWhenLocked     => _showWhenLocked;
        public string               Id          => _id;
        public string               DisplayName => _displayName;
        public string               Description => _description;
        public Sprite               Icon        => _icon;
        public Rarity               BaseRarity  => _baseRarity;
        public CategoryDefinition[] Categories  => _categories;

        /// <summary>True if item can stack (quantity > 1).</summary>
        public virtual bool Stackable   => true;
    }
}