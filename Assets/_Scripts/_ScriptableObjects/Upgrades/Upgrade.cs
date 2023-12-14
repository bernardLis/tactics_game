using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Upgrades/Upgrade")]
public class Upgrade : BaseScriptableObject
{
    GameManager _gameManager;

    public Sprite Icon;
    public string Description;
    public UpgradeType Type;

    public List<UpgradeLevel> Levels;
    public int CurrentLevel = -1;
    public bool PermanentlyUnlocked;

    UpgradeBoard _board;
    bool _isInitialized;

    public event Action OnLevelChanged;
    public virtual void Initialize(UpgradeBoard board)
    {
        _isInitialized = true;
        _gameManager = GameManager.Instance;
        _board = board;
        _board.OnRefundAll += Refund;

        // perma unlocked
        if (CurrentLevel > 0) return;
        if (PermanentlyUnlocked) CurrentLevel = 0;
    }

    // HERE: testing
    public void DebugInitialize()
    {
        if (_isInitialized) return;
        _isInitialized = true;

        // perma unlocked
        if (CurrentLevel > 0) return;
        if (PermanentlyUnlocked) CurrentLevel = 0;
    }



    public virtual void Purchased()
    {
        CurrentLevel++;
        OnLevelChanged?.Invoke();
        _gameManager.SaveJsonData();
    }

    public UpgradeLevel GetCurrentLevel()
    {
        return Levels[CurrentLevel];
    }

    public UpgradeLevel GetNextLevel()
    {
        if (IsMaxLevel()) return null;

        return Levels[CurrentLevel + 1];
    }

    public bool IsMaxLevel()
    {
        return CurrentLevel == Levels.Count - 1;
    }

    public int GetValue()
    {
        int val = 0;
        for (int i = 0; i < Levels.Count; i++)
            if (i <= CurrentLevel)
                val += Levels[i].Value;
        return val;
    }

    public void Refund()
    {
        int val = 0;
        for (int i = 0; i < Levels.Count; i++)
            if (i <= CurrentLevel)
                val += Levels[i].Cost;
        GameManager.Instance.ChangeGoldValue(val);
        CurrentLevel = -1;
        if (PermanentlyUnlocked) CurrentLevel = 0;
        OnLevelChanged?.Invoke();
    }


    public UpgradeData SerializeSelf()
    {
        UpgradeData data = new()
        {
            Name = name,
            Level = CurrentLevel
        };

        return data;

    }
    public void LoadFromData(UpgradeData data)
    {
        CurrentLevel = data.Level;
    }
}

[Serializable]
public struct UpgradeData
{
    public string Name;
    public int Level;
}

public enum UpgradeType { Other, Hero, Building, Creature, Boss, Ability }
