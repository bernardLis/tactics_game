using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(menuName = "ScriptableObject/Map/Map")]
public class Map : BaseScriptableObject
{
    public List<Collectable> Collectables = new();
    public List<Battle> Battles = new();
    public List<Castle> Castles = new();
    public List<int> ExploredListPositions = new();

    public string TemplateCastleId = "549d36bd-34c9-499a-815a-0a46ff37ecb1";
    public Vector2 CastlePosition = new Vector2(-1.5f, -8.5f);
    public Vector2 CastlePosition1 = new Vector2(-0.5f, -0.5f);

    public void Reset()
    {
        Debug.Log($"resetting map");
        Collectables = new();
        for (int i = 0; i < 10; i++)
            CreateCollectable();

        Battles = new();
        for (int i = 0; i < 3; i++)
            CreateBattle();

        Castle castle = (Castle)ScriptableObject.CreateInstance<Castle>();
        castle.Create(TemplateCastleId, CastlePosition);
        Castles.Add(castle);

        Castle castle1 = (Castle)ScriptableObject.CreateInstance<Castle>();
        castle1.Create(TemplateCastleId, CastlePosition1);
        Castles.Add(castle1);
    }

    public void CreateCollectable()
    {
        Vector2 pos = new Vector2(Random.Range(-10, 10), Random.Range(-10, 10));
        pos.x += 0.5f;
        pos.y += 0.5f;
        // 33% chance to create CollectableGold, 33% chance to create CollectableItem, 33% chance to create SpiceCollectable
        int rand = Random.Range(0, 3);
        Collectable collectable = null;
        if (rand == 0) collectable = ScriptableObject.CreateInstance<CollectableGold>();
        if (rand == 1) collectable = ScriptableObject.CreateInstance<CollectableSpice>();
        if (rand == 2) collectable = ScriptableObject.CreateInstance<CollectableItem>();

        collectable.Create(pos);
        Collectables.Add(collectable);
    }

    void CreateBattle()
    {
        Vector2 pos = new Vector2(Random.Range(-10, 10), Random.Range(-10, 10));
        pos.x += 0.5f;
        pos.y += 0.5f;

        Battle battle = (Battle)ScriptableObject.CreateInstance<Battle>();
        battle.Create(pos);
        Battles.Add(battle);
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

        data.ExploredListPositions = ExploredListPositions;

        return data;
    }

    public void LoadFromData(MapData data)
    {
        foreach (CollectableData d in data.CollectableDatas)
        {
            Collectable collectable = null;
            if (d.Type == "CollectableGold")
                collectable = ScriptableObject.CreateInstance<CollectableGold>();
            if (d.Type == "CollectableSpice")
                collectable = ScriptableObject.CreateInstance<CollectableSpice>();
            if (d.Type == "CollectableItem")
                collectable = ScriptableObject.CreateInstance<CollectableItem>();

            collectable.LoadFromData(d);
            Collectables.Add(collectable);
        }

        foreach (BattleData d in data.BattleDatas)
        {
            Battle battle = (Battle)ScriptableObject.CreateInstance<Battle>();
            battle.LoadFromData(d);
            Battles.Add(battle);
        }

        foreach (CastleData d in data.CastleDatas)
        {
            Castle castle = (Castle)ScriptableObject.CreateInstance<Castle>();
            castle.LoadFromData(d);
            Castles.Add(castle);
        }

        ExploredListPositions = data.ExploredListPositions;
    }
}

[System.Serializable]
public struct MapData
{
    public List<CollectableData> CollectableDatas;
    public List<BattleData> BattleDatas;
    public List<CastleData> CastleDatas;
    public List<int> ExploredListPositions;
}

