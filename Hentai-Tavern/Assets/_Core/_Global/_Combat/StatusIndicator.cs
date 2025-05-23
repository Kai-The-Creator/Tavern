using System.Collections.Generic;
using UnityEngine;

namespace _Core._Combat
{
    /// <summary>
    /// Displays active status icons. This is a simple placeholder implementation.
    /// </summary>
    public class StatusIndicator : MonoBehaviour
    {
        [SerializeField] private GameObject poisonIcon;
        [SerializeField] private GameObject regenIcon;
        [SerializeField] private GameObject stunIcon;
        [SerializeField] private GameObject shieldIcon;

        private readonly Dictionary<StatusType, GameObject> _icons = new();

        private void Awake()
        {
            if (poisonIcon) poisonIcon.SetActive(false);
            if (regenIcon) regenIcon.SetActive(false);
            if (stunIcon) stunIcon.SetActive(false);
            if (shieldIcon) shieldIcon.SetActive(false);
            _icons[StatusType.Poison] = poisonIcon;
            _icons[StatusType.Regen] = regenIcon;
            _icons[StatusType.Stun] = stunIcon;
            _icons[StatusType.Shield] = shieldIcon;
        }

        public void SetStatus(StatusType type, bool active)
        {
            if (_icons.TryGetValue(type, out var go) && go != null)
                go.SetActive(active);
        }
    }
}
