using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CSVLoadConfig : ScriptableObject
{
    public string id;
    public int gridId;

    public void Load()
    {
        CSVParse.FetchDataCSV(id, gridId, SetData);
    }

    public virtual void SetData(List<Dictionary<string, string>> dataList)
    {
        
    }

    public void SetDirty(ScriptableObject obj)
    {
#if UNITY_EDITOR
        EditorUtility.SetDirty(obj);
#endif
    }

    public T GetEnumValue<T>(string enumName) where T : System.Enum
    {
        return (T)System.Enum.Parse(typeof(T), enumName);
    }

    public List<StringLang> SetLang(Dictionary<string, string> data)
    {
        List<StringLang> list = new();
        foreach (var item in Enum.GetValues(typeof(Language)))
        {
            list.Add(GetTextByLang((Language)item, item.ToString(), data));
        }

        return list;
    }

    public StringLang GetTextByLang(Language language, string tag, Dictionary<string, string> data)
    {
        data.TryGetValue(tag, out string lang);
        StringLang Lang = new(language, lang);
        return Lang;
    }

    public List<T> GetListObjs<T>(List<string> list) where T : System.Enum
    {
        List<T> result = new List<T>();

        foreach (var item in list)
        {
            var t = GetEnumValue<T>(item);
            result.Add(t);
        }


        return result;
    }

    public List<string> ParceStringToList(string value)
    {
        List<string> list = new();

        if (string.IsNullOrEmpty(value))
        {
            return list;
        }

        string[] splitWords = value.Split(',');

        foreach (string word in splitWords)
        {
            string trimmedWord = word.Trim();
            list.Add(trimmedWord);
        }

        return list;
    }

    public int ParseInt(List<Dictionary<string, string>> dataList, int ID, string varName)
    {
        dataList[ID].TryGetValue(varName, out string v);
        int value = -1;
        int.TryParse(v, out value);
        return value;
    }

    public int ParseInt(string v)
    {
        int value = -1;
        int.TryParse(v, out value);
        return value;
    }

    public float ParseFloat(List<Dictionary<string, string>> dataList, int ID, string varName)
    {
        dataList[ID].TryGetValue(varName, out string v);
        float value = -1;
        float.TryParse(v, out value);
        return value;
    }

    public float ParseFloat(string v)
    {
        if (float.TryParse(v, out float value))
        {
            return value;
        }
        else
        {
            // ћожно вернуть значение по умолчанию, например, 0 или выбросить исключение.
            // return 0; // или
            throw new FormatException($"Ќе удалось преобразовать строку '{v}' в число с плавающей зап€той.");
        }
    }
}
