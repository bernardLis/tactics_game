using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityFight : EntityMovement
{
    [Header("Fight")]
    public IntVariable BasePower;
    public IntVariable BaseAttackRange; // stopping distance of agent
    public IntVariable BaseAttackCooldown;

    public Stat Power { get; protected set; }
    public Stat AttackRange { get; protected set; }
    public Stat AttackCooldown { get; protected set; }

    public Vector2Int PowerGrowthPerLevel;
    public Vector2Int AttackRangeGrowthPerLevel;
    public Vector2Int AttackCooldownGrowthPerLevel;

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

    public void AddKill() { TotalKillCount++; }
    public void AddDmgDealt(int dmg) { TotalDamageDealt += dmg; }

    protected override void CreateStats()
    {
        base.CreateStats();

        BasePower = Instantiate(BasePower);
        Power = CreateInstance<Stat>();
        Power.StatType = StatType.Power;
        Power.SetBaseValue(BasePower.Value);
        BasePower.OnValueChanged += Power.SetBaseValue;

        BaseAttackRange = Instantiate(BaseAttackRange);
        AttackRange = CreateInstance<Stat>();
        AttackRange.StatType = StatType.AttackRange;
        AttackRange.SetBaseValue(BaseAttackRange.Value);
        BaseAttackRange.OnValueChanged += AttackRange.SetBaseValue;

        BaseAttackCooldown = Instantiate(BaseAttackCooldown);
        AttackCooldown = CreateInstance<Stat>();
        AttackCooldown.StatType = StatType.AttackCooldown;
        AttackCooldown.SetBaseValue(BaseAttackCooldown.Value);
        BaseAttackCooldown.OnValueChanged += AttackCooldown.SetBaseValue;
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

