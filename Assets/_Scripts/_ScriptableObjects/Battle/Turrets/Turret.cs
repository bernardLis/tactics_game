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

    public void PurchaseSpecialUpgrade()
    {
        //HERE: turret special ugprade
    }

    public void IncreaseKillCount()
    {
        KillCount++;
    }
}


