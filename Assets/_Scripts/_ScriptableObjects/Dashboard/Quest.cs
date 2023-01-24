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
    public Element ThreatElement;
    public Reward Reward;
    public QuestState QuestState;

    [Header("Management")]
    public int ExpiryDay;
    public int Duration;
    public float Roll;
    public bool IsWon;

    public int DayStarted;
    [HideInInspector] public List<Character> AssignedCharacters = new();

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

    public int GetSuccessChance()
    {
        int successChance = 0;
        foreach (Character c in AssignedCharacters)
        {
            // element
            if (c.Element == ThreatElement.WeakAgainst)
                successChance += 10;
            if (c.Element == ThreatElement.StrongAgainst)
                successChance -= 10;

            // rank
            int rankDiff = c.Rank.Rank - Rank.Rank;
            if (rankDiff == 0)
            {
                successChance += 40;
                continue;
            }

            int influence = 40 + rankDiff * 10; // TODO: magic 40 and 10
            influence = Mathf.Clamp(influence, 0, 100);
            successChance += influence;
        }
        successChance = Mathf.Clamp(successChance, 0, 100);

        return successChance;
    }

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

    public void Won() { IsWon = true; }

    public void Lost()
    {
        foreach (Character character in AssignedCharacters)
            character.SetUnavailable(Random.Range(1, 5));
    }

    // TODO: change reward exp to take into consideration rank difference and winning losing quest
    public int CalculateRewardExp(Character c) { return IsWon ? 100 : 100; }

    public void CreateRandom()
    {
        _gameManager = GameManager.Instance;
        Rank = _gameManager.GameDatabase.GetRandomQuestRankWithMaxRank(_gameManager.MaxQuestRank);
        Title = _gameManager.GameDatabase.QuestDatabase.GetRandomQuestTitle();
        Description = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.";
        ThreatElement = _gameManager.GameDatabase.GetRandomElement();
        QuestState = QuestState.Pending;

        Reward = ScriptableObject.CreateInstance<Reward>();
        Reward.CreateRandom();

        ExpiryDay = _gameManager.Day + Random.Range(3, 7);
        Duration = Random.Range(1, 6);
        AssignedCharacters = new();

        Roll = Random.value;
    }

    public void LoadFromData(QuestData data)
    {
        _gameManager = GameManager.Instance;
        _gameManager.OnDayPassed += OnDayPassed;

        Rank = _gameManager.GameDatabase.GetQuestRankById(data.QuestRankId);
        ThreatElement = _gameManager.GameDatabase.GetElementByName((ElementName)System.Enum.Parse(typeof(ElementName), data.ThreatElement));
        Title = data.Title;
        Description = data.Description;

        QuestState = (QuestState)Enum.Parse(typeof(QuestState), data.QuestState);

        Reward = ScriptableObject.CreateInstance<Reward>();
        Reward.Gold = data.RewardData.Gold;
        Reward.Item = _gameManager.GameDatabase.GetItemById(data.RewardData.ItemId);

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
        qd.ThreatElement = ThreatElement.ElementName.ToString();
        qd.QuestState = QuestState.ToString();

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
    public string ThreatElement;
    public string QuestState;
    public RewardData RewardData;

    public int ExpiryDay;
    public int Duration;
    public int DayStarted;
    public float Roll;
    public bool IsWon;
    public List<string> AssignedCharacters;
}

