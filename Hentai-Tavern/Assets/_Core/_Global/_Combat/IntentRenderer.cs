using UnityEngine;
using UnityEngine.UI;

namespace _Core._Combat
{
    public class IntentRenderer : MonoBehaviour
    {
        [SerializeField] private Text label;

        public void ShowIntent(AbilitySO ability)
        {
            if (label != null)
                label.text = ability != null ? ability.name : string.Empty;
        }
    }
}

