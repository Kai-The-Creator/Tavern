using System;
using _Core._Global.Utility;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class DialogueAnswerButton : Element
{
    [SerializeField] private TextMeshProUGUI answerText;
    [SerializeField] private VirtualButton button;

    public void Init(string text, UnityAction action)
    {
        answerText.text = text;
        button.AddListener(action);

        Image img = button.GetComponent<Image>();

        Activate();
    }
}
