using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;
using System.Linq;

[CreateAssetMenu(menuName = "ScriptableObject/Quest")]
public class Quest : BaseScriptableObject
{
    [Header("Basics")]
    public QuestIcon Icon;
    public string Title;

    [Header("Battle")]
    public string SceneToLoad = Scenes.Battle;
    public TilemapBiome Biome;
    public MapVariant MapVariant;
    public Vector2Int MapSize;
    public BattleGoal BattleGoal;
    public List<Brain> Enemies = new();
    public Reward Reward;

    [Header("Management")]
    [HideInInspector] public bool IsDelegated;
    public int Duration;
    public int DayStarted;
    [HideInInspector] public List<Character> AssignedCharacters = new();

    GameManager _gameManager;

    public void CreateRandom()
    {
        _gameManager = GameManager.Instance;
        Icon = _gameManager.GameDatabase.GetRandomQuestIcon();
        Debug.Log($"creating random Icon: {Icon}");
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

        Duration = Random.Range(1, 6);
        AssignedCharacters = new();
    }

    public void CreateFromData(QuestData data)
    {
        _gameManager = GameManager.Instance;

        Icon = _gameManager.GameDatabase.GetQuestIconById(data.QuestIconId);
        Title = data.Title;
        SceneToLoad = data.SceneToLoad;

        Biome = _gameManager.GameDatabase.GetTilemapBiomeById(data.Biome);
        MapVariant = _gameManager.GameDatabase.GetMapVariantById(data.MapVariant);
        MapSize = data.MapSize;
        BattleGoal = BattleGoal.DefeatAllEnemies;
        Enemies = new();
        foreach (string e in data.Enemies)
            Enemies.Add(_gameManager.GameDatabase.GetEnemyBrainById(e));

        Reward = ScriptableObject.CreateInstance<Reward>();
        Reward.Gold = data.RewardData.Gold;
        Reward.Item = _gameManager.GameDatabase.GetItemByReferenceId(data.RewardData.ItemReferenceId);

        IsDelegated = data.IsDelegated;
        Duration = data.Duration;
        DayStarted = data.DayStarted;

        AssignedCharacters = new();
        foreach (string id in data.AssignedCharacters)
            AssignedCharacters.Add(_gameManager.PlayerTroops.First(x => x.Id == id));
    }

    public void AssignCharacter(Character character) { AssignedCharacters.Add(character); }

    public void RemoveAssignedCharacter(Character character) { AssignedCharacters.Remove(character); }

    public void DelegateQuest()
    {
        DayStarted = _gameManager.Day;
        foreach (Character character in AssignedCharacters)
        {
            character.IsOnQuest = true;
        }
    }

    public QuestData SerializeSelf()
    {
        QuestData qd = new();

        qd.QuestIconId = Icon.Id;
        qd.Title = Title;
        qd.SceneToLoad = SceneToLoad;
        qd.Biome = Biome.Id;
        qd.MapVariant = MapVariant.Id;
        qd.MapSize = MapSize;
        qd.Enemies = new();
        foreach (Brain e in Enemies)
            qd.Enemies.Add(e.Id);

        qd.RewardData = Reward.SerializeSelf();

        qd.IsDelegated = IsDelegated;
        qd.Duration = Duration;
        qd.DayStarted = DayStarted;

        qd.AssignedCharacters = new();
        foreach (Character c in AssignedCharacters)
            qd.AssignedCharacters.Add(c.Id);

        return qd;
    }
}

[Serializable]
public struct QuestData
{
    public string QuestIconId;
    public string Title;
    public string SceneToLoad;
    public string Biome;
    public string MapVariant;
    public Vector2Int MapSize;
    public List<string> Enemies;
    public RewardData RewardData;

    public bool IsDelegated;
    public int Duration;
    public int DayStarted;

    public List<string> AssignedCharacters;

}

