using UnityEngine;

namespace _Core._Combat
{
    [CreateAssetMenu(fileName = "BattleConfig", menuName = "Combat/Battle Config")]
    public class BattleConfig : ScriptableObject
    {
        [Range(0,1)] public float ManaRegenPercentPlayer = 0.10f;
        [Range(0,1)] public float StaminaRegenPercentPlayer = 0.10f;
        [Range(0,1)] public float ManaRegenPercentEnemy = 0.10f;
        [Range(0,1)] public float StaminaRegenPercentEnemy = 0.10f;

    [Range(0,100)] public float UltChargePerAttack = 5f;
    public AbilitySO UltimateAbility;

    [Min(0)] public int PotionsPerTurn = 1;

        public float GetManaRegenPercent(bool isPlayer) =>
            isPlayer ? ManaRegenPercentPlayer : ManaRegenPercentEnemy;

        public float GetStaminaRegenPercent(bool isPlayer) =>
            isPlayer ? StaminaRegenPercentPlayer : StaminaRegenPercentEnemy;
    }
}
