using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Core._Combat
{
    public class PlayerEntity : CombatEntity
    {
        public override UniTask<AbilitySO> SelectAbility()
        {
            // Placeholder for player input
            return UniTask.FromResult<AbilitySO>(null);
        }
    }
}