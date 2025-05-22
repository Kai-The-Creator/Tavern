using UnityEngine;

namespace _Core._Combat
{
    [CreateAssetMenu(fileName = "BasicAttack", menuName = "Combat/Ability/Basic Attack")]
    public class BasicAttackAbilitySO : AbilitySO
    {
        private void Reset()
        {
            Target = TargetSelector.SingleEnemy;
            PhysicalDamage = 10;
        }
    }
}