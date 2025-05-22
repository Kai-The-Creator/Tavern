using UnityEngine;

namespace _Core._Combat
{
    [CreateAssetMenu(fileName = "DamageResistancePassive", menuName = "Combat/Passive/Damage Resistance")]
    public class DamageResistancePassiveSO : PassiveAbilitySO
    {
        [Range(0f,1f)] public float PhysicalResist;
        [Range(0f,1f)] public float MagicalResist;

        public override int ModifyIncomingPhysical(int damage)
        {
            return damage - Mathf.FloorToInt(damage * PhysicalResist);
        }

        public override int ModifyIncomingMagical(int damage)
        {
            return damage - Mathf.FloorToInt(damage * MagicalResist);
        }
    }
}