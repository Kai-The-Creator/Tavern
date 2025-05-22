using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using UnityEditorInternal;
using System.Text.RegularExpressions;

[System.Serializable]
public class MyData
{
    public List<string> stringList = new List<string>();
}

public class GameTagsEditorWindow : EditorWindow
{
    private MyData myData = new MyData();
    private GameTagsData attribute;
    private ReorderableList stringListReorderable;
    private string scriptPath;
    private Vector2 scrollPosition;

    [MenuItem("Custom/GameTagsEditorWindow")]
    public static void ShowWindow()
    {
        GetWindow<GameTagsEditorWindow>("♜ GameTagsEditorWindow");
    }

    private void OnEnable()
    {
        LoadFromJson();
        DrawList();
    }

    private void DrawList()
    {
        
        stringListReorderable = new ReorderableList(myData.stringList, typeof(string), true, true, true, true);
        stringListReorderable.drawHeaderCallback = (Rect rect) => GUI.Label(rect, "List of Strings");
        stringListReorderable.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
        {
            myData.stringList[index] = EditorGUI.TextField(rect, myData.stringList[index]);
        };
        
    }

    private void OnGUI()
    {
        GameTagsData previousEnumValue = attribute;

        attribute = (GameTagsData)EditorGUILayout.EnumPopup("Attribute", attribute);

        if (attribute != previousEnumValue)
        {
            LoadFromJson();
        }

        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
        EditorGUILayout.BeginVertical("List");
        stringListReorderable.DoLayoutList();
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndScrollView();

        /*if (GUILayout.Button("Сохранить в JSON"))
        {
            SaveToJson(myData);
        }*/

        scriptPath = "Assets/Resources/EditorAttributes/Classes/" + attribute.ToString() + ".cs";

        if (!File.Exists(scriptPath))
        {
            if (GUILayout.Button("Создать класс"))
            {
                SaveToJson(myData);
                CreateScript();
            }
        }
        else
        {
            if (GUILayout.Button("Создать параметры"))
            {
                SaveToJson(myData);
                ClearEnumInScript();
                CreateEnumValuesInScript();
            }
        }

       
    }

    private void SaveToJson(MyData data)
    {
        string jsonPath = $"Assets/Resources/EditorAttributes/{attribute.ToString()}.json";

        string jsonString = JsonUtility.ToJson(data);

        File.WriteAllText(jsonPath, jsonString);

        AssetDatabase.Refresh();

        Debug.Log("Данные сохранены в JSON файл: " + jsonPath);
    }

    private void LoadFromJson()
    {
        TextAsset jsonAsset = Resources.Load<TextAsset>($"EditorAttributes/{attribute.ToString()}");
        if (jsonAsset != null)
        {
            myData = JsonUtility.FromJson<MyData>(jsonAsset.text);
            DrawList();
            Debug.Log("Данные загружены из JSON.");
        }
        else
        {
            myData = new MyData();
            DrawList();
            SaveToJson(myData);
        }
    }

    private void CreateScript()
    {
        if (!System.IO.File.Exists(scriptPath))
        {
            System.IO.File.WriteAllText(scriptPath, "public enum " + attribute.ToString() + "\n{\n     }");
            AssetDatabase.Refresh();
            Debug.Log(attribute.ToString() + ".cs created in Resources folder.");
            
        }
        else
        {
            Debug.LogError("Script with name " + attribute.ToString() + ".cs already exists in Resources folder.");
        }
    }

    private void CreateMethodInScript()
    {
        if (System.IO.File.Exists(scriptPath))
        {
            string scriptContent = System.IO.File.ReadAllText(scriptPath);

            int classStartIndex = scriptContent.IndexOf("class ");
            int classEndIndex = scriptContent.LastIndexOf("}");

            if (classStartIndex != -1 && classEndIndex != -1)
            {
                string classContent = scriptContent.Substring(classStartIndex, classEndIndex - classStartIndex);

                List<string> methodDefinitions = new List<string>();

                foreach (string methodName in myData.stringList)
                {
                    string formattedMethodName = methodName.Replace(" ", "").Replace(".", "_");

                    string methodDefinition = "\n    public static string " + formattedMethodName + "=> \"" + formattedMethodName + "\";\n";

                    methodDefinitions.Add(methodDefinition);
                }

                string methodsContent = string.Join("", methodDefinitions);

                classContent += methodsContent;

                scriptContent = scriptContent.Remove(classStartIndex, classEndIndex - classStartIndex);
                scriptContent = scriptContent.Insert(classStartIndex, classContent);

                System.IO.File.WriteAllText(scriptPath, scriptContent);
                AssetDatabase.Refresh();
                Debug.Log("Methods created in " + attribute.ToString());
            }
            else
            {
                Debug.LogError("Script with name " + attribute.ToString() + ".cs does not contain a class.");
            }
        }
        else
        {
            Debug.LogError("Script with name " + attribute.ToString() + ".cs does not exist.");
        }
    }

