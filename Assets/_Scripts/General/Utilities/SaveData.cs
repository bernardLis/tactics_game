using System.Collections.Generic;
using UnityEngine;

// https://www.youtube.com/watch?v=uD7y4T4PVk0\
// C:\Users\blis\AppData\LocalLow\DefaultCompany\Tactics Bu
[System.Serializable]
public class SaveData
{
    // global data
    public int Seed;

    public float SecondsLeftInDay;
    public int Day;
    public int Gold;
    public int Spice;

    public List<CharacterData> PlayerTroops = new();
    public List<ItemData> ItemPouch = new();
    public List<AbilityData> AbilityPouch = new();

    public List<ReportData> Reports = new();
    public List<ReportData> ReportsArchived = new();

    public List<CampBuildingData> CampBuildings = new();
    public List<AbilityNodeGraphData> AbilityNodeGraphs = new();

    public string ToJson() { return JsonUtility.ToJson(this); }

    public void LoadFromJson(string jsonString) { JsonUtility.FromJsonOverwrite(jsonString, this); }
}

public interface ISavable
{
    void PopulateSaveData(SaveData saveData);
    void LoadFromSaveData(SaveData saveData);
}


