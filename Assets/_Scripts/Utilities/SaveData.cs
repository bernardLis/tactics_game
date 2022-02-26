using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Threading.Tasks;

// https://www.youtube.com/watch?v=uD7y4T4PVk0
[System.Serializable]
public class SaveData
{
    public int journeySeed; // this reproduces paths with bridges and nodes // how do I keep the reference to visited nodes?
    public int obols;
    public List<JourneyNodeData> visitedJourneyNodes = new();
    public JourneyNodeData currentJourneyNode;
    // TODO: what do I want to save? Characters...
    public List<CharacterData> characters = new();

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


