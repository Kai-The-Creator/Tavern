using UnityEngine;
using UnityEngine.UI;

namespace _Core._Combat.UI
{
    /// <summary>
    /// Visual icon for a status effect.
    /// </summary>
    public class StatusIcon : MonoBehaviour
    {
        [SerializeField] private Image icon;

        public void SetIcon(Sprite sprite)
        {
            if (icon) icon.sprite = sprite;
        }
    }
}
