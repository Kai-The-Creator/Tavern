using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

/// <summary>
/// Базовый класс панели (UI) с поддержкой DOTween-анимаций (Fade/Scale), 
/// а также Lock/Unlock ввода.
/// </summary>
public class CanvasPanel : MonoBehaviour
{
    [Header("Start Hidden?")]
    [SerializeField] private bool startHidden = false;

    [Header("Tween Settings")]
    [SerializeField] private float showDuration = 0.3f;
    [SerializeField] private float hideDuration = 0.3f;
    [SerializeField] private bool useFadeAnimation = true;
    [SerializeField] private bool useScaleAnimation = false;

    [Tooltip("Scale при скрытой панели (если useScaleAnimation = true)")]
    [SerializeField] private float hiddenScale = 0.8f;
    [Tooltip("Scale при показанной панели")]
    [SerializeField] private float shownScale = 1.0f;

    [Tooltip("Ease кривая при показе")]
    [SerializeField] private Ease showEase = Ease.OutQuad;
    [Tooltip("Ease кривая при скрытии")]
    [SerializeField] private Ease hideEase = Ease.InQuad;

    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private RectTransform rectTransform;
    
    protected Sequence currentSequence;
    protected bool isLocked = false;  // для LockInput / UnlockInput

    protected virtual void Awake()
    {
        // Если отмечено startHidden, скрываем мгновенно
        if (startHidden)
        {
            HideInstant();
        }
        else
        {
            // Приводим в видимое состояние
            canvasGroup.alpha = 1f;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;

            if (useScaleAnimation && rectTransform)
            {
                rectTransform.localScale = Vector3.one * shownScale;
            }
        }
    }

    /// <summary>
    /// Плавно показать панель с учётом настроек (Fade/Scale).
    /// Если уже видна, повторный Show не запускает анимацию.
    /// </summary>
    public virtual void Show()
    {
        // Если панель уже полностью видима, выходим
        if (IsVisible) 
            return;

        // Если панель разблокирована, то можно взаимодействовать, иначе оставляем заблокированной
        canvasGroup.blocksRaycasts = !isLocked;
        canvasGroup.interactable = !isLocked;

        // Останавливаем предыдущую анимацию, если есть
        currentSequence?.Kill();
        currentSequence = DOTween.Sequence();

        // --- Fade (alpha) ---
        if (useFadeAnimation)
        {
            // Сначала выставляем alpha=0, анимируем к 1
            canvasGroup.alpha = 0f;
            currentSequence.Append(
                DOTween.To(() => canvasGroup.alpha, x => canvasGroup.alpha = x, 1f, showDuration)
                       .SetEase(showEase)
            );
        }
        else
        {
            // Без анимации по альфе
            canvasGroup.alpha = 1f;
        }

        // --- Scale ---
        if (useScaleAnimation && rectTransform)
        {
            rectTransform.localScale = Vector3.one * hiddenScale;
            currentSequence.Join(
                rectTransform.DOScale(shownScale, showDuration).SetEase(showEase)
            );
        }

        currentSequence.OnComplete(() =>
        {
            // По завершении анимации
            canvasGroup.blocksRaycasts = !isLocked;
            canvasGroup.interactable = !isLocked;
        });
    }

    /// <summary>
    /// Плавно скрыть панель (Fade/Scale).
    /// Если панель уже скрыта, повторный Hide не сработает.
    /// </summary>
    public virtual void Hide()
    {
        // Если уже скрыта, выходим
        if (!IsVisible) 
            return;

        currentSequence?.Kill();
        currentSequence = DOTween.Sequence();

        // Fade
        if (useFadeAnimation)
        {
            currentSequence.Append(
                DOTween.To(() => canvasGroup.alpha, x => canvasGroup.alpha = x, 0f, hideDuration)
                       .SetEase(hideEase)
            );
        }
        else
        {
            canvasGroup.alpha = 0f;
        }

        // Scale
        if (useScaleAnimation && rectTransform)
        {
            currentSequence.Join(
                rectTransform.DOScale(hiddenScale, hideDuration).SetEase(hideEase)
            );
        }

        currentSequence.OnComplete(() =>
        {
            // По завершении скрытия
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        });
    }

    /// <summary>
    /// Мгновенно скрыть панель (без анимации).
    /// </summary>
    public virtual void HideInstant()
    {
        currentSequence?.Kill();

        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        if (useScaleAnimation && rectTransform)
        {
            rectTransform.localScale = Vector3.one * hiddenScale;
        }
    }

    /// <summary>
    /// Заблокировать ввод: панель видна, но невозможно взаимодействовать.
    /// </summary>
    public virtual void LockInput()
    {
        isLocked = true;
        canvasGroup.interactable = false;
        // blocksRaycasts можно оставить true, если хотим блокировать клики снизу
    }

    /// <summary>
    /// Разблокировать ввод.
    /// Если панель по-прежнему видна, сделаем её interactable.
    /// </summary>
    public virtual void UnlockInput()
    {
        isLocked = false;
        if (IsVisible)
        {
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }
    }

    /// <summary>
    /// Проверяем, видно ли панель (gameObject.activeSelf + alpha>0)
    /// </summary>
    public virtual bool IsVisible
    {
        get
        {
            return gameObject.activeSelf && canvasGroup.alpha > 0.01f;
        }
    }

    /// <summary>
    /// Флаг, заблокирован ли ввод.
    /// </summary>
    public virtual bool IsLocked => isLocked;
}
