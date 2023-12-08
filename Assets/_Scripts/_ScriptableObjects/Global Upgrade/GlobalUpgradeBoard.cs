using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Global Upgrades/Global Upgrade Board")]
public class GlobalUpgradeBoard : BaseScriptableObject
{

    [SerializeField] List<GlobalUpgrade> _globalUpgradeOriginals = new();
    public List<GlobalUpgrade> GlobalUpgrades = new();


    [Header("Hero")]
    public GlobalUpgrade HeroSpeed;
    public GlobalUpgrade HeroArmor;
    public GlobalUpgrade HeroHealth;
    public GlobalUpgrade HeroPull;
    public GlobalUpgrade HeroPower;
    public GlobalUpgrade HeroSprint;
    public GlobalUpgrade HeroGrab;


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
    public GlobalUpgrade BossSlowdown;

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

    public GlobalUpgrade GetUpgradeByName(string name)
    {
        // TODO: this is a bad idea
        foreach (GlobalUpgrade upgrade in GlobalUpgrades)
            if (upgrade.name == name)
                return upgrade;
        return null;
    }

    public List<GlobalUpgrade> GetGlobalUpgradesByType(GlobalUpgradeType type)
    {
        List<GlobalUpgrade> upgrades = new();
        foreach (GlobalUpgrade upgrade in GlobalUpgrades)
            if (upgrade.Type == type)
                upgrades.Add(upgrade);
        return upgrades;
    }

    public void RefundAll() { OnRefundAll?.Invoke(); }

    public void Reset()
    {
        GlobalUpgrades = new();
        foreach (GlobalUpgrade original in _globalUpgradeOriginals)
        {
            GlobalUpgrade instance = Instantiate(original);
            instance.name = original.name;
            GlobalUpgrades.Add(instance);
        }
    }

    public GlobalUpgradeBoardData SerializeSelf()
    {
        List<GlobalUpgradeData> GlobalUpgradeDatas = new();
        foreach (GlobalUpgrade upgrade in GlobalUpgrades)
            GlobalUpgradeDatas.Add(upgrade.SerializeSelf());

        GlobalUpgradeBoardData data = new()
        {
            GlobalUpgradeDatas = GlobalUpgradeDatas
        };

        return data;
    }

    public void LoadFromData(GlobalUpgradeBoardData data)
    {
        GlobalUpgrades = new();
        foreach (GlobalUpgrade original in _globalUpgradeOriginals)
        {
            GlobalUpgradeData upgradeData = data.GlobalUpgradeDatas.Find(u => u.Name == original.name);
            GlobalUpgrade instance = Instantiate(original);
            instance.name = original.name;
            if (upgradeData.Name != null) instance.LoadFromData(upgradeData);
            GlobalUpgrades.Add(instance);
        }
    }
}

[Serializable]
public struct GlobalUpgradeBoardData
{
    public List<GlobalUpgradeData> GlobalUpgradeDatas;
}