using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Battle/Spire/Storey Turrets")]
public class StoreyTurrets : Storey
{
    public StoreyUpgradeTree EarthTurretUpgradeTree;
    public StoreyUpgradeTree FireTurretUpgradeTree;
    public StoreyUpgradeTree WaterTurretUpgradeTree;
    public StoreyUpgradeTree WindTurretUpgradeTree;

    public BattleTurret EarthTurretPrefab;
    public BattleTurret FireTurretPrefab;
    public BattleTurret WaterTurretPrefab;
    public BattleTurret WindTurretPrefab;

    public override void Initialize()
    {
        EarthTurretUpgradeTree.Initialize();
        FireTurretUpgradeTree.Initialize();
        WaterTurretUpgradeTree.Initialize();
        WindTurretUpgradeTree.Initialize();

        base.Initialize();
    }
}
