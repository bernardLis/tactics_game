using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class JourneyManager : MonoBehaviour, ISavable
{
    public List<Character> playerTroops;
    public bool wasJourneySetUp { get; private set; }
    public int journeySeed { get; private set; } = 0; // TODO: this is a bad idea, probably
    public JourneyNodeData currentJourneyNode { get; private set; }
    public int obols { get; private set; }
    public JourneyNodeReward reward { get; private set; }

    [Header("Unity Setup")]
    public JourneyEvent[] allEvents;

    List<JourneyEvent> availableEvents;

    [HideInInspector] public List<JourneyPath> journeyPaths = new();
    public List<JourneyNodeData> visitedJourneyNodes = new();

    public static JourneyManager instance;
    void Awake()
    {
        #region Singleton
        // singleton
        if (instance != null && instance != this)
        {
            // TODO: it gets here on scene transitions, should I do something about it?
            Debug.LogWarning("More than one instance of JourneyManager found");
            Destroy(this.gameObject);
            return;
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        #endregion

        // copy array to list;
        availableEvents = new(allEvents);
        LoadJsonData();
        //LoadPlayerCharacters();
    }

    public void SetPlayerTroops(List<Character> _troops)
    {
        // TODO: I imagine that player characters are set-up somewhere and I can load them
        Debug.Log("Loading player characters");

        playerTroops = new(_troops);
    }

    public void JourneyWasSetUp(bool _was) // TODO: better naming
    {
        wasJourneySetUp = _was;
    }

    public void SetJourneySeed(int _s)
    {
        journeySeed = _s;
    }

    public void SetCurrentJourneyNode(JourneyNodeData _n)
    {
        visitedJourneyNodes.Add(_n);
        currentJourneyNode = _n;
    }

    public void SetObols(int _o)
    {
        obols = _o;
        SaveJsonData();
    }

    public void SetNodeReward(JourneyNodeReward _r)
    {
        reward = _r;
    }


    public JourneyEvent ChooseEvent()
    {
        JourneyEvent ev = availableEvents[Random.Range(0, availableEvents.Count)];
        availableEvents.Remove(ev);
        return ev;
    }

    /*************
    * Saving and Loading
    * https://www.youtube.com/watch?v=uD7y4T4PVk0
    */

    public void SaveJsonData()
    {
        SaveData sd = new SaveData();
        PopulateSaveData(sd);
        if (FileManager.WriteToFile("SaveData.dat", sd.ToJson()))
            Debug.Log("save successful");
    }

    public void PopulateSaveData(SaveData saveData)
    {
        saveData.obols = obols;
        saveData.journeySeed = journeySeed;
        saveData.currentJourneyNode = currentJourneyNode;
        saveData.visitedJourneyNodes = visitedJourneyNodes;
    }

    public void LoadJsonData()
    {
        if (FileManager.LoadFromFile("SaveData.dat", out var json))
        {
            SaveData sd = new SaveData();
            sd.LoadFromJson(json);

            LoadFromSaveData(sd);
            Debug.Log("load complete");
        }
    }

    public void LoadFromSaveData(SaveData saveData)
    {
        obols = saveData.obols;
        journeySeed = saveData.journeySeed;
        currentJourneyNode = saveData.currentJourneyNode;
        visitedJourneyNodes = saveData.visitedJourneyNodes;
    }

    public void ClearSaveData()
    {
        if (FileManager.WriteToFile("SaveData.dat", ""))
            Debug.Log("Cleared SaveData");
    }

}
