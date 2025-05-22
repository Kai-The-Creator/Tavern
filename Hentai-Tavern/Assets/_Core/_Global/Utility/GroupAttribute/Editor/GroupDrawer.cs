#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Reflection;

public static class GroupFoldoutState
{
    public static Dictionary<string, bool> foldoutStates = new Dictionary<string, bool>();
}

[CustomPropertyDrawer(typeof(GroupAttribute))]
public class GroupDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        GroupAttribute group = (GroupAttribute)attribute;

        // Убедимся, что состояние группы существует в словаре
        if (!GroupFoldoutState.foldoutStates.ContainsKey(group.groupName))
        {
            GroupFoldoutState.foldoutStates[group.groupName] = true;
        }

        // Если это первое поле в группе, создаем кнопку для сворачивания/разворачивания
        if (IsFirstPropertyInGroup(property))
        {
            Rect buttonRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            Color originalColor = GUI.backgroundColor;
            GUI.backgroundColor = GroupFoldoutState.foldoutStates[group.groupName] ? Color.green : Color.cyan;

            if (GUI.Button(buttonRect, group.groupName))
            {
                GroupFoldoutState.foldoutStates[group.groupName] = !GroupFoldoutState.foldoutStates[group.groupName];
            }

            GUI.backgroundColor = originalColor;
            position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
        }

        // Если группа развернута, отображаем поля
        if (GroupFoldoutState.foldoutStates[group.groupName])
        {
            EditorGUI.indentLevel++;
            EditorGUI.PropertyField(position, property, label, true);
            EditorGUI.indentLevel--;
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        GroupAttribute group = (GroupAttribute)attribute;

        // Убедимся, что состояние группы существует в словаре
        if (!GroupFoldoutState.foldoutStates.ContainsKey(group.groupName))
        {
            GroupFoldoutState.foldoutStates[group.groupName] = true;
        }

        // Высота свойства, плюс высота кнопки, если это первое свойство в группе
        float propertyHeight = EditorGUI.GetPropertyHeight(property, label, true);
        if (GroupFoldoutState.foldoutStates[group.groupName])
        {
            return propertyHeight + (IsFirstPropertyInGroup(property) ? EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing : 0);
        }
        else
        {
            return IsFirstPropertyInGroup(property) ? EditorGUIUtility.singleLineHeight : 0;
        }
    }

    private bool IsFirstPropertyInGroup(SerializedProperty property)
    {
        string groupName = ((GroupAttribute)attribute).groupName;

        // Получаем все SerializedProperty этого объекта
        SerializedProperty iterator = property.serializedObject.GetIterator();
        iterator.NextVisible(true); // Перемещаемся к первому видимому полю

        while (iterator.NextVisible(false))
        {
            if (iterator.propertyPath == property.propertyPath)
            {
                return true;
            }

            var groupAttribute = GetGroupAttribute(iterator);
            if (groupAttribute != null && groupAttribute.groupName == groupName)
            {
                return false;
            }
        }

        return true;
    }

    private GroupAttribute GetGroupAttribute(SerializedProperty property)
    {
        Type targetType = property.serializedObject.targetObject.GetType();
        FieldInfo field = targetType.GetField(property.name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
        return field != null ? field.GetCustomAttribute<GroupAttribute>() : null;
    }
}
#endif