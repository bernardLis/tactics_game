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

    public virtual void Initialize()
    {
        foreach (Building b in Buildings)
            b.Initialize();
    }


    public virtual CastleData SerializeSelf()
    {
        CastleData data = new();
        data.AvailableArmy = new();
        foreach (ArmyEntity e in AvailableArmy)
            data.AvailableArmy.Add(e.SerializeSelf());

        return data;
    }

    public virtual void LoadFromData(CastleData data)
    {
        AvailableArmy = new();
        foreach (ArmyEntityData d in data.AvailableArmy)
            AvailableArmy.Add(Instantiate(GameManager.Instance.GameDatabase.GetArmyEntityById(d.Id)));
    }
}

[System.Serializable]
public struct CastleData
{
    public List<ArmyEntityData> AvailableArmy;
}


