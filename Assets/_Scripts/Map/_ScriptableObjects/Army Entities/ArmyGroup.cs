using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Map/ArmyGroup")]
public class ArmyGroup : BaseScriptableObject
{
    public ArmyEntity ArmyEntity;
    public int EntityCount;

    public event Action<int> OnCountChanged;
    
    public void ChangeCount(int change)
    {
        EntityCount += change;
        OnCountChanged?.Invoke(EntityCount);
    }

    public ArmyGroupData SerializeSelf()
    {
        ArmyGroupData data = new();
        data.EntityId = ArmyEntity.Id;
        data.EntityCount = EntityCount;
        return data;
    }

    public void LoadFromData(ArmyGroupData data)
    {
        ArmyEntity = GameManager.Instance.GameDatabase.GetArmyEntityById(data.EntityId);
        EntityCount = data.EntityCount;
    }
}

[System.Serializable]
public struct ArmyGroupData
{
    public string EntityId;
    public int EntityCount;
}

