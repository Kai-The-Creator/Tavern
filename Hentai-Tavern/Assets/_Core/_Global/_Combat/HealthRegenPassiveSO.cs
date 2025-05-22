using UnityEngine;

namespace _Core._Combat
{
    [CreateAssetMenu(fileName = "HealthRegenPassive", menuName = "Combat/Passive/Health Regen")]
    public class HealthRegenPassiveSO : PassiveAbilitySO
    {
        [Range(0f,1f)] public float RegenPercent = 0.05f;

        public override void OnTurnStart(CombatEntity entity)
        {
            entity.Resources.Health += Mathf.FloorToInt(entity.Stats.MaxHealth * RegenPercent);
            entity.Resources.Clamp(entity.Stats);
        }
    }
}