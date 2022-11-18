using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CampBuilding : BaseScriptableObject
{
    protected GameManager _gameManager;

    public Sprite OutlineSprite; // static
    public Sprite BuiltSprite; // static

    public int CostToBuild; // static
    public int DaysToBuild; // static

    [HideInInspector] public int DaysLeftToBuild;
    [HideInInspector] public int DayStartedBuilding;

    [HideInInspector] public bool IsBuilt;

    public void Initialize()
    {

    }

    public void StartBuilding()
    {
        DaysLeftToBuild = DaysToBuild;
        DayStartedBuilding = _gameManager.Day;
    }

    public void OnDayPassed(int day)
    {
        DaysLeftToBuild--;
        if (DaysLeftToBuild == 0)
            FinishBuilding();
    }

    public virtual void FinishBuilding() { IsBuilt = true; } // TODO: here I need to fire the effect

    public void LoadFromData(CampBuildingData data)
    {
        _gameManager = GameManager.Instance;
        _gameManager.OnDayPassed += OnDayPassed;

        DaysLeftToBuild = data.DaysLeftToBuild;
        DayStartedBuilding = data.DayStartedBuilding;
        IsBuilt = data.IsBuilt;
    }

    public CampBuildingData SerializeSelf()
    {
        CampBuildingData data = new();

        data.Id = Id;
        data.DaysLeftToBuild = DaysLeftToBuild;
        data.DayStartedBuilding = DayStartedBuilding;
        data.IsBuilt = IsBuilt;

        return data;
    }
}

[Serializable]
public struct CampBuildingData
{
    public string Id;

    public int DaysLeftToBuild;
    public int DayStartedBuilding;

    public bool IsBuilt;
}
