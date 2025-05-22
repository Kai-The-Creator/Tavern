using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CSVLoadConfig), true)]
public class CSVConfigEditor : Editor
{
    public override void OnInspectorGUI()
    {

        CSVLoadConfig node = (CSVLoadConfig)target;

        if (GUILayout.Button("Load"))
        {
            node.Load();
            Debug.Log("Load");
        }

        base.OnInspectorGUI();


    }
}
