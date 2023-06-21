using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Map/Battle")]
public class Battle : BaseScriptableObject
{
    public Hero Opponent;

    public bool Won;


    public BattleData SerializeSelf()
    {
        BattleData data = new();
        data.Id = Id;
        data.Opponent = Opponent.SerializeSelf();
        data.Won = Won;

        return data;
    }

    public void LoadFromData(BattleData data)
    {
        Id = data.Id;
        name = "Battle";
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
