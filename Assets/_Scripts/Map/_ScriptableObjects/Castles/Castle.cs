using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Map/Castle")]
public class Castle : BaseScriptableObject
{
    public static int MaxCastleArmySlots = 5;

    public Vector2 MapPosition;
    public Sprite Sprite;

    public List<Building> Buildings = new();
    public List<ArmyGroupData> AvailableArmy = new();

    public void Initialize()
    {
        foreach (Building b in Buildings)
            b.Initialize();
    }

    public void AddArmy(ArmyGroup armyGroup)
    {
        ArmyGroupData agd = armyGroup.SerializeSelf();

        //  check if there is already this entity and add count if there is
        for (int i = 0; i < AvailableArmy.Count; i++)
        {
            if (AvailableArmy[i].EntityId == armyGroup.ArmyEntity.Id)
            {
                int prevCount = AvailableArmy[i].EntityCount;
                AvailableArmy.Remove(AvailableArmy[i]);
                agd.EntityCount += prevCount;
                AvailableArmy.Insert(i, agd);
                return;
            }
        }
        AvailableArmy.Add(agd);
        GameManager.Instance.SaveJsonData();
    }

    public void RemoveArmy(ArmyGroup armyGroup)
    {
        // TODO: 
        // AvailableArmy.Remove(armyEl.ArmyGroup);
    }

    public void Reset()
    {
        foreach (Building b in Buildings)
            b.Reset();

        AvailableArmy = new();
    }


    public CastleData SerializeSelf()
    {
        CastleData data = new();
        data.Id = Id;
        data.AvailableArmyDatas = new(AvailableArmy);
        return data;
    }

    public void LoadFromData(CastleData data)
    {
        AvailableArmy = new(data.AvailableArmyDatas);
    }
}

[System.Serializable]
public struct CastleData
{
    public string Id;
    public List<ArmyGroupData> AvailableArmyDatas;
}


