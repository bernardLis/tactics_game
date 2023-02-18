using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Character/Injury")]
public class Injury : BaseScriptableObject
{
    public float DaysInjured; // static

    [HideInInspector] public DateTime DateTimeStarted;
    [HideInInspector] public bool IsHealed;

    public event Action OnInjuryHealed;
    public void Healed()
    {
        IsHealed = true;
        OnInjuryHealed?.Invoke();
    }

    public int GetTotalInjuryTimeInSeconds()
    {

        CampBuildingHospital h = GameManager.Instance.GetComponent<BuildingManager>().HospitalBuilding;
        float improvement = (100 - h.GetUpgradeByRank(h.UpgradeRank).PercentHealingImprovement) * 0.01f;
        Debug.Log($"Improvement {improvement}");

        int totalTime = Mathf.FloorToInt(DaysInjured * GameManager.SecondsInDay * improvement);
        Debug.Log($"total time {totalTime}");
        return totalTime;
    }

    public void CreateFromData(InjuryData data)
    {
        IsHealed = data.IsHealed;
        DateTimeStarted = ScriptableObject.CreateInstance<DateTime>();
        DateTimeStarted.LoadFromData(data.DateTimeStarted);
    }

    public InjuryData SerializeSelf()
    {
        InjuryData d = new();
        d.Id = Id;
        d.IsHealed = IsHealed;
        d.DateTimeStarted = DateTimeStarted.SerializeSelf();
        return d;
    }
}

[System.Serializable]
public struct InjuryData
{
    public string Id;
    public bool IsHealed;
    public DateTimeData DateTimeStarted;
}


