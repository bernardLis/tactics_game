using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;
using System.Linq;

[CreateAssetMenu(menuName = "ScriptableObject/Dashboard/Quest")]
public class Quest : BaseScriptableObject
{
    [Header("Basics")]
    public QuestRank Rank;
    public string Title;
    public string Description;
    public QuestState QuestState;

    [Header("Battle")]
    public string SceneToLoad = Scenes.Battle;
    public TilemapBiome Biome;
    public MapVariant MapVariant;
    public Vector2Int MapSize;
    public BattleGoal BattleGoal;
    public List<Brain> Enemies = new();
    public Reward Reward;

    [Header("Management")]
    public int ExpiryDay;
    public int Duration;
    public float Roll;
    public bool IsWon;

    public int DayStarted;
    //[HideInInspector] // HERE: test battle
    public List<Character> AssignedCharacters = new();

    GameManager _gameManager;

    public event Action<QuestState> OnQuestStateChanged;
    public void UpdateQuestState(QuestState newState)
    {
        QuestState = newState;
        switch (newState)
        {
            case QuestState.Pending:
                break;
            case QuestState.Delegated:
                break;
            case QuestState.Finished:
                break;
            case QuestState.Expired:
                break;
            case QuestState.RewardCollected:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        }
        OnQuestStateChanged?.Invoke(newState);
    }

    public void OnDayPassed(int day)
    {
        if (QuestState == QuestState.Delegated)
            HandleDelegatedQuest();

        if (IsExpired() && QuestState != QuestState.Expired)
            UpdateQuestState(QuestState.Expired);
    }

    public void AssignCharacter(Character character)
    {
        character.IsAssigned = true;
        AssignedCharacters.Add(character);
    }

    public void RemoveAssignedCharacter(Character character)
    {
        character.IsAssigned = false;
        AssignedCharacters.Remove(character);
    }
    public int AssignedCharacterCount() { return AssignedCharacters.Count; }

    public int GetSuccessChance() { return AssignedCharacters.Count * 25; } // TODO: ofc, something cooler!

    public void DelegateQuest()
    {
        DayStarted = _gameManager.Day;

        foreach (Character character in AssignedCharacters)
            character.SetUnavailable(Duration);

        UpdateQuestState(QuestState.Delegated);
        _gameManager.SaveJsonData();
    }

    void HandleDelegatedQuest()
    {
        if (CountDaysLeft() > 0)
            return;

        if (Roll <= GetSuccessChance() * 0.01)
            Won();
        else
            Lost();
        UpdateQuestState(QuestState.Finished);
    }

    public int CountDaysLeft() { return Duration - (_gameManager.Day - DayStarted); }

    public bool IsExpired()
    {
        if (QuestState == QuestState.Expired)
            return true;

        if (QuestState != QuestState.Pending)
            return false;

        return ExpiryDay - _gameManager.Day <= 0;
    }

    public void Won()
    {
        IsWon = true;
    }

    public void Lost()
    {
        foreach (Character character in AssignedCharacters)
        {
            if (Random.value < 0.5f) // 50% chance to disable a character 
                continue;

            character.SetUnavailable(Random.Range(1, 5));
        }
    }

    public int CalculateAwardExp()
    {
        return IsWon ? 100 : 10;
    }

    public void CreateRandom()
    {
        _gameManager = GameManager.Instance;
        Rank = _gameManager.GameDatabase.GetRandomQuestRank();
        Title = "Quest Title";
        Description = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.";
        QuestState = QuestState.Pending;

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
        Reward.CreateRandom();

        ExpiryDay = _gameManager.Day + Random.Range(3, 7);
        Duration = Random.Range(1, 6);
        AssignedCharacters = new();

        Roll = Random.value;
    }

    public void CreateFromData(QuestData data)
    {
        _gameManager = GameManager.Instance;
        _gameManager.OnDayPassed += OnDayPassed;

        Rank = _gameManager.GameDatabase.GetQuestRankById(data.QuestRankId);
        Title = data.Title;
        Description = data.Description;

        QuestState = (QuestState)Enum.Parse(typeof(QuestState), data.QuestState);

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

        ExpiryDay = data.ExpiryDay;
        Duration = data.Duration;
        DayStarted = data.DayStarted;
        Roll = data.Roll;
        IsWon = data.IsWon;

        AssignedCharacters = new();
        foreach (string id in data.AssignedCharacters)
            AssignedCharacters.Add(_gameManager.PlayerTroops.First(x => x.Id == id));
    }

    public QuestData SerializeSelf()
    {
        QuestData qd = new();

        qd.QuestRankId = Rank.Id;
        qd.Title = Title;
        qd.Description = Description;
        qd.QuestState = QuestState.ToString();

        qd.SceneToLoad = SceneToLoad;
        qd.Biome = Biome.Id;
        qd.MapVariant = MapVariant.Id;
        qd.MapSize = MapSize;
        qd.Enemies = new();
        foreach (Brain e in Enemies)
            qd.Enemies.Add(e.Id);

        qd.RewardData = Reward.SerializeSelf();

        qd.ExpiryDay = ExpiryDay;
        qd.Duration = Duration;
        qd.DayStarted = DayStarted;
        qd.Roll = Roll;
        qd.IsWon = IsWon;

        qd.AssignedCharacters = new();
        foreach (Character c in AssignedCharacters)
            qd.AssignedCharacters.Add(c.Id);

        return qd;
    }
}

[Serializable]
public struct QuestData
{
    public string QuestRankId;
    public string Title;
    public string Description;
    public string QuestState;

    public string SceneToLoad;
    public string Biome;
    public string MapVariant;
    public Vector2Int MapSize;
    public List<string> Enemies;
    public RewardData RewardData;

    public int ExpiryDay;
    public int Duration;
    public int DayStarted;
    public float Roll;
    public bool IsWon;
    public List<string> AssignedCharacters;
}

