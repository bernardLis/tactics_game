using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Global Upgrades/Global Upgrade Board")]
public class GlobalUpgradeBoard : BaseScriptableObject
{
    [Header("Hero")]
    public GlobalUpgrade HeroSpeed;
    public GlobalUpgrade HeroArmor;
    public GlobalUpgrade HeroHealth;
    public GlobalUpgrade HeroPull;
    public GlobalUpgrade HeroPower;

    [Header("Buildings")]
    public List<GlobalUpgrade> BuildingUpgrades = new();

    [Header("Creatures")]
    public GlobalUpgrade CreatureStartingLevel;
    public GlobalUpgrade CreatureSpeed;
    public GlobalUpgrade CreatureArmor;
    public GlobalUpgrade CreatureHealth;
    public GlobalUpgrade CreaturePower;

    [Header("Boss")]
    public GlobalUpgrade BossCorruptionBreakNodes;
    public GlobalUpgrade BossStunDuration;
    public GlobalUpgrade BossCorruptionDuration;
    public GlobalUpgrade BossSpeed;

    [Header("Other")]
    public GlobalUpgrade TileIndicator;
    public GlobalUpgrade RewardNumber;
    public GlobalUpgrade RewardReroll;

    public event Action OnRefundAll;
    public void Initialize()
    {
    }

    public GlobalUpgrade GetBuildingUpgradeByName(string name)
    {
        // TODO: this is a bad idea
        foreach (GlobalUpgrade upgrade in BuildingUpgrades)
            if (upgrade.name == name)
                return upgrade;
        return null;
    }


    public void RefundAll() { OnRefundAll?.Invoke(); }
    // save
    // load
    // serialize
}


