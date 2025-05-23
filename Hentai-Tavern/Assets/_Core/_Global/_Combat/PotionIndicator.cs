using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Core._Combat
{
    /// <summary>
    /// Displays remaining potion uses for the turn.
    /// </summary>
    public class PotionIndicator : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI text;
        private PotionController _controller;

        private void Awake()
        {
            _controller = GetComponentInParent<PotionController>();
            UpdateText();
        }

        public void UpdateText()
        {
            if (_controller != null && text != null)
            {
                text.text = _controller.RemainingUses.ToString();
                Debug.Log(_controller.RemainingUses.ToString());
            }
        }
    }
}
