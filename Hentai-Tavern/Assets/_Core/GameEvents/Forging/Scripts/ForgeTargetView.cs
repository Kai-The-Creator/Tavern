using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace _Core.GameEvents.Forging.UI
{
    public sealed class ForgeTargetView : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private Image       circle;
        [SerializeField] private AudioSource sfx;

        private bool clicked;
        private UniTaskCompletionSource<bool> tcs;

        public RectTransform RectT { get; private set; }

        void Awake()   => RectT = GetComponent<RectTransform>();
        void OnEnable()
        {
            clicked = false;
            circle.color = new Color(1, 1, 1, 0);
        }
        void OnDisable() => tcs = null;

        /// <summary>Fade-in and prepare for interaction.</summary>
        public void Init(float fadeIn = .3f) =>
            circle.DOFade(1, fadeIn).SetEase(Ease.OutQuad);

        /// <returns>true if clicked before timeout, false otherwise.</returns>
        public async UniTask<bool> WaitForClickOrTimeout(float lifeSec, CancellationToken tok)
        {
            tcs = new UniTaskCompletionSource<bool>();

            var clickTask = tcs.Task;
            var delayTask = UniTask.Delay(TimeSpan.FromSeconds(lifeSec), cancellationToken: tok);

            var (index, _) = await UniTask.WhenAny(clickTask, delayTask);
            return index;
        }

        public void OnPointerClick(PointerEventData _)
        {
            if (clicked) return;
            clicked = true;

            sfx?.Play();
            tcs?.TrySetResult(true);
        }

        /// <summary>Fast fade-out, called by spawner when despawning.</summary>
        public void QuickHide(float fade = .15f)
        {
            circle.DOFade(0, fade)
                  .SetEase(Ease.InQuad)
                  .OnComplete(() => gameObject.SetActive(false));
        }
    }
}
