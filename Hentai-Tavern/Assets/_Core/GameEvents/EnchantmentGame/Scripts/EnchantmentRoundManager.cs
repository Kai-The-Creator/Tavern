using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using _Core.GameEvents.Enchantment.Data;

namespace _Core.GameEvents.Enchantment.Scripts
{
    /// <summary>Создаёт случайные последовательности рун (len∈[min,max],  
    /// одна и та же руна ≤ maxRepeat подряд).</summary>
    public sealed class EnchantmentRoundManager
    {
        private readonly List<RuneData> pool;
        private readonly List<RuneData> seq = new();
        private readonly int minLen;
        private readonly int maxLen;
        private readonly int maxRepeat;

        public EnchantmentRoundManager(RuneData[] srcPool,
            int minLen,
            int maxLen,
            int maxRepeat)
        {
            pool        = new List<RuneData>(srcPool);
            this.minLen = Mathf.Max(1, minLen);
            this.maxLen = Mathf.Max(this.minLen, maxLen);
            this.maxRepeat = Mathf.Max(1, maxRepeat);

            NextSequence();
        }

        public IReadOnlyList<RuneData> Sequence => seq;

        public void NextSequence() => Generate(Random.Range(minLen, maxLen+1));

        public bool Check(IReadOnlyList<RuneData> user)
        {
            if (user.Count != seq.Count) return false;
            for (int i = 0; i < user.Count; i++)
                if (user[i] != seq[i]) return false;
            return true;
        }

        private void Generate(int len)
        {
            seq.Clear();
            while (seq.Count < len)
            {
                var cand = pool[Random.Range(0, pool.Count)];

                int rep=0;
                for (int i=seq.Count-1; i>=0 && seq[i]==cand; i--) rep++;
                if (rep >= maxRepeat) continue;

                seq.Add(cand);
            }
        }
    }
}