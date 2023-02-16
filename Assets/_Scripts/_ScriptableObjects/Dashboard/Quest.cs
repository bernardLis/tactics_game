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
    public int DayAdded;
    public DateTime ExpiryDateTime;
    public DateTime StartedDateTime;
    public int DurationSeconds;
    public float Roll;
    public bool IsWon;

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

    public void AssignCharacter(Character character)
    {
        Debug.Log($"Assigning character: {character} to quest {Title}");
        character.IsAssigned = true;
        AssignedCharacters.Add(character);
    }

    public void RemoveAssignedCharacter(Character character)
    {
        Debug.Log($"Removing assigned character: {character} from quest {Title}");
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
        StartedDateTime = ScriptableObject.CreateInstance<DateTime>();
        StartedDateTime.Day = _gameManager.Day;
        StartedDateTime.Seconds = GameManager.SecondsInDay - _gameManager.SecondsLeftInDay;

        UpdateQuestState(QuestState.Delegated);
        _gameManager.SaveJsonData();
    }

    public void FinishQuest()
    {
        if (Roll <= GetSuccessChance() * 0.01)
            Won();
        else
            Lost();
        UpdateQuestState(QuestState.Finished);
    }

    public void Won() { IsWon = true; }

    public void Lost()
    {
        Debug.Log($"Quest lost");
        CampBuildingHospital h = _gameManager.GetComponent<BuildingManager>().HospitalBuilding;
        int maxDaysDisabled = h.GetUpgradeByRank(h.UpgradeRank).MaxDaysDisabled;
        foreach (Character character in AssignedCharacters)
            character.SetUnavailable(Random.Range(1, maxDaysDisabled + 1));
    }

    // TODO: change reward exp to take into consideration rank difference and winning losing quest
    public int CalculateRewardExp(Character c) { return IsWon ? 100 : 100; }

    public void CreateRandom()
    {
        _gameManager = GameManager.Instance;

        CampBuildingQuests b = _gameManager.GetComponent<BuildingManager>().QuestsBuilding;
        int maxQuestRank = b.GetUpgradeByRank(b.UpgradeRank).MaxQuestRank;
        Rank = _gameManager.GameDatabase.GetRandomQuestRankWithMaxRank(maxQuestRank);

        Title = _gameManager.GameDatabase.QuestDatabase.GetRandomQuestTitle();
        Description = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.";
        ThreatElement = _gameManager.GameDatabase.GetRandomElement();
        QuestState = QuestState.Pending;

        Reward = Instantiate(_gameManager.GameDatabase.GetRewardByQuestRank(Rank.Rank));
        Reward.Initialize();

        DayAdded = _gameManager.Day;
        ExpiryDateTime = ScriptableObject.CreateInstance<DateTime>();
        ExpiryDateTime.Day = _gameManager.Day + Random.Range(3, 7);
        StartedDateTime = ScriptableObject.CreateInstance<DateTime>();
        DurationSeconds = Random.Range(10, 26);
        AssignedCharacters = new();

        Roll = Random.value;
    }

    public void LoadFromData(QuestData data)
    {
        _gameManager = GameManager.Instance;

        Rank = _gameManager.GameDatabase.GetQuestRankById(data.QuestRankId);
        ThreatElement = _gameManager.GameDatabase.GetElementByName((ElementName)System.Enum.Parse(typeof(ElementName), data.ThreatElement));
        Title = data.Title;
        Description = data.Description;

        QuestState = (QuestState)Enum.Parse(typeof(QuestState), data.QuestState);

        Reward r = _gameManager.GameDatabase.GetRewardByRank(data.RewardRank);
        Reward = Instantiate(r);
        Reward.Initialize();

        DayAdded = data.DayAdded;

        ExpiryDateTime = ScriptableObject.CreateInstance<DateTime>();
        ExpiryDateTime.LoadFromData(data.ExpiryDateTime);

        DurationSeconds = data.DurationSeconds;

        StartedDateTime = ScriptableObject.CreateInstance<DateTime>();
        StartedDateTime.LoadFromData(data.StartedDateTime);

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

        qd.RewardRank = Reward.Rank;

        qd.DayAdded = DayAdded;
        qd.ExpiryDateTime = ExpiryDateTime.SerializeSelf();
        qd.DurationSeconds = DurationSeconds;
        qd.StartedDateTime = StartedDateTime.SerializeSelf();
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
    public int RewardRank;

    public int DayAdded;
    public DateTimeData ExpiryDateTime;
    public int DurationSeconds;
    public DateTimeData StartedDateTime;

    public int DayStarted;
    public float Roll;
    public bool IsWon;
    public List<string> AssignedCharacters;
}

