using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Core._Combat.UI
{
    /// <summary>
    /// Visual representation for an ability choice.
    /// Displays icon, name and cooldown state.
    /// </summary>
    public class AbilityButton : MonoBehaviour
    {
        [SerializeField] private Button button;
        [SerializeField] private Image icon;
        [SerializeField] private TextMeshProUGUI label;
        [SerializeField] private TextMeshProUGUI cooldownLabel;

        public Button Button => button;

        public void Setup(AbilitySO ability, int cooldown)
        {
            if (label) label.text = ability ? ability.name : string.Empty;
            if (icon) icon.sprite = ability ? ability.Icon : null;
            if (cooldownLabel)
            {
                cooldownLabel.gameObject.SetActive(cooldown > 0);
                if (cooldown > 0) cooldownLabel.text = cooldown.ToString();
            }
            if (button) button.interactable = cooldown <= 0;
        }
    }
}
