using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Character/Character Rank")]
public class CharacterRank : BaseScriptableObject
{
    public int Rank;
    public string Title;
    public Sprite PortraitBorder;
    public int PointsRequired;
}
