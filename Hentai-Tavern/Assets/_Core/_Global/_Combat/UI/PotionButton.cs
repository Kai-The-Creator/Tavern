using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Core._Combat.UI
{
    /// <summary>
    /// Button for potion abilities shown in the HUD.
    /// </summary>
    public class PotionButton : MonoBehaviour
    {
        [SerializeField] private Button button;
        [SerializeField] private Image icon;
        [SerializeField] private TextMeshProUGUI label;

        public Button Button => button;

        public void Setup(PotionAbilitySO ability)
        {
            if (label) label.text = ability ? ability.name : string.Empty;
            if (icon) icon.sprite = ability ? ability.Icon : null;
        }
    }
}
