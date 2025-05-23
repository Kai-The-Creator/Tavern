using System.Collections.Generic;
using UnityEngine;
using _Core._Combat.UI;

namespace _Core._Combat
{
    /// <summary>
    /// Spawns status icons for active effects.
    /// </summary>
    public class StatusIndicator : MonoBehaviour
    {
        [SerializeField] private Transform container;
        [SerializeField] private StatusIcon iconPrefab;

        private readonly Dictionary<StatusType, StatusIcon> _icons = new();
        private readonly List<StatusIcon> _passiveIcons = new();

        public void SetPassives(IEnumerable<PassiveAbilitySO> passives)
        {
            ClearPassives();
            if (passives == null || iconPrefab == null || container == null)
                return;

            foreach (var p in passives)
            {
                if (p == null || p.Icon == null) continue;
                var icon = Instantiate(iconPrefab, container);
                icon.SetIcon(p.Icon);
                _passiveIcons.Add(icon);
            }
        }

        private void ClearPassives()
        {
            foreach (var i in _passiveIcons)
                if (i) Destroy(i.gameObject);
            _passiveIcons.Clear();
        }

        public void AddStatus(StatusEffectSO effect)
        {
            if (effect == null || _icons.ContainsKey(effect.Type))
                return;
            if (iconPrefab == null || container == null)
                return;

            var icon = Instantiate(iconPrefab, container);
            icon.SetIcon(effect.Icon);
            _icons[effect.Type] = icon;
        }

        public void RemoveStatus(StatusType type)
        {
            if (_icons.TryGetValue(type, out var icon))
            {
                if (icon)
                    Destroy(icon.gameObject);
                _icons.Remove(type);
            }
        }
    }
}
