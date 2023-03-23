using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Map/Castle")]
public class Castle : BaseScriptableObject
{
    public Vector2 MapPosition;
    public Sprite Sprite;

    public List<Building> Buildings;
    public List<ArmyEntity> AvailableArmy;

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
        foreach (ArmyEntity e in AvailableArmy)
            data.AvailableArmy.Add(e.SerializeSelf());

        return data;
    }

    public void LoadFromData(CastleData data)
    {
        AvailableArmy = new();
        foreach (ArmyEntityData d in data.AvailableArmy)
            AvailableArmy.Add(Instantiate(GameManager.Instance.GameDatabase.GetArmyEntityById(d.Id)));
    }
}

[System.Serializable]
public struct CastleData
{
    public string Id;
    public List<ArmyEntityData> AvailableArmy;
}


