using UnityEngine;

namespace _Core._Combat
{
    [CreateAssetMenu(fileName = "PotionAbility", menuName = "Combat/Potion Ability")]
    public class PotionAbilitySO : AbilitySO
    {
        public ItemSystem.PotionConfig Config;
        [Min(0)] public int HealAmount;
        [Min(0)] public int ManaAmount;
        [Min(0)] public int StaminaAmount;
        public StatusType[] RemoveStatuses = System.Array.Empty<StatusType>();
    }
}
