using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(menuName = "ScriptableObject/Battle/Creature")]
public class Creature : EntityFight
{
    [Header("Creature")]
    public int UpgradeTier;

    public CreatureAbility CreatureAbility;

    public GameObject Projectile;
    public GameObject HitPrefab;

    [Header("Upgrade")]
    public Creature EvolvedCreature;

    public override void InitializeBattle(int team)
    {
        base.InitializeBattle(team);

        if (EntityName.Length == 0) EntityName = Helpers.ParseScriptableObjectName(name);
    }

    public void ImportCreatureStats(Creature c)
    {
        EntityName = c.EntityName;
        Level = c.Level;
        Experience.SetValue(c.Experience.Value);

        TotalKillCount = c.TotalKillCount;
        TotalDamageDealt = c.TotalDamageDealt;
        TotalDamageTaken = c.TotalDamageTaken;
        OldDamageDealt = c.OldDamageDealt;
        OldDamageTaken = c.OldDamageTaken;
        OldKillCount = c.OldKillCount;
    }

    public override int GetExpForNextLevel()
    {
        // TODO: math
        float exponent = 3f;
        float multiplier = 0.4f;
        int baseExp = 50;

        int result = Mathf.FloorToInt(multiplier * Mathf.Pow(Level.Value, exponent));
        result = Mathf.RoundToInt(result * 0.1f) * 10; // rounding to tens
        return result + baseExp;
    }

    public bool IsAbilityUnlocked() { return Level.Value >= CreatureAbility.UnlockLevel; }

    public bool CanUseAbility()
    {
        if (CreatureAbility == null) return false;
        return IsAbilityUnlocked();
    }

    public bool ShouldEvolve()
    {
        return Random.value < ChanceToEvolve(Level.Value);
    }

    public float ChanceToEvolve(int level)
    {
        if (EvolvedCreature == null) return 0;
        // TODO: math, and also tier 1 +10 levels
        float chance = 0.1f * ((level - 10) * 1.5f);
        if (chance < 0) return 0;
        return chance;
    }

    new public CreatureData SerializeSelf()
    {
        // TODO: needs to be implemented
        CreatureData data = new()
        {
            CreatureId = Id,

            Name = EntityName,
            Level = Level.Value,

            KillCount = TotalKillCount,
            DamageDealt = TotalDamageDealt,
            DamageTaken = TotalDamageTaken
        };

        return data;
    }

    public void LoadFromData(CreatureData data)
    {
        EntityName = data.Name;

        Level = CreateInstance<IntVariable>();
        Level.SetValue(data.Level);

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