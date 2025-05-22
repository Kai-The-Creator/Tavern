using UnityEngine;


[CreateAssetMenu(fileName="WidgetConfig", menuName="GAME/Widget/ImprovedWidgetConfig")]
public class ImprovedWidgetConfig : ScriptableObject
{
    public Widget widgetPrefab;  
    public Sprite iconSprite;
    public string labelText;
    public OpenBuildAction openAction;

    [Header("Animations")]
    public bool enableHoverAnimation = true;
    public float hoverScale = 1.1f;

    public bool enablePressAnimation = true;
    public float pressScale = 0.9f;
}