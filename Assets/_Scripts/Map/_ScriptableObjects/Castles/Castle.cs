using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Map/Castle")]
public class Castle : BaseScriptableObject
{
    public static int MaxCastleArmySlots = 5;

    public Vector2 MapPosition;
    public Sprite Sprite;

    public List<Building> Buildings;
    public List<ArmyGroup> AvailableArmy;

    public void Initialize()
    {
        foreach (Building b in Buildings)
            b.Initialize();
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
        data.AvailableArmy = new();
        foreach (ArmyGroup ag in AvailableArmy)
            data.AvailableArmy.Add(ag.SerializeSelf());

        return data;
    }

    public void LoadFromData(CastleData data)
    {
        AvailableArmy = new();
        foreach (ArmyGroupData d in data.AvailableArmy)
        {
            ArmyGroup ag = CreateInstance<ArmyGroup>();
            ag.LoadFromData(d);
            AvailableArmy.Add(ag);
        }
    }
}

[System.Serializable]
public struct CastleData
{
    public string Id;
    public List<ArmyGroupData> AvailableArmy;
}


