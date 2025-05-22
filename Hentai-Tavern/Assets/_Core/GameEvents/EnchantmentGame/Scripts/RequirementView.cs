using _Core._Global.ItemSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Core.GameEvents.Enchantment.UI
{
    public sealed class RequirementView : MonoBehaviour
    {
        [SerializeField] private Image    icon;
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private TMP_Text countText;

        public void Bind(MaterialConfig res, int need, int have)
        {
            icon.sprite      = res.Icon;
            nameText.text    = res.DisplayName;
            countText.text   = $"{have}/{need}";
            countText.color  = have < need ? Color.red : Color.white;
        }
    }
}