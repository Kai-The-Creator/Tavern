using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace _Core._Combat
{
    public class StatusController : MonoBehaviour
    {
        private class ActiveStatus
        {
            public StatusEffectSO Effect;
            public int Remaining;
            public int ShieldLeft;
        }

        private readonly List<ActiveStatus> _statuses = new();
        [SerializeField] private StatusIndicator indicator;
        public StatusIndicator Indicator => indicator;

        /// <summary>
        /// The indicator used to display active statuses.
        /// </summary>
        public StatusIndicator Indicator => _indicator;

        /// <summary>
        /// The indicator used to display active statuses.
        /// </summary>
        public StatusIndicator Indicator => _indicator;

        /// <summary>
        /// When true, the owning entity should skip its upcoming turn.
        /// This flag is set when a <see cref="StatusType.Stun"/> is applied
        /// and consumed by <see cref="CombatService"/> after the turn is skipped.
        /// </summary>
        public bool SkipNextTurn { get; set; }

        private void Awake()
        {
            if (_indicator == null)
                _indicator = GetComponent<StatusIndicator>();
        }

        /// <summary>
        /// Sets the indicator used to display statuses.
        /// </summary>
        public void SetIndicator(StatusIndicator indicator)
        {
            _indicator = indicator;
        }


        public bool IsStunned => _statuses.Any(s => s.Effect.Type == StatusType.Stun);

        public void Apply(StatusEffectSO effect)
        {
            var existing = _statuses.FirstOrDefault(s => s.Effect.Type == effect.Type);
            if (existing != null)
            {
                existing.Remaining = Mathf.Max(existing.Remaining, effect.Duration);
                if (effect.Type == StatusType.Shield)
                    existing.ShieldLeft = Mathf.Max(existing.ShieldLeft, effect.Value);
            }
            else
            {
                _statuses.Add(new ActiveStatus
                {
                    Effect = effect,
                    Remaining = effect.Duration,
                    ShieldLeft = effect.Value
                });
            }
            if (effect.Type == StatusType.Stun)
            {
                SkipNextTurn = true;
            }
            indicator?.AddStatus(effect);
        }

        public void TickStartTurn(CombatEntity entity)
        {
            for (int i = _statuses.Count - 1; i >= 0; i--)
            {
                var s = _statuses[i];
                switch (s.Effect.Type)
                {
                    case StatusType.Poison:
                        entity.Resources.Health -= s.Effect.Value;
                        break;
                    case StatusType.Regen:
                        entity.Resources.Health += s.Effect.Value;
                        break;
                    case StatusType.Stun:
                        // Stun effects simply reduce their duration here.
                        // SkipNextTurn is handled separately until the combat
                        // loop consumes it.
                        break;
                }

                s.Remaining--;
                if (s.Remaining <= 0 || (s.Effect.Type == StatusType.Shield && s.ShieldLeft <= 0))
                {
                    _statuses.RemoveAt(i);
                    indicator?.RemoveStatus(s.Effect.Type);
                }
            }
            entity.Resources.Clamp(entity.Stats);
        }

        public int AbsorbShieldDamage(int damage)
        {
            var shield = _statuses.FirstOrDefault(s => s.Effect.Type == StatusType.Shield);
            if (shield == null) return damage;
            var absorbed = Mathf.Min(shield.ShieldLeft, damage);
            shield.ShieldLeft -= absorbed;
            if (shield.ShieldLeft <= 0) shield.Remaining = 0;
            return damage - absorbed;
        }

        public void RemoveStatus(StatusType type)
        {
            for (int i = _statuses.Count - 1; i >= 0; i--)
            {
                if (_statuses[i].Effect.Type == type)
                    _statuses.RemoveAt(i);
            }
            if (!_statuses.Any(s => s.Effect.Type == type))
                indicator?.RemoveStatus(type);
        }
    }
}
