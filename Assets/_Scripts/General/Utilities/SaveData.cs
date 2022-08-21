using System.Collections.Generic;
using UnityEngine;

// https://www.youtube.com/watch?v=uD7y4T4PVk0\
// C:\Users\blis\AppData\LocalLow\DefaultCompany\Tactics Bu
[System.Serializable]
public class SaveData
{
    // global data
    public bool WasTutorialPlayed;
    public int Obols;
    public List<string> PurchasedGlobalUpgrades = new();

    // run data
    public string LastLevel;
    public int JourneySeed; // this reproduces paths with bridges and nodes 
    public int Gold;
    public int SavingsAccountGold;
    public int TotalInterestEarned;
    public List<JourneyNodeData> VisitedJourneyNodes = new();
    public JourneyNodeData CurrentJourneyNode;

    public List<CharacterData> Characters = new();
    public List<string> ItemPouch = new();
    public List<string> AbilityPouch = new();

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


