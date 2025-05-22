using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Core._Global.Categories
{
    public class CategoryFilterToggleView : MonoBehaviour
    {
        [SerializeField] private Toggle toggle;

        public Toggle Toggle
        {
            get => toggle;
            set => toggle = value;
        }

        [SerializeField] private TextMeshProUGUI title;

        public void Init(ToggleGroup toggleGroup, string label, bool isOnByDefault = false)
        {
            Debug.Log($"{label} - Init");
            if (toggleGroup) toggle.group = toggleGroup;
            if (!string.IsNullOrEmpty(label)) title.text = label;
        }
    }
}
