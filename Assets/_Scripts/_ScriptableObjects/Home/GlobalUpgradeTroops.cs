using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Battle/Home/Global Upgrade Troops")]
public class GlobalUpgradeTroops : GlobalUpgrade
{
    public GlobalUpgradeTree CreatureTierTree;
    public GlobalUpgradeTree MaxTroopsTree;

    public override void Initialize()
    {
        CreatureTierTree.Initialize();
        MaxTroopsTree.Initialize();

        base.Initialize();
    }
}
