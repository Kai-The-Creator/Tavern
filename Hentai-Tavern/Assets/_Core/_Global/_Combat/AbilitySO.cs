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

    public class AbilityEffect
    {
        public int PhysicalDamage;
        public int MagicalDamage;
    }

    [CreateAssetMenu(fileName = "Ability", menuName = "Combat/Ability")]
    public class AbilitySO : ScriptableObject
    {
        public int PhysicalDamage;
        public int MagicalDamage;
        public int CostMana;
        public int CostStamina;
        public int Cooldown;
        public List<AbilityEffect> Effects = new List<AbilityEffect>();
        public TargetSelector Target;
    }
}