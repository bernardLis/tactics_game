using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CampBuilding : BaseScriptableObject
{
    protected GameManager _gameManager;

    public string DisplayName;

    public Sprite OutlineSprite; // static
    public Sprite BuiltSprite; // static

    public int CostToBuild; // static
    public int DaysToBuild; // static
    public Sound BuildingSound; // static 

    [HideInInspector] public CampBuildingState CampBuildingState;
    [HideInInspector] public int DaysLeftToBuild;
    [HideInInspector] public int DayStartedBuilding;
    [HideInInspector] public int UpgradeRank;

    public event Action<CampBuildingState> OnCampBuildingStateChanged;
    public event Action<int> OnUpgraded;

    public virtual void Initialize()
    {
        _gameManager = GameManager.Instance;
        _gameManager.OnDayPassed += OnDayPassed;
    }

    public void UpdateCampBuildingState(CampBuildingState newState)
    {
        CampBuildingState = newState;
        switch (newState)
        {
            case CampBuildingState.NotBuilt:
                break;
            case CampBuildingState.Building:
                break;
            case CampBuildingState.Built:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        }
        OnCampBuildingStateChanged?.Invoke(newState);
    }

    public void StartBuilding()
    {
        UpdateCampBuildingState(CampBuildingState.Building);
        DaysLeftToBuild = DaysToBuild;
        DayStartedBuilding = _gameManager.Day;
        _gameManager.GetComponent<AudioManager>().PlaySFX(BuildingSound, Vector3.one);
    }

    public void OnDayPassed(int day)
    {
        if (CampBuildingState == CampBuildingState.Building)
        {
            DaysLeftToBuild--;
            if (DaysLeftToBuild == 0)
                FinishBuilding();
        }
    }

    public virtual void FinishBuilding()
    {
        Upgrade();
        UpdateCampBuildingState(CampBuildingState.Built);
    }

    public virtual int GetMaxUpgradeRank()
    {
        // meant to be overwritten
        return 0;
    }

    public virtual int GetUpgradeCost() { return Mathf.RoundToInt(CostToBuild * UpgradeRank * 1.5f); }

    public virtual void Upgrade()
    {
        UpgradeRank++;
        OnUpgraded?.Invoke(UpgradeRank);
        _gameManager.SaveJsonData();
    }

    public void ResetSelf()
    {
        DaysLeftToBuild = DaysToBuild;
        DayStartedBuilding = 0;
        CampBuildingState = CampBuildingState.NotBuilt;
        UpgradeRank = 0;
    }

    public void LoadFromData(CampBuildingData data)
    {
        Initialize();
        CampBuildingState = (CampBuildingState)System.Enum.Parse(typeof(CampBuildingState), data.CampBuildingState);

        DaysLeftToBuild = data.DaysLeftToBuild;
        DayStartedBuilding = data.DayStartedBuilding;
        UpgradeRank = data.UpgradeRank;
    }

    public CampBuildingData SerializeSelf()
    {
        CampBuildingData data = new();

        data.Id = Id;
        data.CampBuildingState = CampBuildingState.ToString();
        data.DaysLeftToBuild = DaysLeftToBuild;
        data.DayStartedBuilding = DayStartedBuilding;
        data.UpgradeRank = UpgradeRank;

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
    public int UpgradeRank;
}
