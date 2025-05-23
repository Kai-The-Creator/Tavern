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
