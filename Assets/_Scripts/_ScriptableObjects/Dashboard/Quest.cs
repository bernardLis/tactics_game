using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;


[CreateAssetMenu(menuName = "ScriptableObject/Quest")]
public class Quest : BaseScriptableObject
{
    public Sprite Icon;
    public string Title;
    public string SceneToLoad = Scenes.Battle;

    public TilemapBiome Biome;
    public MapVariant MapVariant;
    public Vector2Int MapSize;
    public BattleGoal BattleGoal;
    public List<Brain> Enemies = new();
    public Reward Reward;

    GameManager _gameManager;

    public void CreateRandom()
    {
        _gameManager = GameManager.Instance;
        Icon = _gameManager.GameDatabase.GetRandomBattleNodeIcon();
        Title = "Title of The Quest";

        Biome = _gameManager.GameDatabase.GetRandomBiome();
        MapVariant = _gameManager.GameDatabase.GetRandomMapVariant();
        MapSize = new Vector2Int(Random.Range(5, 20), Random.Range(5, 20));
        BattleGoal = BattleGoal.DefeatAllEnemies;

        int numberOfEnemies = Random.Range(1, 6);
        for (int i = 0; i < numberOfEnemies; i++)
        {
            Brain b = Instantiate(_gameManager.GameDatabase.GetRandomEnemyBrain());
            Enemies.Add(b);
        }

        Reward = ScriptableObject.CreateInstance<Reward>();
        Reward.IsRandomized = true;
        Reward.GoldRange = new Vector2Int(4, 10);
        Reward.HasItem = true;
        Reward.Initialize();
    }

    public void CreateFromData(BattleNodeData data)
    {
        _gameManager = GameManager.Instance;

        // TODO: need icon with id
        Icon = _gameManager.GameDatabase.GetRandomBattleNodeIcon();
        Title = data.Title;
        SceneToLoad = data.SceneToLoad;

        Biome = _gameManager.GameDatabase.GetTilemapBiomeById(data.Biome);
        MapVariant = _gameManager.GameDatabase.GetMapVariantById(data.MapVariant);
        MapSize = data.MapSize;
        BattleGoal = BattleGoal.DefeatAllEnemies;
        Enemies = new();
        foreach (string e in data.Enemies)
            Enemies.Add(_gameManager.GameDatabase.GetEnemyBrainById(e));

        // TODO: make reward serializable
        Reward = ScriptableObject.CreateInstance<Reward>();
        Reward.IsRandomized = true;
        Reward.GoldRange = new Vector2Int(4, 10);
        Reward.HasItem = true;
    }
}

[Serializable]
public struct BattleNodeData
{
    // TODO: need icon with id
    public string Title;
    public string SceneToLoad;
    public string Biome;
    public string MapVariant;
    public Vector2Int MapSize;
    public List<string> Enemies;
    // TODO: make reward serializable

}

