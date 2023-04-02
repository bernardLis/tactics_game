using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Map/ArmyGroup")]
public class ArmyGroup : BaseScriptableObject
{
    public ArmyEntity ArmyEntity;
    public int EntityCount;
    public int ListPosition; // used for save/load of castle and character army

    public event Action<int, int> OnCountChanged;
    public void ChangeCount(int change)
    {
        EntityCount += change;
        OnCountChanged?.Invoke(ListPosition, EntityCount);
    }

    public ArmyGroupData SerializeSelf()
    {
        ArmyGroupData data = new();
        data.EntityId = ArmyEntity.Id;
        data.EntityCount = EntityCount;
        data.ListPosition = ListPosition;
        return data;
    }

    public void LoadFromData(ArmyGroupData data)
    {
        ArmyEntity = GameManager.Instance.GameDatabase.GetArmyEntityById(data.EntityId);
        EntityCount = data.EntityCount;
        ListPosition = data.ListPosition;
    }
}

[System.Serializable]
public struct ArmyGroupData
{
    public string EntityId;
    public int EntityCount;
    public int ListPosition;
}

