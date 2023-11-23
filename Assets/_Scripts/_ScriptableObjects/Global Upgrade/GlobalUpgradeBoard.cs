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
    public GlobalUpgrade HeroPower;

    [Header("Buildings")]
    public GlobalUpgrade HomeCrystal;
    public GlobalUpgrade WolfLair;

    public GlobalUpgrade BombLair;
    public GlobalUpgrade BudLair;
    public GlobalUpgrade DragonSparkLair;
    public GlobalUpgrade MetalonLair;
    public GlobalUpgrade PracticeDummyLair;
    public GlobalUpgrade ShellLair;
    public GlobalUpgrade SnakeletLair;
    public GlobalUpgrade SunBlossomLair;

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

    public List<GlobalUpgrade> BuildingUpgrades = new();

    public void Initialize()
    {
        BuildingUpgrades.Add(HomeCrystal);
        BuildingUpgrades.Add(WolfLair);
        BuildingUpgrades.Add(BombLair);
        BuildingUpgrades.Add(BudLair);
        BuildingUpgrades.Add(DragonSparkLair);
        BuildingUpgrades.Add(MetalonLair);
        BuildingUpgrades.Add(PracticeDummyLair);
        BuildingUpgrades.Add(ShellLair);
        BuildingUpgrades.Add(SnakeletLair);
        BuildingUpgrades.Add(SunBlossomLair);
    }

    public GlobalUpgrade GetBuildingUpgradeByName(string name)
    {
        foreach (GlobalUpgrade upgrade in BuildingUpgrades)
            if (upgrade.name == name)
                return upgrade;
        return null;
    }

    public event Action OnRefundAll;
    public void RefundAll()
    {
        OnRefundAll?.Invoke();

        // // oh kill me...
        // if (HeroSpeed != null) HeroSpeed.Refund();
        // if (HeroArmor != null) HeroArmor.Refund();
        // if (HeroHealth != null) HeroHealth.Refund();
        // if (HeroPower != null) HeroPower.Refund();
        // if (HomeCrystal != null) HomeCrystal.Refund();
        // if (WolfLair != null) WolfLair.Refund();
        // if (BombLair != null) BombLair.Refund();
        // if (BudLair != null) BudLair.Refund();
        // if (DragonSparkLair != null) DragonSparkLair.Refund();
        // if (MetalonLair != null) MetalonLair.Refund();
        // if (PracticeDummyLair != null) PracticeDummyLair.Refund();
        // if (ShellLair != null) ShellLair.Refund();
        // if (SnakeletLair != null) SnakeletLair.Refund();
        // if (SunBlossomLair != null) SunBlossomLair.Refund();
        // if (CreatureStartingLevel != null) CreatureStartingLevel.Refund();
        // if (CreatureSpeed != null) CreatureSpeed.Refund();
        // if (CreatureArmor != null) CreatureArmor.Refund();
        // if (CreatureHealth != null) CreatureHealth.Refund();
        // if (CreaturePower != null) CreaturePower.Refund();
        // if (BossCorruptionBreakNodes != null) BossCorruptionBreakNodes.Refund();
        // if (BossStunDuration != null) BossStunDuration.Refund();
        // if (BossCorruptionDuration != null) BossCorruptionDuration.Refund();
        // if (BossSpeed != null) BossSpeed.Refund();
        // if (TileIndicator != null) TileIndicator.Refund();
        // if (RewardNumber != null) RewardNumber.Refund();
        // if (RewardReroll != null) RewardReroll.Refund();
    }
    // save
    // load
    // serialize
}


