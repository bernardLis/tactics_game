using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Recruit : BaseScriptableObject
{
    GameManager _gameManager;

    public RecruitState RecruitState { get; private set; }
    public Character Character;
    public int DayAdded;
    public int DaysUntilExpired;

    public event Action<int> OnDaysUntilExpiredChanged;
    public event Action<RecruitState> OnRecruitStateChanged;
    public void UpdateRecruitState(RecruitState newState)
    {
        Debug.Log($"UpdateRecruitState: {newState} for {Character.CharacterName}");
        RecruitState = newState;
        switch (newState)
        {
            case RecruitState.Pending:
                break;
            case RecruitState.Resolved:
                break;
            case RecruitState.Expired:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        }
        OnRecruitStateChanged?.Invoke(newState);
    }

    public void Initialize()
    {
        _gameManager = GameManager.Instance;
        _gameManager.OnDayPassed += OnDayPassed;
    }

    void OnDayPassed(int day)
    {
        if (RecruitState != RecruitState.Pending)
            return;

        DaysUntilExpired--;
        if (DaysUntilExpired <= 0)
            UpdateRecruitState(RecruitState.Expired);

        OnDaysUntilExpiredChanged?.Invoke(DaysUntilExpired);
    }

    public void CreateRandom()
    {
        Initialize();

        RecruitState = RecruitState.Pending;

        Character newChar = ScriptableObject.CreateInstance<Character>();
        newChar.CreateRandom();
        Character = newChar;

        DayAdded = _gameManager.Day;
        DaysUntilExpired = Random.Range(2, 5);
        Debug.Log($"DaysUntilExpired: {DaysUntilExpired}");
    }

    public void LoadFromData(RecruitData data)
    {
        Initialize();

        RecruitState = (RecruitState)System.Enum.Parse(typeof(RecruitState), data.RecruitState);

        Character character = ScriptableObject.CreateInstance<Character>();
        character.CreateFromData(data.CharacterData);
        Character = character;

        DayAdded = data.DayAdded;
        DaysUntilExpired = data.DaysUntilExpired;
        if (DaysUntilExpired <= 0 && RecruitState != RecruitState.Expired)
            UpdateRecruitState(RecruitState.Expired);
    }

    public RecruitData SerializeSelf()
    {
        RecruitData data = new();

        data.RecruitState = RecruitState.ToString();
        data.CharacterData = Character.SerializeSelf();
        data.DayAdded = DayAdded;
        data.DaysUntilExpired = DaysUntilExpired;

        return data;
    }
}


[Serializable]
public struct RecruitData
{
    public string RecruitState;
    public CharacterData CharacterData;
    public int DayAdded;
    public int DaysUntilExpired;
}
