using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class JourneyManager : MonoBehaviour, ISavable
{
    LevelLoader levelLoader;

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

        levelLoader = GetComponent<LevelLoader>();

        // copy array to list;
        availableEvents = new(allEvents);
        LoadJsonData();
        //LoadPlayerCharacters();
    }

    public void LoadLevel(string level)
    {
        SaveJsonData();
        levelLoader.LoadLevel(level);
    }

    public void SetPlayerTroops(List<Character> troops)
    {
        Debug.Log("Setting player characters");
        playerTroops = new(troops);
    }

    public void JourneyWasSetUp(bool was) // TODO: better naming
    {
        wasJourneySetUp = was;
    }

    public void SetJourneySeed(int s)
    {
        journeySeed = s;
    }

    public void SetCurrentJourneyNode(JourneyNodeData n)
    {
        visitedJourneyNodes.Add(n);
        currentJourneyNode = n;
    }

    public void SetObols(int o)
    {
        obols = o;
        SaveJsonData();
    }

    public void SetNodeReward(JourneyNodeReward r)
    {
        reward = r;
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
        saveData.characters = PopulateCharacters();
    }

    List<CharacterData> PopulateCharacters()
    {
        List<CharacterData> charData = new();
        foreach (Character c in playerTroops)
        {
            CharacterData data = new();
            data.ReferenceID = c.ReferenceID;
            data.CharacterName = c.CharacterName;
            data.Portrait = c.Portrait.name;
            data.Level = c.Level;
            data.Strength = c.Strength;
            data.Intelligence = c.Intelligence;
            data.Agility = c.Agility;
            data.Stamina = c.Stamina;
            data.Body = c.Body.name;
            data.Weapon = c.Weapon.name;

            List<string> abilityReferenceIds = new();
            foreach (Ability a in c.Abilities)
                abilityReferenceIds.Add(a.ReferenceID);
            data.AbilityReferenceIds = new(abilityReferenceIds);

            charData.Add(data);
        }

        return charData;
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
        playerTroops = new();
        foreach (CharacterData data in saveData.characters)
        {
            Character playerCharacter = (Character)ScriptableObject.CreateInstance<Character>();
            playerCharacter.Create(data);
            playerTroops.Add(playerCharacter);
        }

        // TODO: probably here I need to create Characters from save data 
        // and populate troops
    }

    public void ClearSaveData()
    {
        if (FileManager.WriteToFile("SaveData.dat", ""))
            Debug.Log("Cleared SaveData");
    }

}
