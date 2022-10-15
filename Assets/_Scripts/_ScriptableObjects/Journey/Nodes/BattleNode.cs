using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[CreateAssetMenu(menuName = "ScriptableObject/Journey/BattleNode")]
public class BattleNode : JourneyNode
{
    GameManager _gameManager;

    public TilemapBiome Biome;
    public MapVariant MapVariant;
    public List<Brain> Enemies;
    public Vector2Int MapSize;
    public BattleGoal BattleGoal;

    public override void Initialize(GameObject self)
    {
        base.Initialize(self);
        _gameManager = GameManager.Instance;

        Biome = _gameManager.GameDatabase.GetRandomBiome();
        MapVariant = _gameManager.GameDatabase.GetRandomMapVariant();

        int numberOfEnemies = Random.Range(1, 6);
        for (int i = 0; i < numberOfEnemies; i++)
        {
            Brain b = _gameManager.GameDatabase.GetRandomEnemyBrain();
            Enemies.Add(b);
        }

        MapSize = new Vector2Int(Random.Range(5, 20), Random.Range(5, 20));
    }

    public void AddEnemy(Brain b)
    {
        Enemies.Add(b);
    }

}
