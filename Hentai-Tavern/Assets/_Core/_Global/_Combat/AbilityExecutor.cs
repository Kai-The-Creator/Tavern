using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Core._Combat
{
    public static class AbilityExecutor
    {
        public static async UniTask Execute(ICombatEntity source, System.Collections.Generic.IEnumerable<ICombatEntity> targets, AbilitySO ability)
        {
            if (ability == null) return;

            foreach (var target in targets)
            {
                int physical = ability.PhysicalDamage;
                int magical = ability.MagicalDamage;

                if (source is CombatEntity sEntity && sEntity.Passives != null)
                {
                    foreach (var p in sEntity.Passives)
                    {
                        physical = p.ModifyOutgoingPhysical(physical);
                        magical = p.ModifyOutgoingMagical(magical);
                    }
                }

                if (target is CombatEntity tEntity && tEntity.Passives != null)
                {
                    foreach (var p in tEntity.Passives)
                    {
                        physical = p.ModifyIncomingPhysical(physical);
                        magical = p.ModifyIncomingMagical(magical);
                    }
                }

                if (physical > 0)
                    target.Resources.Health -= physical;
                if (magical > 0)
                    target.Resources.Mana -= magical;

                target.Resources.Clamp(target.Stats);
            }

            await UniTask.Yield();
        }
    }
}