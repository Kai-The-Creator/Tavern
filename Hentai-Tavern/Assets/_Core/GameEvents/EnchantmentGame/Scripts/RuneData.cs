using UnityEngine;

namespace _Core.GameEvents.Enchantment.Data
{
    [CreateAssetMenu(menuName = "GAME/Enchantment/Rune", fileName = "Rune_")]
    public class RuneData : ScriptableObject
    {
        public Sprite     icon;
        public AudioClip  sfx;
    }
}