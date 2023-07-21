using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Battle/Turret")]
public class Turret : BaseScriptableObject
{
    public Sprite Icon;
    public string Description;
    public Element Element;

    public TurretUpgrade[] TurretUpgrades;
    public int CurrentTurretUpgradeIndex;
}


