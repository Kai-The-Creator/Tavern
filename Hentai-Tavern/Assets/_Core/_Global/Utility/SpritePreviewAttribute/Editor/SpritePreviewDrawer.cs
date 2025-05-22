#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(SpritePreviewAttribute))]
public class SpritePreviewDrawer : PropertyDrawer
{
    private const float previewSize = 100f;
    private const float padding = 4f;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        if (property.propertyType == SerializedPropertyType.ObjectReference && property.objectReferenceValue is Sprite)
        {
            // Draw label
            Rect labelRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(labelRect, label);

            Rect objectFieldRect = new Rect(position.x + position.width - previewSize, position.y + EditorGUIUtility.singleLineHeight + padding, previewSize, previewSize);
            property.objectReferenceValue = EditorGUI.ObjectField(objectFieldRect, property.objectReferenceValue, typeof(Sprite), false);
        }
        else if (property.propertyType == SerializedPropertyType.ObjectReference)
        {
            Rect objectFieldRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            EditorGUI.ObjectField(objectFieldRect, property, label);
        }
        else
        {
            EditorGUI.LabelField(position, label.text, "Use SpritePreview with Sprite.");
        }

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (property.propertyType == SerializedPropertyType.ObjectReference && property.objectReferenceValue is Sprite)
        {
            return EditorGUIUtility.singleLineHeight + previewSize + padding;
        }
        else
        {
            return EditorGUIUtility.singleLineHeight;
        }
    }
}
#endif
