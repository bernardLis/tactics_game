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

    [Tooltip("0 - not built, 1 built and then upgrades")]
    public Vector2 UpgradeRange; // static

    public Sound BuildingSound; // static 

    [HideInInspector] public CampBuildingState CampBuildingState;
    [HideInInspector] public int DaysLeftToBuild;
    [HideInInspector] public int DayStartedBuilding;
    [HideInInspector] public int UpgradeLevel;

    public event Action<CampBuildingState> OnCampBuildingStateChanged;
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

    public void Initialize()
    {
        _gameManager = GameManager.Instance;
        _gameManager.OnDayPassed += OnDayPassed;
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
        UpdateCampBuildingState(CampBuildingState.Built);
        UpgradeLevel = 1;
    }

    public virtual int GetUpgradeCost()
    {
        return Mathf.RoundToInt(CostToBuild * UpgradeLevel * 1.5f);
    }

    public virtual void Upgrade()
    {
        UpgradeLevel++;
    }

    public void ResetSelf()
    {
        DaysLeftToBuild = DaysToBuild;
        DayStartedBuilding = 0;
        CampBuildingState = CampBuildingState.NotBuilt;
        UpgradeLevel = 0;
    }

    public void LoadFromData(CampBuildingData data)
    {
        Initialize();
        CampBuildingState = (CampBuildingState)System.Enum.Parse(typeof(CampBuildingState), data.CampBuildingState);

        DaysLeftToBuild = data.DaysLeftToBuild;
        DayStartedBuilding = data.DayStartedBuilding;
        UpgradeLevel = data.UpgradeLevel;
    }

    public CampBuildingData SerializeSelf()
    {
        CampBuildingData data = new();

        data.Id = Id;
        data.CampBuildingState = CampBuildingState.ToString();
        data.DaysLeftToBuild = DaysLeftToBuild;
        data.DayStartedBuilding = DayStartedBuilding;
        data.UpgradeLevel = UpgradeLevel;

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
    public int UpgradeLevel;

}
