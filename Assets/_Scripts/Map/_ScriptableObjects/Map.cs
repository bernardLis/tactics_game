using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Map/Map")]
public class Map : BaseScriptableObject
{
    public List<Collectable> Collectables = new();
    public List<Battle> Battles = new();

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

    public void LoadFromData(MapData data)
    {
        Collectables = new();
        foreach (CollectableData d in data.CollectableDatas)
        {
            if (d.Type == "CollectableGold")
            {
                CollectableGold g = ScriptableObject.CreateInstance<CollectableGold>();
                g.LoadFromData(d);
                Collectables.Add(g);
            }
            if (d.Type == "CollectableSpice")
            {
                CollectableSpice g = ScriptableObject.CreateInstance<CollectableSpice>();
                g.LoadFromData(d);
                Collectables.Add(g);
            }
            if (d.Type == "CollectableItem")
            {
                CollectableItem g = ScriptableObject.CreateInstance<CollectableItem>();
                g.LoadFromData(d);
                Collectables.Add(g);
            }
        }

        Battles = new();
        foreach (BattleData d in data.BattleDatas)
        {
            // create a battle and add it to battles
            Battle b = ScriptableObject.CreateInstance<Battle>();
            b.LoadFromData(d);
            Battles.Add(b);
        }
    }
}

[System.Serializable]
public struct MapData
{
    public List<CollectableData> CollectableDatas;
    public List<BattleData> BattleDatas;
}
