using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Recruit : BaseScriptableObject
{
    public RecruitState RecruitState { get; private set; }
    public Character Character;

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
    }

    public void LoadFromData(RecruitData data)
    {
        RecruitState = (RecruitState)System.Enum.Parse(typeof(RecruitState), data.RecruitState);

        Character character = ScriptableObject.CreateInstance<Character>();
        character.CreateFromData(data.CharacterData);
        Character = character;
    }

    public RecruitData SerializeSelf()
    {
        RecruitData data = new();

        data.RecruitState = RecruitState.ToString();
        data.CharacterData = Character.SerializeSelf();

        return data;
    }
}


[Serializable]
public struct RecruitData
{
    public string RecruitState;
    public CharacterData CharacterData;
    }
