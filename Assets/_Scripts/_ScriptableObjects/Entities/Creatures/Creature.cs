using System;
using System.Collections;
using System.Collections.Generic;
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


    public override void InitializeBattle(int team)
    {
        base.InitializeBattle(team);

        if (EntityName.Length == 0) EntityName = Helpers.ParseScriptableObjectName(name);
    }

    public bool IsAbilityUnlocked() { return Level.Value >= CreatureAbility.UnlockLevel; }

    public bool CanUseAbility()
    {
        if (CreatureAbility == null) return false;
        return IsAbilityUnlocked();
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

[Serializable]
public struct CreatureData
{
    public string Name;
    public int Level;
    public string CreatureId;

    public int KillCount;
    public int DamageDealt;
    public int DamageTaken;
}