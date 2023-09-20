using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Battle/Home/Building Troops")]
public class BuildingTroops : Building
{
    public BuildingUpgradeTree CreatureTierTree;
    public BuildingUpgradeTree MaxTroopsTree;

    public override void Initialize()
    {
        CreatureTierTree.Initialize();
        MaxTroopsTree.Initialize();

        base.Initialize();
    }
}
