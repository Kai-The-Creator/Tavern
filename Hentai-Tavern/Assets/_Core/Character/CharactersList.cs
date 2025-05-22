using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharactersList", menuName = "GAME/Characters/CharactersList")]
public class CharactersList : ScriptableObject
{
    [field: SerializeField] public List<CharacterConfig> list { get; private set; } = new();

    public CharacterConfig GetCharacter(Character tag) => list.Find(c => c.tag == tag);
}
