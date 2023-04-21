using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Map/Battle")]
public class Battle : BaseScriptableObject
{
    public Vector2 MapPosition;
    public List<ArmyGroup> Army = new();

    public bool Won;

    public void Create(Vector2 pos)
    {
        Id = System.Guid.NewGuid().ToString();
        MapPosition = pos;
        RandomizeBattle();
    }

    public void RandomizeBattle()
    {
        GameManager gameManager = GameManager.Instance;

        Army = new();
        Army.Add(ScriptableObject.CreateInstance<ArmyGroup>());
        Army[0].ArmyEntity = gameManager.GameDatabase.AllEnemyArmyEntities[0];
        Army[0].EntityCount = Random.Range(1, 10);
        Army.Add(ScriptableObject.CreateInstance<ArmyGroup>());
        Army[1].ArmyEntity = gameManager.GameDatabase.AllEnemyArmyEntities[1];
        Army[1].EntityCount = Random.Range(1, 10);
    }

    public int GetTotalNumberOfEnemies()
    {
        int total = 0;
        foreach (ArmyGroup ag in Army)
            total += ag.EntityCount;
        return total;
    }

    public BattleData SerializeSelf()
    {
        BattleData data = new();
        data.Id = Id;
        data.MapPosition = MapPosition;
        data.ArmyDatas = new();
        foreach (ArmyGroup ag in Army)
            data.ArmyDatas.Add(ag.SerializeSelf());

        data.Won = Won;

        return data;
    }

    public void LoadFromData(BattleData data)
    {
        Id = data.Id;
        name = "Battle";
        MapPosition = data.MapPosition;
        Army = new();
        foreach (ArmyGroupData agd in data.ArmyDatas)
        {
            ArmyGroup ag = ScriptableObject.CreateInstance<ArmyGroup>();
            ag.LoadFromData(agd);
            Army.Add(ag);
        }

        Won = data.Won;
    }
}

[System.Serializable]
public struct BattleData
{
    public string Id;
    public Vector2 MapPosition;
    public List<ArmyGroupData> ArmyDatas;
    public bool Won;
}
