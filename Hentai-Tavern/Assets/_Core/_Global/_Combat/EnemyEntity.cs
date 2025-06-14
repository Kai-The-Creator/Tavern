using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Core._Combat
{
    public class EnemyEntity : CombatEntity
    {
        private BehaviourPatternSO behaviour;
        private int _index;

        public override UniTask<AbilitySO> SelectAbility()
        {
            if (behaviour == null || behaviour.Abilities.Count == 0)
                return UniTask.FromResult<AbilitySO>(null);

            for (int i = 0; i < behaviour.Abilities.Count; i++)
            {
                var ability = behaviour.Abilities[_index];
                _index = (_index + 1) % behaviour.Abilities.Count;
                if (!IsOnCooldown(ability) && CanUse(ability))
                    return UniTask.FromResult(ability);
            }

            return UniTask.FromResult<AbilitySO>(null);
        }

        public void SetBehaviour(BehaviourPatternSO pattern)
        {
            behaviour = pattern;
            _index = 0;
        }
    }
}