using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace _Core._Combat
{
    public static class AbilityExecutor
    {
        public static async UniTask Execute(ICombatEntity source, System.Collections.Generic.IEnumerable<ICombatEntity> targets, AbilitySO ability)
        {
            if (ability == null) return;

            if (source is CombatEntity src)
            {
                src.Resources.Mana -= ability.CostMana;
                src.Resources.Stamina -= ability.CostStamina;
                src.Resources.Clamp(src.Stats);
            }

            var list = targets.ToList();

            foreach (var target in list)
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

                    var status = tEntity.GetComponent<StatusController>();
                    if (status)
                        physical = status.AbsorbShieldDamage(physical);
                }

                if (physical > 0)
                    target.Resources.Health -= physical;
                if (magical > 0)
                    target.Resources.Mana -= magical;

                target.Resources.Clamp(target.Stats);
            }

            if (list.Count > 0)
            {
                var origin = source.Transform.position;
                var seq = DOTween.Sequence();
                seq.Append(source.Transform.DOMove(list[0].Transform.position, 0.25f));
                seq.AppendInterval(0.1f);
                seq.Append(source.Transform.DOMove(origin, 0.25f));
                await seq.AsyncWaitForCompletion();
            }

            await UniTask.Yield();
        }
    }
}