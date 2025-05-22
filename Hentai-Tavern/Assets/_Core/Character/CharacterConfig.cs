using Spine;
using Spine.Unity;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterConfig", menuName = "GAME/Characters/CharacterConfig")]
public class CharacterConfig : CSVLoadConfig
{
    public Character tag;
    public Sprite characterView;
    public SkeletonDataAsset spineView;
    public List<StringLang> characterName = new();
}
