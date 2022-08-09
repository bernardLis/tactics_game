using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : PersistentSingleton<GameManager>, ISavable
{
    LevelLoader _levelLoader;
    RunManager _runManager;

    // TODO: better set-up, something needs to track where you are in the game and save/load the info
    public bool WasTutorialPlayed { get; private set; }
    bool _isRunActive;

    public int Obols { get; private set; }


    [Header("Unity Setup")]
    public List<GlobalUpgrade> AllGlobalUpgrades;
    public List<GlobalUpgrade> PurchasedGlobalUpgrades;

    public CharacterDatabase CharacterDatabase;
    public JourneyEvent[] AllEvents;

    public string PreviousLevel { get; private set; }
    string _currentLevel;

    public event Action<int> OnObolsChanged;

    protected override void Awake()
    {
        base.Awake();
        _levelLoader = GetComponent<LevelLoader>();
        _runManager = GetComponent<RunManager>();

        PreviousLevel = Scenes.MainMenu;

        // global save per 'game'
        if (PlayerPrefs.GetString("saveName").Length == 0)
            CreateNewSaveFile();
        else
            LoadFromSaveFile();
    }

    void CreateNewSaveFile()
    {
        // new save
        string guid = System.Guid.NewGuid().ToString();
        string fileName = guid + ".dat";
        FileManager.CreateFile(fileName);
        PlayerPrefs.SetString("saveName", fileName);
        PlayerPrefs.Save();

        SaveJsonData();
    }

    public void LoadFromSaveFile()
    {
        LoadJsonData(PlayerPrefs.GetString("saveName"));
    }

    public void PurchaseGlobalUpgrade(GlobalUpgrade upgrade)
    {
        PurchasedGlobalUpgrades.Add(upgrade);
        ChangeObolValue(-upgrade.Price);
        SaveJsonData();
    }

    public bool IsGlobalUpgradePurchased(GlobalUpgrade upgrade)
    {
        return PurchasedGlobalUpgrades.Contains(upgrade);
    }

    public void StartNewRun()
    {
        ClearRunData();
        _runManager.InitializeNewRun();
        // check whether player beat tutorial or not
        if (WasTutorialPlayed)
            _levelLoader.LoadLevel(Scenes.Journey);
        else
            _levelLoader.LoadLevel(Scenes.Cutscene);
    }

    public bool IsRunActive() { return _isRunActive; }

    public void ResumeLastRun()
    {
        _levelLoader.LoadLevel(Scenes.Journey);
    }

    public void LoadLevel(string level)
    {
        if (level == Scenes.Journey) // TODO: I want to save only on coming back to Journey, does it make sense?
            SaveJsonData();

        _levelLoader.LoadLevel(level);
    }

    public void ChangeObolValue(int o)
    {
        Obols += o;
        OnObolsChanged?.Invoke(Obols);
        SaveJsonData();
    }

    public void SetPreviousLevel(string level) { PreviousLevel = level; }

    public void SetWasTutorialPlayer(bool was)
    {
        WasTutorialPlayed = was;
        SaveJsonData();
    }

    /*************
    * Saving and Loading
    * https://www.youtube.com/watch?v=uD7y4T4PVk0
    */

    public void SaveJsonData()
    {
        SaveData sd = new SaveData();
        PopulateSaveData(sd);
        if (FileManager.WriteToFile(PlayerPrefs.GetString("saveName"), sd.ToJson()))
            Debug.Log("Save successful");
    }

    public void PopulateSaveData(SaveData saveData)
    {
        // global data
        saveData.Obols = Obols;
        saveData.WasTutorialPlayed = WasTutorialPlayed;
        saveData.PurchasedGlobalUpgrades = PopulatePurchasedGlobalUpgrades();

        // run data
        saveData.LastLevel = SceneManager.GetActiveScene().name;
        saveData.Gold = _runManager.Gold;
        saveData.JourneySeed = _runManager.JourneySeed;
        saveData.CurrentJourneyNode = _runManager.CurrentJourneyNode;
        saveData.VisitedJourneyNodes = _runManager.VisitedJourneyNodes;
        saveData.Characters = PopulateCharacters();
        saveData.ItemPouch = PopulateItemPouch();
        saveData.AbilityPouch = PopulateAbilityPouch();
    }

    List<string> PopulatePurchasedGlobalUpgrades()
    {
        List<string> ids = new();
        foreach (GlobalUpgrade upgrade in PurchasedGlobalUpgrades)
            ids.Add(upgrade.Id);

        return ids;
    }

    List<CharacterData> PopulateCharacters()
    {
        List<CharacterData> charData = new();
        foreach (Character c in _runManager.PlayerTroops)
        {
            CharacterData data = new();
            data.ReferenceID = c.ReferenceID;
            data.CharacterName = c.CharacterName;
            data.Portrait = c.Portrait.name;
            data.Level = c.Level;
            data.Experience = c.Experience;
            data.Power = c.Power;
            data.Body = c.Body.name;
            data.Weapon = c.Weapon.name;

            List<string> abilityReferenceIds = new();
            foreach (Ability a in c.Abilities)
                abilityReferenceIds.Add(a.ReferenceID);
            data.AbilityReferenceIds = new(abilityReferenceIds);

            List<string> itemReferenceIds = new();
            foreach (Item i in c.Items)
                itemReferenceIds.Add(i.ReferenceID);
            data.ItemReferenceIds = new(itemReferenceIds);

            charData.Add(data);
        }

        return charData;
    }

    List<string> PopulateItemPouch()
    {
        List<string> itemReferenceIds = new();
        foreach (Item i in _runManager.PlayerItemPouch)
            itemReferenceIds.Add(i.ReferenceID);

        return itemReferenceIds;
    }

    List<string> PopulateAbilityPouch()
    {
        List<string> abilityReferenceIds = new();
        foreach (Ability a in _runManager.PlayerAbilityPouch)
            abilityReferenceIds.Add(a.ReferenceID);

        return abilityReferenceIds;
    }


    void LoadJsonData(string fileName)
    {
        if (FileManager.LoadFromFile(fileName, out var json))
        {
            SaveData sd = new SaveData();
            sd.LoadFromJson(json);

            LoadFromSaveData(sd);
        }
    }

    public void LoadFromSaveData(SaveData saveData)
    {
        // global data
        Obols = saveData.Obols;
        foreach (string savedId in saveData.PurchasedGlobalUpgrades)
            PurchasedGlobalUpgrades.Add(AllGlobalUpgrades.First(x => x.Id == savedId));

        WasTutorialPlayed = saveData.WasTutorialPlayed;

        // run data
        if (saveData.JourneySeed != 0 && saveData.WasTutorialPlayed)
            _isRunActive = true;

        _runManager.Gold = saveData.Gold;
        _runManager.JourneySeed = saveData.JourneySeed;
        _runManager.CurrentJourneyNode = saveData.CurrentJourneyNode;
        _runManager.VisitedJourneyNodes = saveData.VisitedJourneyNodes;
        _runManager.PlayerTroops = new();
        foreach (CharacterData data in saveData.Characters)
        {
            Character playerCharacter = (Character)ScriptableObject.CreateInstance<Character>();
            playerCharacter.Create(data);
            _runManager.PlayerTroops.Add(playerCharacter);
        }

        _runManager.PlayerItemPouch = new();
        foreach (string itemReferenceId in saveData.ItemPouch)
            _runManager.PlayerItemPouch.Add(CharacterDatabase.GetItemByReference(itemReferenceId));

        _runManager.PlayerAbilityPouch = new();
        foreach (string abilityReferenceId in saveData.AbilityPouch)
            _runManager.PlayerAbilityPouch.Add(CharacterDatabase.GetAbilityByReferenceID(abilityReferenceId));

        _currentLevel = saveData.LastLevel;
    }

    public void ClearRunData()
    {
        SaveData sd = new SaveData();

        // global data
        sd.Obols = Obols;
        sd.PurchasedGlobalUpgrades = PopulatePurchasedGlobalUpgrades();

        // run data
        sd.LastLevel = "";
        sd.Gold = 0;
        sd.JourneySeed = 0;
        sd.CurrentJourneyNode = new JourneyNodeData();
        sd.VisitedJourneyNodes = null;
        sd.Characters = null;
        sd.ItemPouch = null;
        sd.AbilityPouch = null;

        if (FileManager.WriteToFile(PlayerPrefs.GetString("saveName"), sd.ToJson()))
            Debug.Log("Save successful");
    }


    public void ClearSaveData()
    {
        if (FileManager.WriteToFile(PlayerPrefs.GetString("saveName"), ""))
            Debug.Log("Cleared active save");
    }

}
