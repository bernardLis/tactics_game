using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Recruit : BaseScriptableObject
{
    public RecruitState RecruitState { get; private set; }
    public Character Character;

    public DateTime DateTimeAdded;
    public DateTime DateTimeExpired;

    public event Action<RecruitState> OnRecruitStateChanged;
    public void UpdateRecruitState(RecruitState newState)
    {
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

    public void CreateRandom(int level)
    {
        GameManager gm = GameManager.Instance;

        RecruitState = RecruitState.Pending;

        Character newChar = ScriptableObject.CreateInstance<Character>();
        newChar.CreateRandom(level);
        Character = newChar;

        DateTimeAdded = ScriptableObject.CreateInstance<DateTime>();
        DateTimeAdded = gm.GetCurrentDateTime();

        DateTimeExpired = ScriptableObject.CreateInstance<DateTime>();
        DateTimeExpired.Day = gm.Day + Random.Range(2, 5);
    }

    public void LoadFromData(RecruitData data)
    {
        RecruitState = (RecruitState)System.Enum.Parse(typeof(RecruitState), data.RecruitState);

        Character character = ScriptableObject.CreateInstance<Character>();
        character.CreateFromData(data.CharacterData);
        Character = character;

        DateTimeAdded = ScriptableObject.CreateInstance<DateTime>();
        DateTimeAdded.LoadFromData(data.DateTimeAdded);

        DateTimeExpired = ScriptableObject.CreateInstance<DateTime>();
        DateTimeExpired.LoadFromData(data.DateTimeExpired);

        float diff = DateTimeExpired.GetTimeInSeconds() - GameManager.Instance.GetCurrentTimeInSeconds();
        Debug.Log($"DateTimeExpired.GetTimeInSeconds() {DateTimeExpired.GetTimeInSeconds()}");
        Debug.Log($"GameManager.Instance.GetCurrentTimeInSeconds() {GameManager.Instance.GetCurrentTimeInSeconds()}");

        Debug.Log($"diff {diff}");
        if (diff <= 0 && RecruitState != RecruitState.Expired)
            UpdateRecruitState(RecruitState.Expired);
    }

    public RecruitData SerializeSelf()
    {
        RecruitData data = new();

        data.RecruitState = RecruitState.ToString();
        data.CharacterData = Character.SerializeSelf();

        data.DateTimeAdded = DateTimeAdded.SerializeSelf();
        data.DateTimeExpired = DateTimeExpired.SerializeSelf();

        return data;
    }
}


[Serializable]
public struct RecruitData
{
    public string RecruitState;
    public CharacterData CharacterData;
    public DateTimeData DateTimeAdded;
    public DateTimeData DateTimeExpired;
}
