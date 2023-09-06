using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Hero/Hero Rank")]
public class HeroRank : BaseScriptableObject
{
    public int Rank;
    public string Title;
    public Sprite PortraitBorder;
    public int PointsRequired;
}
