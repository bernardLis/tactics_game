using System;
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

    public int KillCount;

    public GameObject Prefab;

    public event Action OnTurretUpgradePurchased;
    public void PurchaseUpgrade()
    {
        CurrentTurretUpgradeIndex++;
        OnTurretUpgradePurchased?.Invoke();
    }

    public TurretUpgrade GetCurrentUpgrade()
    {
        return TurretUpgrades[CurrentTurretUpgradeIndex];
    }

    public TurretUpgrade GetNextUpgrade()
    {
        if (CurrentTurretUpgradeIndex + 1 < TurretUpgrades.Length)
            return TurretUpgrades[CurrentTurretUpgradeIndex + 1];

        return null;
    }

    public TurretUpgrade GetNextNextUpgrade()
    {
        if (CurrentTurretUpgradeIndex + 2 < TurretUpgrades.Length)
            return TurretUpgrades[CurrentTurretUpgradeIndex + 2];

        return null;
    }

    public void PurchaseSpecialUpgrade()
    {
        //HERE: turret special upgrade
    }

    public void IncreaseKillCount()
    {
        KillCount++;
    }
}


