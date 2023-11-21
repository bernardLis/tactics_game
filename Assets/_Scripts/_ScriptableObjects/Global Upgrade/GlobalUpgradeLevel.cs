using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Global Upgrades/Global Upgrade Level")]
public class GlobalUpgradeLevel : BaseScriptableObject
{
    public Sprite Icon;
    public int Cost;
    public int Value;
    public bool IsPurchased;
    public string Description;

}
