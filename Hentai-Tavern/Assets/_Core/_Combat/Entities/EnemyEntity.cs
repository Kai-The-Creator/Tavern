using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Core._Combat
{
    public class EnemyEntity : CombatEntity
    {
        [SerializeField] private BehaviourPatternSO behaviour;
        private int _index;

        public override UniTask<AbilitySO> SelectAbility()
        {
            if (behaviour == null || behaviour.Abilities.Count == 0)
                return UniTask.FromResult<AbilitySO>(null);

            var ability = behaviour.Abilities[_index];
            _index = (_index + 1) % behaviour.Abilities.Count;
            return UniTask.FromResult(ability);
        }
    }
}
