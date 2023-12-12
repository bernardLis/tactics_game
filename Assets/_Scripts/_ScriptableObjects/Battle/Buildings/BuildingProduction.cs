using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Battle/Building Production")]
public class BuildingProduction : Building
{
    public Creature ProducedCreature;

    public UpgradeLevelBuilding GetCurrentUpgrade()
    {
        return BuildingUpgrade.GetCurrentLevel() as UpgradeLevelBuilding;
    }
}
