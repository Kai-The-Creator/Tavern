// WorkstationTag.cs

using UnityEngine;

namespace _Core._Global.ItemSystem
{
    /// <summary>Marker-asset that represents a craft station (Forge, Magic Shop, Kitchen …).</summary>
    [CreateAssetMenu(menuName = "GAME/Crafting/WorkstationTag", fileName = "Tag_")]
    public class WorkstationTag : ScriptableObject
    {
        [Tooltip("Unique textual ID (used for Addressables or debug).")]
        [SerializeField] private string id;

        [Tooltip("Human-readable name shown in editors / logs.")]
        [SerializeField] private string displayName;

        public string Id          => id;
        public string DisplayName => displayName;
    }
}