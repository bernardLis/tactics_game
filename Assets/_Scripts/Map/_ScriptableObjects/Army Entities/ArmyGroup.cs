using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Map/ArmyGroup")]
public class ArmyGroup : BaseScriptableObject
{
    public ArmyEntity ArmyEntity;
    public int Count;

    public event Action<int> OnCountChanged;
    
    public void ChangeCount(int change)
    {
        Count += change;
        OnCountChanged?.Invoke(Count);
    }

    public ArmyGroupData SerializeSelf()
    {
        ArmyGroupData data = new();
        data.EntityId = ArmyEntity.Id;
        data.Count = Count;
        return data;
    }

    public void LoadFromData(ArmyGroupData data)
    {
        ArmyEntity = GameManager.Instance.GameDatabase.GetArmyEntityById(data.EntityId);
        Count = data.Count;
    }
}

[System.Serializable]
public struct ArmyGroupData
{
    public string EntityId;
    public int Count;
}

