using System;
using UnityEngine;

namespace Layer_lab._3D_Casual_Character.Fantasy
{
    public class PanelAnimation : MonoBehaviour
    {
        [SerializeField] private AnimationClip[] animationClip;
        [SerializeField] private ButtonAnimation buttonAnimation;
        [SerializeField] private Transform buttonParent;
        private bool _isShow;
        private int CurrentAnimationIndex { get; set; }
        public Action<string> OnChangeAnimation { get; set; }
        
        
        public string CurrentAnimationName => animationClip[CurrentAnimationIndex].name;
        
        public void Init()
        {
            for (var i = 0; i < animationClip.Length; i++)
            {
                var button = Instantiate(buttonAnimation, buttonParent);
                button.SetButton(i, animationClip[i]);
            }
    
            buttonAnimation.gameObject.SetActive(false);
        }

        public void PlayAnimationByIndex(int index)
        {
            CurrentAnimationIndex = index;
            ChangeAnimation();
            Close();
        }

        public void PreviousAnimation()
        {
            if (CurrentAnimationIndex < 0)
            {
                CurrentAnimationIndex = animationClip.Length - 1;
            }
            else
            {
                CurrentAnimationIndex--;
            }
            
            ChangeAnimation();
        }
        
        public void NextAnimation()
        {
            if (CurrentAnimationIndex >= animationClip.Length - 1)
            {   
                CurrentAnimationIndex = 0;
            }
            else
            {
                CurrentAnimationIndex++;
            }

            ChangeAnimation();
        }

        private void ChangeAnimation()
        {
            OnChangeAnimation.Invoke(animationClip[CurrentAnimationIndex].name);
            Character.Instance.PlayAnimation(animationClip[CurrentAnimationIndex]);
        }
        
        private void SetActive()
        {
            if (!_isShow)
            {
                _isShow = true;
                gameObject.SetActive(true);
            }
            else
            {
                _isShow = false;
                gameObject.SetActive(false);
            }
        }
        
        public void Show()
        {
            _isShow = true;
            gameObject.SetActive(true);
        }
        
        public void Close()
        {
            _isShow = false;
            gameObject.SetActive(false);
        }
    }
}

