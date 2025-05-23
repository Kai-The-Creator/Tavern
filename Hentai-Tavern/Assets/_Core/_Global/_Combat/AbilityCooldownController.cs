using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace _Core._Combat
{
    /// <summary>
    /// Tracks cooldowns for abilities on a CombatEntity.
    /// </summary>
    public class AbilityCooldownController : MonoBehaviour
    {
        private readonly Dictionary<AbilitySO, int> _cooldowns = new();

        /// <summary>
        /// Checks if an ability is currently on cooldown.
        /// </summary>
        public bool IsOnCooldown(AbilitySO ability)
        {
            return ability != null && _cooldowns.TryGetValue(ability, out var turns) && turns > 0;
        }

        /// <summary>
        /// Returns remaining cooldown turns for the ability.
        /// </summary>
        public int GetRemaining(AbilitySO ability)
        {
            return ability != null && _cooldowns.TryGetValue(ability, out var turns) ? turns : 0;
        }

        /// <summary>
        /// Starts cooldown for specified ability.
        /// </summary>
        public void StartCooldown(AbilitySO ability)
        {
            if (ability == null) return;
            if (ability.Cooldown <= 0) return;
            _cooldowns[ability] = ability.Cooldown;
        }

        /// <summary>
        /// Decrements all cooldown counters, removing expired ones.
        /// </summary>
        public void Tick()
        {
            var keys = _cooldowns.Keys.ToList();
            foreach (var ability in keys)
            {
                _cooldowns[ability]--;
                if (_cooldowns[ability] <= 0)
                    _cooldowns.Remove(ability);
            }
        }
    }
}
