using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Core._Combat
{
    public static class AbilityExecutor
    {
        public static async UniTask Execute(ICombatEntity source, ICombatEntity target, AbilitySO ability)
        {
            if (ability == null) return;

            if (ability.PhysicalDamage > 0)
                target.Resources.Health -= ability.PhysicalDamage;
            if (ability.MagicalDamage > 0)
                target.Resources.Mana -= ability.MagicalDamage;

            target.Resources.Clamp(target.Stats);
            await UniTask.Yield();
        }
    }
}
