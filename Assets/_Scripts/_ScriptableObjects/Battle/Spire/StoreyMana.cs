using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Battle/Spire/Storey Mana")]
public class StoreyMana : Storey
{
    public IntVariable ManaInBank;

    public StoreyUpgradeTree ManaBankCapacityTree;
    public StoreyUpgradeTree ManaPerTurnTree;
    public StoreyUpgradeTree ManaTurnLengthTree;

    public override void Initialize()
    {
        ManaInBank = ScriptableObject.CreateInstance<IntVariable>();
        ManaInBank.SetValue(0);

        ManaBankCapacityTree.Initialize();
        ManaPerTurnTree.Initialize();
        ManaTurnLengthTree.Initialize();

        base.Initialize();
    }
}
