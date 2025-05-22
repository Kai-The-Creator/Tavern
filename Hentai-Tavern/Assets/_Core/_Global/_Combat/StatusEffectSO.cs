using UnityEngine;

namespace _Core._Combat
{
    public enum StatusType
    {
        Poison,
        Regen,
        Stun,
        Shield
    }

    [CreateAssetMenu(fileName = "StatusEffect", menuName = "Combat/Status Effect")]
    public class StatusEffectSO : ScriptableObject
    {
        public StatusType Type;
        public int Value;
        public int Duration;
    }
}