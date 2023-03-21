using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Map/Battle")]
public class Battle : BaseScriptableObject
{
    public Vector2 MapPosition;
    public int NumberOfMeleeEnemies;
    public int NumberOfRangedEnemies;

    [HideInInspector] public Character Character;

    public bool Won;

    public void RandomizeBattle()
    {
        NumberOfMeleeEnemies = Random.Range(10, 20);
        NumberOfRangedEnemies = Random.Range(10, 20);
    }

    public BattleData SerializeSelf()
    {
        BattleData data = new();
        data.Id = Id;
        data.MapPosition = MapPosition;
        data.NumberOfMeleeEnemies = NumberOfMeleeEnemies;
        data.NumberOfRangedEnemies = NumberOfRangedEnemies;
        data.Won = Won;
        if (Character != null)
            data.CharacterId = Character.Id;

        return data;
    }

    public void LoadFromData(BattleData data)
    {
        MapPosition = data.MapPosition;
        NumberOfMeleeEnemies = data.NumberOfMeleeEnemies;
        NumberOfRangedEnemies = data.NumberOfRangedEnemies;
        Won = data.Won;

        // TODO: loading character from ID
        //   if (data.CharacterId != null)
        //      data.CharacterId = Character.Id;
    }
}

[System.Serializable]
public struct BattleData
{
    public string Id;
    public Vector2 MapPosition;
    public int NumberOfMeleeEnemies;
    public int NumberOfRangedEnemies;
    public bool Won;

    public string CharacterId;
}
