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

    public virtual void Initialize()
    {
    }

    public virtual void Purchased()
    {
        CurrentLevel++;
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


}


