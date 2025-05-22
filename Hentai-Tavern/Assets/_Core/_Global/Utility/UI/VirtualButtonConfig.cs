using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "VirtualButtonConfig", menuName = "GameEngine/UI/VirtualButtonConfig")]
public class VirtualButtonConfig : ScriptableObject
{
    public bool isDisabled;
    public bool emptySpriteMode;

    public Sprite DefaultImage;
    public Sprite HowerImage;
    public Sprite PressedImage;
    public Sprite DisabledImage;

    public bool IsScaleble;
    public float HowerScale;
    public float PressedScale;

    public AudioClip clickSound;
}
