using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DungeonGenerator))]
public class DungeonGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // ������ ����������� ���������
        base.OnInspectorGUI();

        // ��������� ������ ���������
        if (GUILayout.Button("Generate Dungeon"))
        {
            // �������� ������� ���������
            DungeonGenerator generator = (DungeonGenerator)target;

            // ������� ������ ����� ����� ����������
            generator.ClearDungeon();

            // ���������� ����� ����������
            generator.GenerateDungeon();
        }
    }
}
