using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Upgrades/Upgrade Level")]
public class UpgradeLevel : BaseScriptableObject
{
    public int Cost;
    public int Value;
    public string Description;

}
