using System.Collections.Generic;
using UnityEngine;

// https://www.youtube.com/watch?v=uD7y4T4PVk0\
// C:\Users\blis\AppData\LocalLow\DefaultCompany\Tactics Bu
[System.Serializable]
public class SaveData
{
    // global data
    public bool WasTutorialPlayed;
    public int Seed;

    public int Gold;
    public int Day;

    public List<string> ShopItems = new();
    public int ShopRerollPrice;

    public List<CharacterData> PlayerTroops = new();
    public List<string> ItemPouch = new();
    public List<string> AbilityPouch = new();

    public List<QuestData> NewQuests = new();
    public List<QuestData> AvailableQuests = new();

    public List<ReportData> Reports = new();
    public List<ReportData> ReportsArchived = new();

    public string ToJson()
    {
        return JsonUtility.ToJson(this);
    }

    public void LoadFromJson(string jsonString)
    {
        JsonUtility.FromJsonOverwrite(jsonString, this);
    }
}

public interface ISavable
{
    void PopulateSaveData(SaveData saveData);
    void LoadFromSaveData(SaveData saveData);
}


