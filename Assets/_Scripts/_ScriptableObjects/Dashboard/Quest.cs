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

    public int DayStarted;
    [HideInInspector] public List<Character> AssignedCharacters = new();

    GameManager _gameManager;

    public event Action<QuestState> OnQuestStateChanged;
    public void UpdateQuestState(QuestState newState)
    {
        QuestState = newState;
        Debug.Log($"updating quest state: {newState}");
        switch (newState)
        {
            case QuestState.Pending:
                break;
            case QuestState.Delegated:
                break;
            case QuestState.Won:
                Won();
                break;
            case QuestState.Lost:
                Lost();
                break;
            case QuestState.Expired:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        }
        OnQuestStateChanged?.Invoke(newState);
    }

    public void OnDayPassed(int day)
    {
        Debug.Log($"on day passed");
        if (QuestState == QuestState.Delegated)
            HandleDelegatedQuest();

        if (IsExpired())
            UpdateQuestState(QuestState.Expired);
    }

    public void AssignCharacter(Character character) { AssignedCharacters.Add(character); }

    public void RemoveAssignedCharacter(Character character) { AssignedCharacters.Remove(character); }
    public int AssignedCharacterCount() { return AssignedCharacters.Count; }

    public int GetSuccessChance() { return AssignedCharacters.Count * 25; } // TODO: ofc, something cooler!

    public void DelegateQuest()
    {
        UpdateQuestState(QuestState.Delegated);
        DayStarted = _gameManager.Day;

        foreach (Character character in AssignedCharacters)
            character.SetUnavailable(Duration);

        _gameManager.SaveJsonData();
    }

    void HandleDelegatedQuest()
    {
        if (CountDaysLeft() > 0)
            return;

        if (Random.value < GetSuccessChance() * 0.01)
            UpdateQuestState(QuestState.Won);
        else
            UpdateQuestState(QuestState.Lost);
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
        Debug.Log($"Quest won");
    }

    public void Lost()
    {
        Debug.Log($"Quest lost");
        foreach (Character character in AssignedCharacters)
        {
            if (Random.value < 0.5f) // 50% chance to disable a character 
                continue;

            character.SetUnavailable(Random.Range(1, 5));
        }
    }

    public void CreateRandom()
    {
        _gameManager = GameManager.Instance;
        Icon = _gameManager.GameDatabase.GetRandomQuestIcon();
        Title = "Quest Title";
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
        Reward.IsRandomized = true;
        Reward.GoldRange = new Vector2Int(4, 10);
        Reward.HasItem = true;
        Reward.Initialize();

        ExpiryDay = _gameManager.Day + Random.Range(3, 7);
        Duration = Random.Range(1, 6);
        AssignedCharacters = new();
    }

    public void CreateFromData(QuestData data)
    {
        _gameManager = GameManager.Instance;
        _gameManager.OnDayPassed += OnDayPassed;

        Icon = _gameManager.GameDatabase.GetQuestIconById(data.QuestIconId);
        Title = data.Title;

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

        AssignedCharacters = new();
        foreach (string id in data.AssignedCharacters)
            AssignedCharacters.Add(_gameManager.PlayerTroops.First(x => x.Id == id));
    }

    public QuestData SerializeSelf()
    {
        QuestData qd = new();

        qd.QuestIconId = Icon.Id;
        qd.Title = Title;
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

    public List<string> AssignedCharacters;

    public bool IsWon;

}

