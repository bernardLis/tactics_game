using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : BaseScriptableObject
{
    public string DisplayName;

    public Sprite OutlineSprite;
    public Sprite BuiltSprite;

    public int CostToBuild;
    public bool IsBuilt;

    protected GameManager _gameManager;
    public virtual void Initialize()
    {
        _gameManager = GameManager.Instance;
        _gameManager.OnDayPassed += OnDayPassed;
    }

    public virtual void Build() { IsBuilt = true; }

    public virtual void Reset() { IsBuilt = false; }

    public virtual void OnDayPassed(int day)
    {
        // meant to be overwritten
    }

    public virtual void Produce(int count)
    {
        // meant to be overwritten
    }

    public virtual string GetDescription()
    {
        // meant to be overwritten
        return "";
    }

    public virtual BuildingData SerializeSelf()
    {
        BuildingData data = new();
        data.Id = Id;
        data.IsBuilt = IsBuilt;
        return data;
    }

    public virtual void LoadFromData(BuildingData data)
    {
        IsBuilt = data.IsBuilt;
    }
}

[System.Serializable]
public struct BuildingData
{
    public string Id;
    public bool IsBuilt;
    public int AvailableToBuyCount;
}

