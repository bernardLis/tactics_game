using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Battle/Spire/Storey Upgrade")]
public class StoreyUpgrade : BaseScriptableObject
{
    public Sprite Icon;
    public int Cost;
    public int Value;
    public bool IsPurchased;
    public string Description;
    public bool IsInfinite;
    public float CircleRadiusMultiplier;

}
