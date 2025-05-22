#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(RandomRangeAttribute))]
public class RandomRangeDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        RandomRangeAttribute range = (RandomRangeAttribute)attribute;

        EditorGUI.BeginProperty(position, label, property);

        // Draw label
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        // Calculate positions for the fields and slider
        Rect minFieldRect = new Rect(position.x, position.y, 30, 20);
        Rect maxFieldRect = new Rect(position.x + position.width - 30, position.y, 30, 20);
        Rect sliderRect = new Rect(position.x, position.y + 25, position.width, position.height);

        if (property.type == "FloatRange")
        {
            SerializedProperty minValueProp = property.FindPropertyRelative("minValue");
            SerializedProperty maxValueProp = property.FindPropertyRelative("maxValue");

            float minValue = minValueProp.floatValue;
            float maxValue = maxValueProp.floatValue;

            EditorGUI.MinMaxSlider(sliderRect, ref minValue, ref maxValue, range.minLimit, range.maxLimit);
            minValue = EditorGUI.FloatField(minFieldRect, minValue);
            maxValue = EditorGUI.FloatField(maxFieldRect, maxValue);


            minValueProp.floatValue = Mathf.Clamp(minValue, range.minLimit, maxValue);
            maxValueProp.floatValue = Mathf.Clamp(maxValue, minValue, range.maxLimit);
        }
        else if (property.type == "IntRange")
        {
            SerializedProperty minValueProp = property.FindPropertyRelative("minValue");
            SerializedProperty maxValueProp = property.FindPropertyRelative("maxValue");

            int minValue = minValueProp.intValue;
            int maxValue = maxValueProp.intValue;

            minValue = EditorGUI.IntField(minFieldRect, minValue);
            maxValue = EditorGUI.IntField(maxFieldRect, maxValue);

            float minSliderValue = minValue;
            float maxSliderValue = maxValue;

            EditorGUI.MinMaxSlider(sliderRect, ref minSliderValue, ref maxSliderValue, range.minLimit, range.maxLimit);

            minValueProp.intValue = Mathf.Clamp((int)minSliderValue, (int)range.minLimit, maxValue);
            maxValueProp.intValue = Mathf.Clamp((int)maxSliderValue, minValue, (int)range.maxLimit);
        }
        else
        {
            EditorGUI.LabelField(position, label.text, "Use RandomRange with FloatRange or IntRange.");
        }

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return base.GetPropertyHeight(property, label) + EditorGUIUtility.singleLineHeight * 2 + 6;
    }
}
#endif
