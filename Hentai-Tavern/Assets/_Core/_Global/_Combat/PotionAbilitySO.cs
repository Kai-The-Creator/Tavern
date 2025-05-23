using UnityEngine;

namespace _Core._Combat
{
    [CreateAssetMenu(fileName = "PotionAbility", menuName = "Combat/Potion Ability")]
    public class PotionAbilitySO : AbilitySO
    {
        public int HealAmount;
        public int RestoreMana;
        public int RestoreStamina;
    }
}

