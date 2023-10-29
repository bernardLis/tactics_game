using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityFight : EntityMovement
{
    [Header("Fight")]
    public Stat Power;
    public Stat AttackRange;
    public Stat AttackCooldown;

    [HideInInspector] public int OldKillCount;
    [HideInInspector] public int TotalKillCount;
    [HideInInspector] public int OldDamageDealt;
    [HideInInspector] public int TotalDamageDealt;

    public override void InitializeBattle(int team)
    {
        base.InitializeBattle(team);
        OldKillCount = TotalKillCount;
        OldDamageDealt = TotalDamageDealt;
    }

    public void AddKill(Entity entity)
    {
        AddExp(entity.Price);
        TotalKillCount++;
    }
    public void AddDmgDealt(int dmg) { TotalDamageDealt += dmg; }

    protected override void CreateStats()
    {
        base.CreateStats();

        Power = Instantiate(Power);
        AttackRange = Instantiate(AttackRange);
        AttackCooldown = Instantiate(AttackCooldown);

        Power.Initialize();
        AttackRange.Initialize();
        AttackCooldown.Initialize();

        OnLevelUp += Power.LevelUp;
        OnLevelUp += AttackRange.LevelUp;
        OnLevelUp += AttackCooldown.LevelUp;
    }

    /* SERIALIZATION */
    new public EntityFightData SerializeSelf()
    {
        // TODO: to be implemented
        EntityFightData data = new()
        {
            EntityMovementData = base.SerializeSelf(),

        };

        return data;
    }

    public void LoadFromData(EntityFightData data)
    {
        // TODO: to be implemented

    }
}

[Serializable]
public struct EntityFightData
{
    public EntityMovementData EntityMovementData;

    public int Power;
    public int AttackRange;
    public int AttackCooldown;
}

