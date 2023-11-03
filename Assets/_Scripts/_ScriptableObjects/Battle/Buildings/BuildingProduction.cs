using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Battle/Building Production")]
public class BuildingProduction : Building
{
    public IntVariable CurrentLevel;

    public BuildingUpgrade[] BuildingUpgrades;

    public event Action OnUpgradePurchased;

    public override void Initialize()
    {
        CurrentLevel = CreateInstance<IntVariable>();
        CurrentLevel.SetValue(1);
    }

    public BuildingUpgrade GetCurrentUpgrade()
    {
        return BuildingUpgrades[CurrentLevel.Value - 1];
    }

    public BuildingUpgrade GetNextUpgrade()
    {
        if (CurrentLevel.Value == BuildingUpgrades.Length) return null;
        return BuildingUpgrades[CurrentLevel.Value];
    }

    public void Upgrade()
    {
        if (CurrentLevel.Value == BuildingUpgrades.Length) return;

        CurrentLevel.ApplyChange(1);
        OnUpgradePurchased?.Invoke();
    }

}
