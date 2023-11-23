using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Global Upgrades/Global Upgrade")]
public class GlobalUpgrade : BaseScriptableObject
{
    public Sprite Icon;
    public string Description;

    public List<GlobalUpgradeLevel> Levels;
    public int CurrentLevel = 0;

    GlobalUpgradeBoard _board;
    public event Action OnLevelChanged;
    public virtual void Initialize(GlobalUpgradeBoard board)
    {
        _board = board;
        _board.OnRefundAll += Refund;
    }

    public virtual void Purchased()
    {
        CurrentLevel++;
        OnLevelChanged?.Invoke();
    }

    public GlobalUpgradeLevel GetCurrentLevel()
    {
        return Levels[CurrentLevel];
    }

    public GlobalUpgradeLevel GetNextLevel()
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
        OnLevelChanged?.Invoke();
    }


}


