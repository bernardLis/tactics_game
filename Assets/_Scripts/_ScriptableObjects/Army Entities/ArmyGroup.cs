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
    public int OldKillCount;
    public int KillCount;
    public int ListPosition; // used for save/load of castle and hero army

    public event Action<ArmyEntity> OnEvolved;

    public event Action<int, int> OnCountChanged;
    public void ChangeCount(int change)
    {
        EntityCount += change;
        OnCountChanged?.Invoke(ListPosition, EntityCount);
    }

    public void InitializeBattle()
    {
        OldKillCount = KillCount;
    }

    public void AddKill(int ignored)
    {
        KillCount++;
    }

    public int NumberOfKillsToEvolve()
    {
        return ArmyEntity.KillsToUpgrade * EntityCount;
    }

    public bool ShouldEvolve()
    {
        if (ArmyEntity.UpgradedEntity == null) return false;

        return KillCount >= NumberOfKillsToEvolve();
    }

    public void Evolve()
    {
        KillCount -= NumberOfKillsToEvolve();

        ArmyEntity = ArmyEntity.UpgradedEntity;
        OnEvolved?.Invoke(ArmyEntity);
    }

    public ArmyGroupData SerializeSelf()
    {
        ArmyGroupData data = new();
        data.Name = Name;
        data.EntityId = ArmyEntity.Id;
        data.EntityCount = EntityCount;
        data.KillCount = KillCount;
        data.ListPosition = ListPosition;
        return data;
    }

    public void LoadFromData(ArmyGroupData data)
    {
        ArmyEntity = GameManager.Instance.HeroDatabase.GetArmyEntityById(data.EntityId);
        Name = data.Name;
        EntityCount = data.EntityCount;
        KillCount = data.KillCount;
        ListPosition = data.ListPosition;
    }
}

[System.Serializable]
public struct ArmyGroupData
{
    public string Name;
    public string EntityId;
    public int EntityCount;
    public int KillCount;
    public int ListPosition;
}

