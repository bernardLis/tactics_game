using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(menuName = "ScriptableObject/Battle/Creature")]
public class Creature : Entity
{
    public int UpgradeTier;
    public float BasePower;
    public float AttackRange; // stopping distance of agent
    public float AttackCooldown;

    public CreatureAbility CreatureAbility;

    public GameObject Projectile;
    public GameObject HitPrefab;

    [Header("Upgrade")]
    public Creature EvolvedCreature;

    // exp
    public IntVariable Experience;
    public IntVariable ExpForNextLevel;
    public int LeftoverExp;

    // battle
    [HideInInspector] public int OldKillCount;
    [HideInInspector] public int TotalKillCount;

    [HideInInspector] public int OldDamageDealt;
    [HideInInspector] public int TotalDamageDealt;

    [HideInInspector] public int OldDamageTaken;
    [HideInInspector] public int TotalDamageTaken;

    public event Action OnLevelUp;
    public override void InitializeBattle(Hero hero)
    {
        base.InitializeBattle(hero);

        Experience = ScriptableObject.CreateInstance<IntVariable>();
        Experience.SetValue(0);

        ExpForNextLevel = ScriptableObject.CreateInstance<IntVariable>();
        ExpForNextLevel.SetValue(GetExpForNextLevel());

        OldKillCount = TotalKillCount;
        OldDamageDealt = TotalDamageDealt;
        OldDamageTaken = TotalDamageTaken;

        // HERE: b modifier rework
        Battle b = GameManager.Instance.SelectedBattle;

        Speed *= b.CreatureSpeedMultiplier;
        _elementalDamageMultiplier = b.ElementalDamageMultiplier;
        if (CreatureAbility != null)
        {
            CreatureAbility = Instantiate(CreatureAbility);
            CreatureAbility.Cooldown = Mathf.FloorToInt(CreatureAbility.Cooldown * b.CreatureAbilityCooldown);
        }
    }

    public void ImportCreatureStats(Creature c)
    {
        Level = c.Level;
        Experience.SetValue(c.Experience.Value);

        TotalKillCount = c.TotalKillCount;
        TotalDamageDealt = c.TotalDamageDealt;
        TotalDamageTaken = c.TotalDamageTaken;
        OldDamageDealt = c.OldDamageDealt;
        OldDamageTaken = c.OldDamageTaken;
        OldKillCount = c.OldKillCount;
    }

    public int GetPower() { return Mathf.RoundToInt(BasePower + 0.1f * BasePower * (Level - 1)); }

    public void AddKill(int ignored) { TotalKillCount++; }
    public void AddDmgDealt(int dmg) { TotalDamageDealt += dmg; }
    public void AddDmgTaken(int dmg) { TotalDamageTaken += dmg; }

    public void AddExp(int gain)
    {
        LeftoverExp = gain;
        if (Experience.Value + gain >= ExpForNextLevel.Value)
        {
            LeftoverExp = Experience.Value + gain - ExpForNextLevel.Value;
            LevelUp();
        }

        Experience.ApplyChange(LeftoverExp);
        LeftoverExp = 0;
    }

    public int GetExpForNextLevel()
    {
        // TODO: math
        float exponent = 3f;
        float multiplier = 0.8f;
        int baseExp = 100;

        int result = Mathf.FloorToInt((multiplier * Mathf.Pow(Level, exponent)));
        result = Mathf.RoundToInt(result * 0.1f) * 10; // rounding to tens
        return result + baseExp;
    }

    public void LevelUp()
    {
        Level++;
        Experience.SetValue(0);
        ExpForNextLevel.SetValue(GetExpForNextLevel());

        MaxHealth.SetValue(GetMaxHealth());

        OnLevelUp?.Invoke();
    }

    public bool IsAbilityUnlocked() { return Level >= CreatureAbility.UnlockLevel; }

    public bool ShouldEvolve()
    {
        return Random.value < ChanceToEvolve(Level);
    }

    public float ChanceToEvolve(int level)
    {
        if (EvolvedCreature == null) return 0;
        // TODO: math, and also tier 1 +10 levels
        float chance = 0.1f * ((level - 10) * 1.5f);
        if (chance < 0) return 0;
        return chance;
    }

    public CreatureData SerializeSelf()
    {
        CreatureData data = new();
        data.CreatureId = Id;

        data.Name = Name;
        data.Level = Level;

        data.KillCount = TotalKillCount;
        data.DamageDealt = TotalDamageDealt;
        data.DamageTaken = TotalDamageTaken;

        return data;
    }

    public void LoadFromData(CreatureData data)
    {
        Name = data.Name;
        Level = data.Level;

        TotalKillCount = data.KillCount;
        TotalDamageDealt = data.DamageDealt;
        TotalDamageTaken = data.DamageTaken;
    }
}

[System.Serializable]
public struct CreatureData
{
    public string Name;
    public int Level;
    public string CreatureId;

    public int KillCount;
    public int DamageDealt;
    public int DamageTaken;
}