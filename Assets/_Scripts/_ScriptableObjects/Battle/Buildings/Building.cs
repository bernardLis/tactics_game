using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Battle/Building")]
public class Building : BaseScriptableObject
{
    public IntVariable CurrentLevel;

    public BuildingUpgrade[] BuildingUpgrades;

    public bool IsSecured;

    public event Action OnUpgradePurchased;
    public event Action OnSecured;

    public void Initialize()
    {
        CurrentLevel = CreateInstance<IntVariable>();
        CurrentLevel.SetValue(1);
    }

    public void Secure()
    {
        IsSecured = true;
        OnSecured?.Invoke();
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
