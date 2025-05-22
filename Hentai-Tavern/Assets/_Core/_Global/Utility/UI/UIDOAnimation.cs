using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UI;

public class UIDOAnimation : MonoBehaviour
{
    [Range(1f, 2f)]
    public float scale = 1f;
    [Range(0f, 1f)]
    public float duration = 0.3f;
    [Space]
    public Vector3 startPosition;
    public Vector3 endPosition;

    [Space]
    public Vector3 startRotation;
    public Vector3 endRotation;

    [Space]
    [Range(0f, 1f)]
    public float fadeCount;


    public void OpenScaleAnimation()
    {
        transform.localScale = Vector3.zero;
        Vector3 endScale = new Vector3(scale, scale, scale);
        transform.DOScale(endScale, duration);
    }

    public void CloseScaleAnimation()
    {
        Vector3 endScale = new Vector3(scale, scale, scale);
        transform.localScale = endScale;
        transform.DOScale(Vector3.zero, duration);
    }

    public void ResetToStartScaleAnimation()
    {
        transform.localScale = Vector3.zero;
    }

    public void OpenMoveAnimation()
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        rectTransform.anchoredPosition = startPosition;
        rectTransform.DOAnchorPos(endPosition, duration).SetEase(Ease.InOutQuad);
    }

    public void CloseMoveAnimation()
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        rectTransform.anchoredPosition = endPosition;
        rectTransform.DOAnchorPos(startPosition, duration).SetEase(Ease.InOutQuad);
    }

    public void OpenFadeAnimation()
    {
        Image img = GetComponent<Image>();
        Color c = img.color;
        c.a = 0;
        img.color = c;
        img.DOFade(fadeCount, duration);
    }

    public void CloseFadeAnimation()
    {
        Image img = GetComponent<Image>();
        Color c = img.color;
        c.a = 1;
        img.color = c;
        img.DOFade(0, duration);
    }

    public void OpenRotationAnimation(Action action = null)
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        rectTransform.rotation = Quaternion.Euler(startRotation);
        rectTransform.DORotate(endRotation, duration).OnComplete(() => { action?.Invoke(); }); 
    }

    public void CloseRotationAnimation(Action action = null)
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        rectTransform.rotation = Quaternion.Euler(endRotation);
        rectTransform.DORotate(startRotation, duration).OnComplete(()=> { action?.Invoke(); });
    }

    public void ResetRotation()
    {

    }
}
