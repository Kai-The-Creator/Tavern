using UnityEngine;

namespace _Core.GameEvents.Enchantment.Data
{
    [CreateAssetMenu(menuName = "GAME/Configs/EnchantmentGame", fileName = "EnchantmentGameConfig")]
    public class EnchantmentGameConfig : ScriptableObject
    {
        [Header("Sequence length (inclusive)")]
        [Min(1)] public int minSequenceLength = 3;
        [Min(1)] public int maxSequenceLength = 6;

        [Header("Max identical runes in a row")]
        [Min(1)] public int maxConsecutiveRepeat = 2;

        [Header("Rounds to win")]
        [Min(1)] public int roundsToWin = 3;

        [Header("Timing (seconds)")]
        [Range(.05f,5f)] public float runeShowTime = .6f;
        [Range(.05f,5f)] public float runePauseTime= .3f;
        [Min(.1f)]       public float inputTimeout = 5f;

        [Header("Lives")]
        public bool infiniteLives = false;
        [Min(1)]   public int  totalLives = 3;

        [Header("Reward")]
        [Min(0)]   public int duplicateGold = 50;

        [Header("Rune pool (layout order)")]
        public RuneData[] RunePool;
    }
}