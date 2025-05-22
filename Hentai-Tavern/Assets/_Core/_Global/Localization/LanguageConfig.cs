using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[CreateAssetMenu(fileName = "LanguageConfig", menuName = "GAME/Localization/LanguageConfig")]
public class LanguageConfig : ScriptableObject
{
    public List<LangData> langs;

    public LangData GetData(Language lang)  => langs.Find(l => l.lang == lang);
    
}

[System.Serializable]
public struct LangData
{
    public string Name;
    public Language lang;
    public List<FontData> fonts;

    public TMP_FontAsset GetFont(TextSwitcherType type) =>
        fonts.Find(f => f.type == type).Font;
}

[System.Serializable]
public struct FontData
{
    public TextSwitcherType type;
    public TMP_FontAsset Font;
}

public enum TextSwitcherType
{
    Base,
    Tipical,
    BaseBold,
    TipicalBold
}