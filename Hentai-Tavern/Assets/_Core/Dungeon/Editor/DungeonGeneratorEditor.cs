using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DungeonGenerator))]
public class DungeonGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Рисуем стандартный инспектор
        base.OnInspectorGUI();

        // Добавляем кнопку генерации
        if (GUILayout.Button("Generate Dungeon"))
        {
            // Получаем целевой генератор
            DungeonGenerator generator = (DungeonGenerator)target;

            // Очищаем старые чанки перед генерацией
            generator.ClearDungeon();

            // Генерируем новое подземелье
            generator.GenerateDungeon();
        }
    }
}
