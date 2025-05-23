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

            if (source is CombatEntity src && ability is not PotionAbilitySO)
            {
                src.Resources.Mana -= ability.CostMana;
                src.Resources.Stamina -= ability.CostStamina;
                src.Resources.Clamp(src.Stats);
            }

            var list = targets.ToList();
            var effects = ability.Effects != null && ability.Effects.Count > 0
                ? ability.Effects
                : new System.Collections.Generic.List<AbilityEffect> { new AbilityEffect { PhysicalDamage = ability.PhysicalDamage, MagicalDamage = ability.MagicalDamage } };

            foreach (var target in list)
            {
                int heal = 0;
                int mana = 0;
                int stamina = 0;
                StatusType[] remove = System.Array.Empty<StatusType>();

                if (ability is PotionAbilitySO potion)
                {
                    heal = potion.HealAmount;
                    mana = potion.ManaAmount;
                    stamina = potion.StaminaAmount;
                    remove = potion.RemoveStatuses;
                }

                foreach (var effect in effects)
                {
                    int physical = effect.PhysicalDamage;
                    int magical = effect.MagicalDamage;

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

                        if (status && remove.Length > 0)
                        {
                            foreach (var r in remove)
                                status.RemoveStatus(r);
                        }
                    }

                    if (physical > 0)
                        target.Resources.Health -= physical;
                    if (magical > 0)
                        target.Resources.Mana -= magical;
                }

                if (heal > 0)
                    target.Resources.Health += heal;
                if (mana > 0)
                    target.Resources.Mana += mana;
                if (stamina > 0)
                    target.Resources.Stamina += stamina;

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