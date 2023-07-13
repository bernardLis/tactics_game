using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Battle/Base/Upgrade Level")]
public class BaseUpgradeLevel : BaseScriptableObject
{
    public int Cost;
    public int Value;
    public bool IsInfinite;
    public float CircleRadiusMultiplier;

}
