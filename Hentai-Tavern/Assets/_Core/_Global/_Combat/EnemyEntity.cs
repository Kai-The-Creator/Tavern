using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Core._Combat
{
    public class EnemyEntity : CombatEntity
    {
        [SerializeField] private BehaviourPatternSO behaviour;
        [SerializeField] private IntentRenderer intent;
        private int _index;

        public override UniTask OnTurnStart(BattleConfig config)
        {
            if (behaviour != null && behaviour.Abilities.Count > 0 && intent != null)
            {
                var next = behaviour.Abilities[_index];
                intent.ShowIntent(next);
            }
            return base.OnTurnStart(config);
        }

        public override UniTask<AbilitySO> SelectAbility()
        {
            if (behaviour == null || behaviour.Abilities.Count == 0)
                return UniTask.FromResult<AbilitySO>(null);
            for (int i = 0; i < behaviour.Abilities.Count; i++)
            {
                var idx = (_index + i) % behaviour.Abilities.Count;
                var ab = behaviour.Abilities[idx];
                if (!Cooldowns.TryGetValue(ab, out var cd) || cd <= 0)
                {
                    _index = (idx + 1) % behaviour.Abilities.Count;
                    return UniTask.FromResult(ab);
                }
            }
            return UniTask.FromResult<AbilitySO>(null);
        }
    }
}