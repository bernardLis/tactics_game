using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Map/Map")]
public class Map : BaseScriptableObject
{
    public List<Collectable> Collectables = new();
    public List<Battle> Battles = new();
    public List<Castle> Castles = new();

    public MapData SerializeSelf()
    {
        MapData data = new();
        data.CollectableDatas = new();
        foreach (Collectable c in Collectables)
            data.CollectableDatas.Add(c.SerializeSelf());
        data.BattleDatas = new();
        foreach (Battle b in Battles)
            data.BattleDatas.Add(b.SerializeSelf());

        return data;
    }

    public void Reset()
    {
        foreach (Collectable c in Collectables)
            c.IsCollected = false;
        foreach (Battle b in Battles)
            b.Won = false;
    }

    public void LoadFromData(MapData data)
    {
        foreach (CollectableData d in data.CollectableDatas)
            foreach (Collectable c in Collectables)
                if (d.Id == c.Id)
                    c.LoadFromData(d);

        foreach (BattleData d in data.BattleDatas)
            foreach (Battle b in Battles)
                if (d.Id == b.Id)
                    b.LoadFromData(d);
    }
}

[System.Serializable]
public struct MapData
{
    public List<CollectableData> CollectableDatas;
    public List<BattleData> BattleDatas;
}
