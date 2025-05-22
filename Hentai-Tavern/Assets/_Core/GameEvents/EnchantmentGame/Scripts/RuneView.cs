using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using _Core.GameEvents.Enchantment.Data;

namespace _Core.GameEvents.Enchantment.UI
{
    public sealed class RuneView : MonoBehaviour,
                                   IPointerEnterHandler,
                                   IPointerExitHandler,
                                   IPointerDownHandler,
                                   IPointerUpHandler
    {
        [SerializeField] private Image       icon;
        [SerializeField] private Image       highlight;
        [SerializeField] private AudioSource sfx;

        private const float HoverA = .45f;
        private const float PressA = .85f;
        private const float HoverScale = 1.05f;
        private const float PressScale = 0.9f;
        private const string TweenId = "RuneTween";

        public  RuneData Rune { get; private set; }

        private Action<RuneView> clickCb;
        private bool interactable = true;

        void Awake()
        {
            highlight.color = new Color(1,1,1,0);
            highlight.gameObject.SetActive(true);
        }

        public void SetData(RuneData d) => (Rune, icon.sprite) = (d, d ? d.icon : null);
        public void SetCallback(Action<RuneView> cb) => clickCb = cb;

        public void SetInteractable(bool v)
        {
            interactable = v;
            KillTweens();
            highlight.color = new Color(1,1,1,0);
            transform.localScale = Vector3.one;
        }

        public async UniTask Flash(float time)
        {
            if (Rune?.sfx) sfx.PlayOneShot(Rune.sfx);
            KillTweens();

            var seq = DOTween.Sequence().SetId(TweenId);
            seq.Append(transform.DOScale(HoverScale, time*0.25f).SetEase(Ease.OutBack));
            seq.Join(highlight.DOFade(PressA, time*0.25f));
            seq.Append(transform.DOScale(1f, time*0.25f).SetEase(Ease.InBack));
            seq.Join(highlight.DOFade(0,  time*0.25f));
            await seq.ToUniTask();
        }

        public void OnPointerEnter(PointerEventData _){ if(interactable) HoverIn(); }
        public void OnPointerExit (PointerEventData _){ if(interactable) HoverOut(); }

        public void OnPointerDown(PointerEventData _)
        {
            if (!interactable) return;
            KillTweens();
            Press();
            if (Rune?.sfx) sfx.PlayOneShot(Rune.sfx);
            clickCb?.Invoke(this);
        }
        public void OnPointerUp(PointerEventData _) { if(interactable) HoverIn(); }

        private void HoverIn() =>
            TweenScaleAlpha(HoverScale, HoverA);
        private void HoverOut() =>
            TweenScaleAlpha(1f, 0f);
        private void Press() =>
            TweenScaleAlpha(PressScale, PressA);

        private void TweenScaleAlpha(float scale, float a)=>
            KillTweens()
              .Append(transform.DOScale(scale,.1f).SetEase(Ease.OutQuad))
              .Join(highlight.DOFade(a,.1f));

        private Sequence KillTweens()
        {
            DOTween.Kill(TweenId);
            var seq = DOTween.Sequence().SetId(TweenId);
            return seq;
        }
    }
}
