using System.Collections.Generic;
using UnityEngine;

// https://www.youtube.com/watch?v=uD7y4T4PVk0\
// C:\Users\blis\AppData\LocalLow\DefaultCompany\Tactics Bu
[System.Serializable]
public class SaveData
{
    public string LastLevel;
    public int CutSceneIndex; // TODO: I doubt it is the correct way yo handle it.
    public string PlayerName;
    public int JourneySeed; // this reproduces paths with bridges and nodes 
    public int Obols;
    public List<JourneyNodeData> VisitedJourneyNodes = new();
    public JourneyNodeData CurrentJourneyNode;
    public List<CharacterData> Characters = new();

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


