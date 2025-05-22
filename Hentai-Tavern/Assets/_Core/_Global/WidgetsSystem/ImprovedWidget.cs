using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using UnityEngine.EventSystems;

public class ImprovedWidget : Widget, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [Header("UI Refs")]
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI labelText;

    private ImprovedWidgetConfig configData;

    private Sequence hoverSequence;
    private Sequence pressSequence;
    private Vector3 defaultScale = Vector3.one;

    private bool isPressed = false;

    public override void OnStartWidget()
    {
        base.OnStartWidget();
        defaultScale = transform.localScale;
    }

    public void ApplyConfig(ImprovedWidgetConfig cfg)
    {
        configData = cfg;

        if (iconImage && cfg.iconSprite)
        {
            iconImage.sprite = cfg.iconSprite;
        }
        if (labelText && !string.IsNullOrEmpty(cfg.labelText))
        {
            labelText.text = cfg.labelText;
        }
    }

    private void ExecuteOpenAction()
    {
        if (configData && configData.openAction)
        {
            configData.openAction.SendAction();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (configData && configData.enableHoverAnimation)
        {
            hoverSequence?.Kill();
            hoverSequence = DOTween.Sequence()
                .Append(transform.DOScale(defaultScale * configData.hoverScale, 0.2f));
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (configData && configData.enableHoverAnimation)
        {
            hoverSequence?.Kill();
            hoverSequence = DOTween.Sequence()
                .Append(transform.DOScale(defaultScale, 0.2f));
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isPressed = true;
        if (configData && configData.enablePressAnimation)
        {
            pressSequence?.Kill();
            pressSequence = DOTween.Sequence()
                .Append(transform.DOScale(defaultScale * configData.pressScale, 0.1f));
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (configData && isPressed)
        {
            ExecuteOpenAction();
        }
        isPressed = false;

        if (configData && configData.enablePressAnimation)
        {
            pressSequence?.Kill();
            pressSequence = DOTween.Sequence()
                .Append(transform.DOScale(defaultScale, 0.15f));
        }
    }

    public override void OnUpdateWidget()
    {
    }
}
