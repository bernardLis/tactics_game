using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Map/ArmyGroup")]
public class ArmyGroup : BaseScriptableObject
{
    public string Name;
    public ArmyEntity ArmyEntity;
    public int EntityCount;
    public int ListPosition; // used for save/load of castle and hero army

    public int OldKillCount;
    public int TotalKillCount;

    public int OldDamageDealt;
    public int TotalDamageDealt;

    public int OldDamageTaken;
    public int TotalDamageTaken;

    public ArmyEntity PreviousEntity;

    public event Action<ArmyEntity> OnEvolved;

    public event Action<int, int> OnCountChanged;
    public void ChangeCount(int change)
    {
        EntityCount += change;
        OnCountChanged?.Invoke(ListPosition, EntityCount);
    }

    public void InitializeBattle()
    {
        OldKillCount = TotalKillCount;
        OldDamageDealt = TotalDamageDealt;
        OldDamageTaken = TotalDamageTaken;
    }

    public void AddKill(int ignored)
    {
        TotalKillCount++;
    }

    public void AddDmgDealt(int dmg)
    {
        TotalDamageDealt += dmg;
    }

    public void AddDmgTaken(int dmg)
    {
        TotalDamageTaken += dmg;
    }

    public int NumberOfKillsToEvolve()
    {
        return ArmyEntity.KillsToUpgrade * EntityCount;
    }

    public bool ShouldEvolve()
    {
        if (ArmyEntity.UpgradedEntity == null) return false;

        return TotalKillCount >= NumberOfKillsToEvolve();
    }

    public void Evolve()
    {
        TotalKillCount -= NumberOfKillsToEvolve();
        PreviousEntity = ArmyEntity;
        ArmyEntity = ArmyEntity.UpgradedEntity;
        OnEvolved?.Invoke(ArmyEntity);
    }

    public ArmyGroupData SerializeSelf()
    {
        ArmyGroupData data = new();
        data.Name = Name;
        data.EntityId = ArmyEntity.Id;
        data.EntityCount = EntityCount;
        data.ListPosition = ListPosition;

        data.KillCount = TotalKillCount;
        data.DamageDealt = TotalDamageDealt;
        data.DamageTaken = TotalDamageTaken;

        return data;
    }

    public void LoadFromData(ArmyGroupData data)
    {
        ArmyEntity = GameManager.Instance.HeroDatabase.GetArmyEntityById(data.EntityId);
        Name = data.Name;
        EntityCount = data.EntityCount;
        ListPosition = data.ListPosition;

        TotalKillCount = data.KillCount;
        TotalDamageDealt = data.DamageDealt;
        TotalDamageTaken = data.DamageTaken;
    }
}

[System.Serializable]
public struct ArmyGroupData
{
    public string Name;
    public string EntityId;
    public int EntityCount;
    public int ListPosition;

    public int KillCount;
    public int DamageDealt;
    public int DamageTaken;


}

