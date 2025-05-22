using _Core._Global.Utility;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class VirtualButton : Element,
    IPointerEnterHandler,
    IPointerExitHandler,
    IPointerClickHandler,
    IPointerUpHandler
{
    [Header("Config (обязателен)")]
    public VirtualButtonConfig config;

    [Header("Events")]
    public UnityEvent OnClick;    
    public UnityEvent OnClickAgain;
    public UnityEvent OnEnterHover;
    public UnityEvent OnExitHover;

    [Header("Double-Click Mode")]
    public bool IsDoubleClickMode;
    [Tooltip("Время (сек), в течение которого второй клик считается двойным")]
    public float doubleClickThreshold = 0.3f;

    [Header("Tweens Setup")]
    [SerializeField] private float hoverDuration = 0.3f;  
    [SerializeField] private float pressDuration = 0.2f;   

    private Image _image;
    private float _defaultScale = 1f;
    private bool _isInteractable = true;

    private float _lastClickTime = -999f; 

    private Sequence _hoverSequence; 
    private Sequence _pressSequence;

    private void Awake()
    {
        _image = GetComponent<Image>();
        _defaultScale = transform.localScale.x <= 0 ? 1f : transform.localScale.x;

        if (!config)
        {
            Debug.LogWarning($"{name}: VirtualButtonConfig не назначен! Анимации и спрайты работать не будут.");
            return;
        }

        if (config.isDisabled)
        {
            _isInteractable = false;
            if (!config.emptySpriteMode && config.DisabledImage)
                _image.sprite = config.DisabledImage;
        }
        else
        {
            _isInteractable = true;
            if (!config.emptySpriteMode && config.DefaultImage)
                _image.sprite = config.DefaultImage;
        }
    }

    private void OnEnable()
    {
        ResetButtonState();
    }

    private void OnDisable()
    {
        KillAllTweens();
    }

    private void OnDestroy()
    {
        KillAllTweens();
    }

    private void KillAllTweens()
    {
        if (_hoverSequence != null)
        {
            _hoverSequence.Kill();
            _hoverSequence = null;
        }
        if (_pressSequence != null)
        {
            _pressSequence.Kill();
            _pressSequence = null;
        }
    }

    private void ResetButtonState()
    {
        KillAllTweens();
        _lastClickTime = -999f;

        if (config && !config.emptySpriteMode && config.DefaultImage && _isInteractable)
        {
            _image.sprite = config.DefaultImage;
        }
        else if (config && !config.emptySpriteMode && !string.IsNullOrEmpty(config.DisabledImage?.name) && !_isInteractable)
        {
            _image.sprite = config.DisabledImage;
        }

        transform.localScale = Vector3.one * _defaultScale;
    }

    public bool IsInteractable => _isInteractable;

    #region Pointer Events

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!_isInteractable || config == null) return;

        if (!config.emptySpriteMode && config.HowerImage)
            _image.sprite = config.HowerImage;

        KillHoverSequence();
        _hoverSequence = DOTween.Sequence()
            .Append(transform.DOScale(GetHoverScale(), hoverDuration).SetEase(Ease.OutQuad));

        OnEnterHover?.Invoke();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!_isInteractable || config == null) return;

        if (!config.emptySpriteMode && config.DefaultImage)
            _image.sprite = config.DefaultImage;

        KillHoverSequence();
        _hoverSequence = DOTween.Sequence()
            .Append(transform.DOScale(_defaultScale, hoverDuration).SetEase(Ease.OutQuad));

        OnEnterHover?.Invoke();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!_isInteractable || config == null) return;

        if (!config.emptySpriteMode && config.PressedImage)
            _image.sprite = config.PressedImage;

        KillPressSequence();
        _pressSequence = DOTween.Sequence().Append(transform.DOScale(GetPressedScale(), pressDuration).SetEase(Ease.OutQuad));

        if (IsDoubleClickMode)
        {
            // Проверяем, сколько времени прошло с прошлого клика
            float timeSinceLastClick = Time.time - _lastClickTime;
            if (timeSinceLastClick <= doubleClickThreshold)
            {
                OnClickAgain?.Invoke();
                _lastClickTime = -999f;  // сбрасываем, чтобы след. клик не посчитался двойным
            }
            else
            {
                // Одиночный клик
                OnClick?.Invoke();
                _lastClickTime = Time.time;
            }
        }
        else
        {
            // Одиночный клик
            OnClick?.Invoke();
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!_isInteractable || config == null) return;

        if (!config.emptySpriteMode && config.HowerImage)
        {
            _image.sprite = config.HowerImage;
        }

        KillPressSequence();
        _pressSequence = DOTween.Sequence()
            .Append(transform.DOScale(GetHoverScale(), pressDuration).SetEase(Ease.OutQuad));
    }

    #endregion

    #region Public API

    public void SetInactive(bool disabled)
    {
        _isInteractable = !disabled;
        KillAllTweens();
        if (config == null || config.emptySpriteMode) return;

        if (_isInteractable)
        {
            if (config.DefaultImage)
                _image.sprite = config.DefaultImage;
            transform.localScale = Vector3.one * _defaultScale;
        }
        else
        {
            if (config.DisabledImage)
                _image.sprite = config.DisabledImage;
            transform.localScale = Vector3.one * _defaultScale;
        }
    }

    public void AddListener(UnityAction action)
    {
        OnClick.RemoveAllListeners();
        OnClick.AddListener(action);
    }

    public void AddHoverListener(UnityAction action)
    {
        OnEnterHover.RemoveAllListeners();
        OnEnterHover.AddListener(action);
    }

    public void AddExitHoverListener(UnityAction action)
    {
        OnExitHover.RemoveAllListeners();
        OnExitHover.AddListener(action);
    }

    #endregion

    #region Internal Helpers

    private float GetHoverScale()
    {
        if (config.HowerScale > 0) return config.HowerScale;
        return _defaultScale;
    }

    private float GetPressedScale()
    {
        if (config.PressedScale > 0) return config.PressedScale;
        return _defaultScale;
    }

    private void KillHoverSequence()
    {
        if (_hoverSequence != null)
        {
            _hoverSequence.Kill();
            _hoverSequence = null;
        }
    }

    private void KillPressSequence()
    {
        if (_pressSequence != null)
        {
            _pressSequence.Kill();
            _pressSequence = null;
        }
    }

    #endregion
}