    private void CreateEnumValuesInScript()
    {
        if (System.IO.File.Exists(scriptPath))
        {
            string scriptContent = System.IO.File.ReadAllText(scriptPath);

            int classStartIndex = scriptContent.IndexOf("enum ");
            int classEndIndex = scriptContent.LastIndexOf("}");

            if (classStartIndex != -1 && classEndIndex != -1)
            {
                string classContent = scriptContent.Substring(classStartIndex, classEndIndex - classStartIndex);

                List<string> methodDefinitions = new List<string>();

                foreach (string methodName in myData.stringList)
                {
                    string formattedMethodName = methodName.Replace(" ", "").Replace(".", "_");

                    string methodDefinition = $"    {formattedMethodName},\n";

                    methodDefinitions.Add(methodDefinition);
                }

                string methodsContent = string.Join("", methodDefinitions);

                classContent += methodsContent;

                scriptContent = scriptContent.Remove(classStartIndex, classEndIndex - classStartIndex);
                scriptContent = scriptContent.Insert(classStartIndex, classContent);

                System.IO.File.WriteAllText(scriptPath, scriptContent);
                AssetDatabase.Refresh();
                Debug.Log("Methods created in " + attribute.ToString());
            }
            else
            {
                Debug.LogError("Script with name " + attribute.ToString() + ".cs does not contain a class.");
            }
        }
        else
        {
            Debug.LogError("Script with name " + attribute.ToString() + ".cs does not exist.");
        }
    }

    private void ClearClassInScript()
    {
        
        if (System.IO.File.Exists(scriptPath))
        {
            string scriptContent = System.IO.File.ReadAllText(scriptPath);

            int classStartIndex = scriptContent.IndexOf("class ");
            int classEndIndex = scriptContent.LastIndexOf("}");

            if (classStartIndex != -1 && classEndIndex != -1)
            {
                string classContent = scriptContent.Substring(classStartIndex, classEndIndex - classStartIndex);

                string[] lines = classContent.Split('\n');

                for (int i = 0; i < lines.Length; i++)
                {
                    string line = lines[i].Trim();
                    if (line.StartsWith("public ") || line.StartsWith("private ") || line.StartsWith("protected "))
                    {
                        while (!lines[i].Contains("=>"))
                        {
                            i++;
                        }
                        int closingBraceIndex = FindMatchingClosingBrace(lines, i);
                        if (closingBraceIndex != -1)
                        {
                            for (int j = i; j <= closingBraceIndex; j++)
                            {
                                lines[j] = string.Empty;
                            }
                        }
                    }
                }

                classContent = string.Join("\n", lines);

                scriptContent = scriptContent.Remove(classStartIndex, classEndIndex - classStartIndex);
                scriptContent = scriptContent.Insert(classStartIndex, classContent);
                scriptContent = scriptContent.Replace("\n", "");

                System.IO.File.WriteAllText(scriptPath, scriptContent);
                AssetDatabase.Refresh();
                Debug.Log("Methods cleared in " + attribute.ToString());
            }
            else
            {
                Debug.LogError("Script with name " + attribute.ToString() + ".cs does not contain a class.");
            }
        }
        else
        {
            Debug.LogError("Script with name " + attribute.ToString() + ".cs does not exist.");
        }
    }

    private void ClearEnumInScript()
    {
        if (System.IO.File.Exists(scriptPath))
        {
            string scriptContent = System.IO.File.ReadAllText(scriptPath);

            int classStartIndex = scriptContent.IndexOf("enum ");
            int classEndIndex = scriptContent.LastIndexOf("}");

            if (classStartIndex != -1 && classEndIndex != -1)
            {
                string classContent = scriptContent.Substring(classStartIndex, classEndIndex - classStartIndex);

                string pattern = @"{[^{}]*}";

                // Заменяем найденные совпадения на пустую строку
                string cleanedCode = Regex.Replace(scriptContent, pattern, "{\n}");


                scriptContent = cleanedCode;

                System.IO.File.WriteAllText(scriptPath, cleanedCode);
                AssetDatabase.Refresh();
                Debug.Log("Methods cleared in " + attribute.ToString());
            }
            else
            {
                Debug.LogError("Script with name " + attribute.ToString() + ".cs does not contain a class.");
            }
        }
        else
        {
            Debug.LogError("Script with name " + attribute.ToString() + ".cs does not exist.");
        }
    }

    private int FindMatchingClosingBrace(string[] lines, int startIndex)
    {
        int openBraces = 0;
        for (int i = startIndex; i < lines.Length; i++)
        {
            openBraces += lines[i].Split('{').Length - 1;
            int closeBraces = lines[i].Split('}').Length - 1;
            openBraces -= closeBraces;
            if (openBraces <= 0)
            {
                return i;
            }
        }
        return -1;
    }
}