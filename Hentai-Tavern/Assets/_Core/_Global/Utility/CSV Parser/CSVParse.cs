using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Windows;


[ExecuteInEditMode]
public static class CSVParse
{
    private static MonoBehaviourHelper helper;

    public static void FetchDataCSV(string id, int gridId, Action<List<Dictionary<string, string>>> loadAction)
    {
        helper = MonoBehaviourHelper.Instance;
        helper.StartCoroutine(EFetchDataCSV(id, gridId, loadAction));
    }

    private static IEnumerator EFetchDataCSV(string id, int gridId, Action<List<Dictionary<string, string>>> loadAction)
    {
        string url = $@"https://docs.google.com/spreadsheet/ccc?key={id}&usp=sharing&output=csv&id=KEY&gid={gridId}";

        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                string content = webRequest.downloadHandler.text;
                string[] lines = content.Split('\n');

                if (lines.Length > 0)
                {
                    string[] headers = SplitCSVLine(lines[0]);

                    List<Dictionary<string, string>> dataList = new List<Dictionary<string, string>>();

                    for (int i = 1; i < lines.Length; i++)
                    {
                        string[] values = SplitCSVLine(lines[i]);

                        Dictionary<string, string> rowData = new Dictionary<string, string>();

                        for (int j = 0; j < headers.Length && j < values.Length; j++)
                        {
                            rowData[headers[j]] = values[j];
                        }

                        dataList.Add(rowData);
                    }

                    loadAction?.Invoke(dataList);
                    

                    Debug.Log("Данные успешно преобразованы");
                }
                else
                {
                    Debug.LogError("Нет данных для обработки.");
                }
            }
            else
            {
                Debug.LogError("Ошибка при получении данных: " + webRequest.error);
            }
        }

        helper.Clear();
    }

    private static string[] SplitCSVLine(string line)
    {
        List<string> values = new List<string>();
        bool insideQuotes = false;
        string currentField = "";

        foreach (char c in line)
        {
            if (c == '"')
            {
                insideQuotes = !insideQuotes;
            }
            else if (c == ',' && !insideQuotes)
            {
                values.Add(currentField);
                currentField = "";
            }
            else
            {
                currentField += c;
            }
        }

        values.Add(currentField);
        return values.ToArray();
    }
}

[ExecuteInEditMode]
public class MonoBehaviourHelper : MonoBehaviour
{
    private static MonoBehaviourHelper instance;

    public static MonoBehaviourHelper Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject go = new GameObject("MonoBehaviourHelper");
                instance = go.AddComponent<MonoBehaviourHelper>();
            }
            return instance;
        }
    }

    public void Clear()
    {
        DestroyImmediate(gameObject);
    }
}