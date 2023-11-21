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


    // save
    // load
    // serialize
}


