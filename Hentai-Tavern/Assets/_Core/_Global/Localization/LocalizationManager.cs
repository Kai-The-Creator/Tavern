using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class LocalizationManager : Singleton<LocalizationManager>
{
    [Inject] private LanguageConfig config;

    public List<LangTextSwitcher> textSwitcherList = new();

    public override void AwakeAction()
    {
        var culture = System.Globalization.CultureInfo.CurrentCulture;
        Language = Language.EN;
        switch (culture.ToString())
        {
            case "ru-RU":
                Language = Language.RU;
                break;
            case "en-US":
                Language = Language.EN;
                break;
            case "zh-CN":
                Language = Language.CHINESE_SIMPLE;
                break;
            case "de-DE":
                Language = Language.GERMAN;
                break;
            case "es-ES":
                Language = Language.SPANISH;
                break;
            case "tr-TR":
                Language = Language.TURKISH;
                break;
            case "pt-PT":
            case "pt-BR":
                Language = Language.PORTUGUESE;
                break;
            case "it-IT":
                Language = Language.ITALIAN;
                break;
            default:
                Language = Language.EN;
                break;
        }

        DontDestroyOnLoad(gameObject);

        //SetLang(Language.EN);
    }

    public static Language Language {  get; private set; }
    public LanguageConfig Config { get => config; }

    public void SetLang(Language language)
    {
        Language = language;

        foreach (var t in textSwitcherList)
        {
            t.Switch();
        }
    }
}
