using UnityEngine;

namespace _Core._Combat
{
    public enum PotionType
    {
        HealHP,
        RestoreMana,
        RestoreStamina
    }

    [System.Serializable]
    public class Potion
    {
        public PotionType Type;
        public int Amount;
    }
}
