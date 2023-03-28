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

    public string TemplateCastleId = "549d36bd-34c9-499a-815a-0a46ff37ecb1";
    public Vector2 CastlePosition = new Vector2(-1.5f, -8.5f);

    public void Reset()
    {
        Debug.Log($"resetting map");
        foreach (Collectable c in Collectables)
            c.IsCollected = false;
        foreach (Battle b in Battles)
            b.Won = false;

        Castle castle = (Castle)ScriptableObject.CreateInstance<Castle>();
        castle.Create(TemplateCastleId, CastlePosition);
        Castles.Add(castle);
    }

    public MapData SerializeSelf()
    {
        MapData data = new();
        data.CollectableDatas = new();
        foreach (Collectable c in Collectables)
            data.CollectableDatas.Add(c.SerializeSelf());
        data.BattleDatas = new();
        foreach (Battle b in Battles)
            data.BattleDatas.Add(b.SerializeSelf());
        data.CastleDatas = new();
        foreach (Castle c in Castles)
            data.CastleDatas.Add(c.SerializeSelf());

        return data;
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


        foreach (CastleData d in data.CastleDatas)
        {
            Castle castle = (Castle)ScriptableObject.CreateInstance<Castle>();
            castle.LoadFromData(d);
            Castles.Add(castle);
        }

        // foreach (CastleData d in data.CastleDatas)
        //     foreach (Castle c in Castles)
        //        if (d.Id == c.Id)
        //            c.LoadFromData(d);
    }
}

[System.Serializable]
public struct MapData
{
    public List<CollectableData> CollectableDatas;
    public List<BattleData> BattleDatas;
    public List<CastleData> CastleDatas;
}

