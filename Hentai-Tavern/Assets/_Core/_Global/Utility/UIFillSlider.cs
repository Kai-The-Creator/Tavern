using _Core._Global.Utility;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIFillSlider : Element
{
    [SerializeField] private Image fillImg;
    [SerializeField] private TextMeshProUGUI fillText;

    public UnityEvent onStartEvent;
    public UnityEvent onEndEvent;

    private float max;
    private float current;

    public void Init(float max, UnityAction onStart, UnityAction onEnd)
    {
        current = 0;

        this.max = max;
        if(fillImg != null)
        {
            fillImg.fillAmount = current / max;
        }

        if(fillText != null)
        {
            fillText.text = $"{current} / {max}";
        }

        onStartEvent.RemoveAllListeners();
        onEndEvent.RemoveAllListeners();

        onStartEvent.AddListener(onStart);
        onEndEvent.AddListener(onEnd);

        onStart?.Invoke();
        Activate();
    }

    public void AddValue(float value)
    {
        current += value;

        if(current > max)
        {
            current = max;
        }

        Debug.Log(value);
        UpdateSlider();
    }

    public void RemoveValue(float value)
    {
        current -= value;

        if(current < 0)
            current = 0;

        UpdateSlider();
    }

    private void UpdateSlider()
    {
        if (fillImg != null)
        {
            fillImg.fillAmount = current / max;
        }

        if (fillText != null)
        {
            fillText.text = $"{current} / {max}";
        }

        if(current >= max)
        {
            onEndEvent?.Invoke();
        }
    }
}
