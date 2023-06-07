using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Map/Battle")]
public class Battle : BaseScriptableObject
{
    public Vector2 MapPosition;
    public Hero Opponent;

    public bool Won;

    public void Create(Vector2 pos)
    {
        Id = System.Guid.NewGuid().ToString();
        MapPosition = pos;
    }

    public int GetTotalNumberOfEnemies()
    {
        int total = 0;
        foreach (ArmyGroup ag in Opponent.Army)
            total += ag.NumberOfUnits;
        return total;
    }

    public BattleData SerializeSelf()
    {
        BattleData data = new();
        data.Id = Id;
        data.MapPosition = MapPosition;
        data.Opponent = Opponent.SerializeSelf();
        data.Won = Won;

        return data;
    }

    public void LoadFromData(BattleData data)
    {
        Id = data.Id;
        name = "Battle";
        MapPosition = data.MapPosition;
        Opponent = ScriptableObject.CreateInstance<Hero>();
        Opponent.LoadFromData(data.Opponent);

        Won = data.Won;
    }
}

[System.Serializable]
public struct BattleData
{
    public string Id;
    public Vector2 MapPosition;
    public HeroData Opponent;
    public bool Won;
}
