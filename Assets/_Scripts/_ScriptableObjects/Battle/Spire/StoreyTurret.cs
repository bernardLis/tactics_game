using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Battle/Spire/Storey Turret")]
public class StoreyTurret : Storey
{
    public Element Element;
    public StoreyUpgradeTree TurretUpgradeTree;

    [SerializeField] StoreyUpgrade SpecialUpgradeOriginal;
    [HideInInspector] public StoreyUpgrade SpecialUpgrade;

    public override void Initialize()
    {
        TurretUpgradeTree.Initialize();
        SpecialUpgrade = Instantiate(SpecialUpgradeOriginal);

        base.Initialize();
    }
}
