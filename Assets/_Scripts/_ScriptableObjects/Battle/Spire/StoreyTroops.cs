using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Battle/Spire/Storey Troops")]
public class StoreyTroops : Storey
{
    public StoreyUpgradeTree CreatureTierTree;
    public StoreyUpgradeTree MaxTroopsTree;

    public override void Initialize()
    {
        CreatureTierTree.Initialize();
        MaxTroopsTree.Initialize();

        base.Initialize();
    }
}
