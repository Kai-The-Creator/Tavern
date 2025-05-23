using System.Collections.Generic;
using UnityEngine;

namespace _Core._Combat
{
    public enum TargetSelector
    {
        Self,
        SingleEnemy,
        AllEnemies
    }

    [System.Serializable]
    public class AbilityEffect
    {
        public int PhysicalDamage;
        public int MagicalDamage;
        public StatusEffectSO[] AppliedStatuses = System.Array.Empty<StatusEffectSO>();
    }

    [CreateAssetMenu(fileName = "Ability", menuName = "Combat/Ability")]
    public class AbilitySO : ScriptableObject
    {
        public int PhysicalDamage;
        public int MagicalDamage;
        public int CostMana;
        public int CostStamina;
        public int Cooldown;
        [Min(1)] public int MaxTargets = 1;
        public List<AbilityEffect> Effects = new List<AbilityEffect>();
        public TargetSelector Target;
    }
}