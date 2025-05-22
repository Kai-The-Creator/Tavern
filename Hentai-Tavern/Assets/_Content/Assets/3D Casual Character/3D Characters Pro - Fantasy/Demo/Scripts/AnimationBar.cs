using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Layer_lab._3D_Casual_Character.Fantasy
{
    public class AnimationBar : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private TMP_Text textAnimation;

        public void Init()
        {
            textAnimation.text = UIControl.Instance.PanelAnimation.CurrentAnimationName;
            UIControl.Instance.PanelAnimation.OnChangeAnimation += OnChangeAnimation;
        }

        private void OnChangeAnimation(string animationName)
        {
            textAnimation.text = animationName;
        }
        
        public void OnClick_Left()
        {
            UIControl.Instance.PanelAnimation.PreviousAnimation();
        }

        public void OnClick_Right()
        {
            UIControl.Instance.PanelAnimation.NextAnimation();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            UIControl.Instance.PanelAnimation.Show();
        }
    }
}
