using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Battle/Home/Building Upgrade")]
public class BuildingUpgrade : BaseScriptableObject
{
    public Sprite Icon;
    public int Cost;
    public int Value;
    public bool IsPurchased;
    public string Description;
    public bool IsInfinite;
    public float CircleRadiusMultiplier;

}
