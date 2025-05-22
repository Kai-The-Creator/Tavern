using UnityEngine;

namespace _Core._Combat
{
    [CreateAssetMenu(fileName = "AreaAttack", menuName = "Combat/Ability/Area Attack")]
    public class AreaAttackAbilitySO : AbilitySO
    {
        private void Reset()
        {
            Target = TargetSelector.AllEnemies;
            PhysicalDamage = 5;
        }
    }
}