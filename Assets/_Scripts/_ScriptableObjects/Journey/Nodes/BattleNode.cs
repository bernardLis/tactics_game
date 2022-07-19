using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BattleGoal { DefeatAllEnemies } // TODO: implement other battle goals (defeat the leader, hold position, ...)


[CreateAssetMenu(menuName = "ScriptableObject/Journey/BattleNode")]
public class BattleNode : JourneyNode
{
    JourneyMapManager _journeyMapManager;

    public TilemapBiome Biome;
    public MapVariant MapVariant;
    public List<Brain> Enemies;
    public Vector2Int MapSize;
    public BattleGoal BattleGoal; 

    public override void Initialize(GameObject self)
    {
        base.Initialize(self);
        _journeyMapManager = JourneyMapManager.Instance;

        Biome = _journeyMapManager.BattleDatabase.GetRandomBiome();
        MapVariant = _journeyMapManager.BattleDatabase.GetRandomMapVariant();

        int numberOfEnemies = Random.Range(1, 6);
        for (int i = 0; i < numberOfEnemies; i++)
        {
            Brain b = _journeyMapManager.BattleDatabase.GetRandomEnemyBrain();
            Enemies.Add(b);
        }

        MapSize = new Vector2Int(Random.Range(5, 20), Random.Range(5, 20));
    }

}
