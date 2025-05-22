using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace _Core.GameEvents
{
    public class GameStatusPanel : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private CanvasGroup panelGroup;
        [SerializeField] private TextMeshProUGUI statusText;

        [Header("Settings")]
        [Tooltip("Время, в течение которого панель остаётся видимой, не считая задержки.")]
        [SerializeField] private float defaultDisplayDuration = 1.5f;

        [Tooltip("Длительность анимации появления/исчезновения панели.")]
        [SerializeField] private float fadeDuration = 0.5f;

        [Tooltip("Учитывать ли timeScale при анимациях (false = игнорировать timeScale).")]
        [SerializeField] private bool ignoreTimeScale = false;

        private void Awake()
        {
            if (panelGroup != null)
            {
                panelGroup.alpha = 0f;
                panelGroup.interactable = false;
                panelGroup.blocksRaycasts = false;
            }
        }

        public async UniTask ShowStatusAsync(
            string message,
            float? displayDuration = null,
            float? delayBeforeHide = null,
            CancellationToken token = default)
        {
            if (panelGroup == null || statusText == null)
                return;

            panelGroup.DOKill();

            statusText.text = message;

            panelGroup.alpha = 0f;
            panelGroup.interactable = true;
            panelGroup.blocksRaycasts = true;

            try
            {
                await panelGroup
                    .DOFade(1f, fadeDuration)
                    .SetUpdate(ignoreTimeScale)
                    .ToUniTask(cancellationToken: token);

                float duration = displayDuration ?? defaultDisplayDuration;
                await UniTask.Delay((int)(duration * 1000), cancellationToken: token);

                float extraDelay = delayBeforeHide ?? 0f;
                if (extraDelay > 0f)
                {
                    await UniTask.Delay((int)(extraDelay * 1000), cancellationToken: token);
                }

                await panelGroup
                    .DOFade(0f, fadeDuration)
                    .SetUpdate(ignoreTimeScale)
                    .ToUniTask(cancellationToken: token);
            }
            catch (OperationCanceledException) { }
            finally
            {
                panelGroup.alpha = 0f;
                panelGroup.interactable = false;
                panelGroup.blocksRaycasts = false;
            }
        }
    }
}
