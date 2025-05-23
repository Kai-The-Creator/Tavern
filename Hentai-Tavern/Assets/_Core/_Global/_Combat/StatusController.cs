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
        private StatusIndicator _indicator;

        private void Awake()
        {
            _indicator = GetComponent<StatusIndicator>();
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
            _indicator?.SetStatus(effect.Type, true);
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
                        // just consume duration
                        break;
                }

                s.Remaining--;
                if (s.Remaining <= 0 || (s.Effect.Type == StatusType.Shield && s.ShieldLeft <= 0))
                {
                    _statuses.RemoveAt(i);
                    _indicator?.SetStatus(s.Effect.Type, false);
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
            _indicator?.SetStatus(type, _statuses.Any(s => s.Effect.Type == type));
        }
    }
}
