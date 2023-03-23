using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : BaseScriptableObject
{
    public Sprite OutlineSprite;
    public Sprite BuiltSprite;

    public int Price;
    public bool IsBuilt;

    protected GameManager _gameManager;
    public virtual void Initialize()
    {
        _gameManager = GameManager.Instance;
        _gameManager.OnDayPassed += OnDayPassed;
    }

    public virtual void OnDayPassed(int day)
    {
        // meant to be overwritten
    }

    public virtual void Produce()
    {
        // meant to be overwritten
    }

    public virtual BuildingData SerializeSelf()
    {
        BuildingData data = new();
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
    public bool IsBuilt;
    public int AvailableToBuyCount;
}

