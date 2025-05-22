using System.Collections;
using System.Collections.Generic;
using _Core._Global.GConfig;
using _Core._Global.Services;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class LangTextSwitcher : MonoBehaviour
{
    [Inject] private LocalizationUIConfig config;

    public TextSwitcherType Type;
    public LocalizationUnit tag;

    public bool isButton;

    private TextMeshProUGUI tmp_text;


    private void Start()
    {
        if(config == null)
            config = GService.GetService<GlobalConfigService>().LocalizationUIConfig;

        LocalizationManager.instance.textSwitcherList.Add(this);

        if(isButton)
            tmp_text = GetComponentInChildren<TextMeshProUGUI>();
        else
            tmp_text = GetComponent<TextMeshProUGUI>();

        Switch();
    }

    private void OnDestroy()
    {
        LocalizationManager.instance.textSwitcherList.Remove(this);
    }

    public void Switch()
    {
        Language lang = LocalizationManager.Language;

        tmp_text.font = LocalizationManager.instance.Config.GetData(lang).GetFont(Type);

        if (tag == LocalizationUnit.None) return;

        tmp_text.text = config.GetUnit(tag);
    }
}


