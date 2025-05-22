using UnityEngine;

namespace _Core._Combat
{
    [CreateAssetMenu(fileName = "DamageBonusPassive", menuName = "Combat/Passive/Damage Bonus")]
    public class DamageBonusPassiveSO : PassiveAbilitySO
    {
        [Range(0f,1f)] public float PhysicalBonus;
        [Range(0f,1f)] public float MagicalBonus;

        public override int ModifyOutgoingPhysical(int damage)
        {
            return damage + Mathf.FloorToInt(damage * PhysicalBonus);
        }

        public override int ModifyOutgoingMagical(int damage)
        {
            return damage + Mathf.FloorToInt(damage * MagicalBonus);
        }
    }
}