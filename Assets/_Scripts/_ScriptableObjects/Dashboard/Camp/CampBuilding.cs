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
    [HideInInspector] public bool IsBuildingStarted;
    [HideInInspector] public bool IsBuilt;

    public void Initialize()
    {
        Debug.Log($"Initialize {IsBuildingStarted}");
        if (IsBuilt)
            return;

        _gameManager = GameManager.Instance;
        _gameManager.OnDayPassed += OnDayPassed;
    }


    public void StartBuilding()
    {
        Debug.Log($"start building");
        IsBuildingStarted = true;
        DaysLeftToBuild = DaysToBuild;
        DayStartedBuilding = _gameManager.Day;
    }

    public void OnDayPassed(int day)
    {
        Debug.Log($"on day passed");
        if (!IsBuildingStarted || IsBuilt)
            return;

        DaysLeftToBuild--;
        if (DaysLeftToBuild == 0)
            FinishBuilding();
    }

    public virtual void FinishBuilding()
    {
        IsBuilt = true;
        _gameManager.OnDayPassed -= OnDayPassed;
    }

    public void ResetSelf()
    {
        DaysLeftToBuild = DaysToBuild;
        DayStartedBuilding = 0;
        IsBuildingStarted = false;
        IsBuilt = false;
    }

    public void LoadFromData(CampBuildingData data)
    {
        Initialize();
        IsBuildingStarted = data.IsBuildingStarted;
        DaysLeftToBuild = data.DaysLeftToBuild;
        DayStartedBuilding = data.DayStartedBuilding;
        IsBuilt = data.IsBuilt;
    }

    public CampBuildingData SerializeSelf()
    {
        CampBuildingData data = new();

        data.Id = Id;
        data.IsBuildingStarted = IsBuildingStarted;
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

    public bool IsBuildingStarted;
    public int DaysLeftToBuild;
    public int DayStartedBuilding;

    public bool IsBuilt;
}
