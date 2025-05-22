using TMPro;
using UnityEngine;
using UnityEngine.UI;
using _Core._Global.ItemSystem;

namespace _Core.GameEvents.Forging.UI
{
    public sealed class ForgeRequirementView : MonoBehaviour
    {
        [SerializeField] private Image    icon;
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private TMP_Text countText;

        public void Bind(MaterialConfig material, int need, int have)
        {
            if (icon  && material.Icon)      icon.sprite  = material.Icon;
            if (nameText)                    nameText.text = material.DisplayName;
            if (countText)                   countText.text = $"{have}/{need}";
            countText.color = have < need ? Color.red : Color.white;
        }
    }
}