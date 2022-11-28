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

    public Sound BuildingSound;

    [HideInInspector] public CampBuildingState CampBuildingState;
    [HideInInspector] public int DaysLeftToBuild;
    [HideInInspector] public int DayStartedBuilding;

    public event Action<CampBuildingState> OnCampBuildingStateChanged;
    public void UpdateCampBuildingState(CampBuildingState newState)
    {
        CampBuildingState = newState;
        switch (newState)
        {
            case CampBuildingState.Pending:
                break;
            case CampBuildingState.Started:
                break;
            case CampBuildingState.Finished:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        }
        OnCampBuildingStateChanged?.Invoke(newState);
    }

    public void Initialize()
    {
        if (CampBuildingState == CampBuildingState.Finished)
            return;

        _gameManager = GameManager.Instance;
        _gameManager.OnDayPassed += OnDayPassed;
    }

    public void StartBuilding()
    {
        UpdateCampBuildingState(CampBuildingState.Started);
        DaysLeftToBuild = DaysToBuild;
        DayStartedBuilding = _gameManager.Day;
        _gameManager.GetComponent<AudioManager>().PlaySFX(BuildingSound, Vector3.one);
    }

    public void OnDayPassed(int day)
    {
        if (CampBuildingState != CampBuildingState.Started)
            return;

        DaysLeftToBuild--;
        if (DaysLeftToBuild == 0)
            FinishBuilding();
    }

    public virtual void FinishBuilding()
    {
        UpdateCampBuildingState(CampBuildingState.Finished);
        _gameManager.OnDayPassed -= OnDayPassed;
    }

    public void ResetSelf()
    {
        DaysLeftToBuild = DaysToBuild;
        DayStartedBuilding = 0;
        CampBuildingState = CampBuildingState.Pending;
    }

    public void LoadFromData(CampBuildingData data)
    {
        Initialize();
        CampBuildingState = (CampBuildingState)System.Enum.Parse(typeof(CampBuildingState), data.CampBuildingState);

        DaysLeftToBuild = data.DaysLeftToBuild;
        DayStartedBuilding = data.DayStartedBuilding;
    }

    public CampBuildingData SerializeSelf()
    {
        CampBuildingData data = new();

        data.Id = Id;
        data.CampBuildingState = CampBuildingState.ToString();
        data.DaysLeftToBuild = DaysLeftToBuild;
        data.DayStartedBuilding = DayStartedBuilding;

        return data;
    }
}

[Serializable]
public struct CampBuildingData
{
    public string Id;

    public string CampBuildingState;
    public int DaysLeftToBuild;
    public int DayStartedBuilding;
}
