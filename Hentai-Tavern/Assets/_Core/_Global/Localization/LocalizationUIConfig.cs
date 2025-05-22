using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LocalizationUIConfig", menuName = "GAME/Localization/LocalizationUIConfig")]
public class LocalizationUIConfig : CSVLoadConfig
{
    [Space]

    public List<LocalizationUIUnit> list = new();

    public string GetUnit(LocalizationUnit tag)
    {
        return list.Find(u => u.tag == tag).GetName();
    }

    public override void SetData(List<Dictionary<string, string>> dataList)
    {
        list.Clear();

        for (int i = 0; i < dataList.Count; i++)
        {
            int ID = i;
            dataList[ID].TryGetValue("Tag", out string tag);
            LocalizationUnit t = GetEnumValue<LocalizationUnit>(tag);

            SetContent(t, SetLang(dataList[ID]));
        }

        SetDirty(this);
    }

    internal void SetContent(LocalizationUnit t, List<StringLang> stringLangs)
    {
        LocalizationUIUnit unit = new();
        unit.tag = t;
        unit.Name = stringLangs;
        list.Add(unit);
    }
}

[System.Serializable]
public class LocalizationUIUnit
{
    public LocalizationUnit tag;
    public List<StringLang> Name = new();

    public string GetName() => Name.Find(n => n.language == LocalizationManager.Language).Text;
}
