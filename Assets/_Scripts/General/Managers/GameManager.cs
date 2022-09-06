using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : PersistentSingleton<GameManager>, ISavable
{
    LevelLoader _levelLoader;
    RunManager _runManager;

    public GameDatabase GameDatabase;

    // global data
    public int Obols; //{ get; private set; }
    public List<GlobalUpgrade> PurchasedGlobalUpgrades { get; private set; }
    public bool WasTutorialPlayed { get; private set; }
    bool _isRunActive;

    // game data
    public string PreviousLevel { get; private set; }
    string _currentLevel;

    public event Action<int> OnObolsChanged;
    public event Action<string> OnLevelLoaded;

    protected override void Awake()
    {
        base.Awake();
        _levelLoader = GetComponent<LevelLoader>();
        _runManager = GetComponent<RunManager>();

        PreviousLevel = Scenes.MainMenu;
        PurchasedGlobalUpgrades = new();

        // global save per 'game'
        if (PlayerPrefs.GetString("saveName").Length == 0)
            CreateNewSaveFile();
        else
            LoadFromSaveFile();
    }

    public void ChangeObolValue(int o)
    {
        Obols += o;
        OnObolsChanged?.Invoke(Obols);
        SaveJsonData();
    }

    public void PurchaseGlobalUpgrade(GlobalUpgrade upgrade)
    {
        PurchasedGlobalUpgrades.Add(upgrade);
        ChangeObolValue(-upgrade.Price);
        SaveJsonData();
    }

    public bool IsGlobalUpgradePurchased(GlobalUpgrade upgrade) { return PurchasedGlobalUpgrades.Contains(upgrade); }

    public void StartNewRun()
    {
        ClearRunData();
        _runManager.InitializeNewRun();
        _isRunActive = true;
        if (WasTutorialPlayed)
            _levelLoader.LoadLevel(Scenes.Journey);
        else
            _levelLoader.LoadLevel(Scenes.Cutscene);
    }

    public bool IsRunActive() { return _isRunActive; }

    public void ResumeLastRun() { _levelLoader.LoadLevel(Scenes.Journey); }

    public void SetPreviousLevel(string level) { PreviousLevel = level; }

    public void SetWasTutorialPlayer(bool was)
    {
        WasTutorialPlayed = was;
        SaveJsonData();
    }

    public void LoadLevel(string level)
    {
        if (level == Scenes.Journey) // TODO: I want to save only on coming back to Journey, does it make sense?
            SaveJsonData();

        _levelLoader.LoadLevel(level);
        OnLevelLoaded?.Invoke(level);
    }

    /*************
    * Saving and Loading
    * https://www.youtube.com/watch?v=uD7y4T4PVk0
    */

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
    public void LoadFromSaveFile() { LoadJsonData(PlayerPrefs.GetString("saveName")); }

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
        saveData.AvailableEvents = PopulateAvailableEvents();
        saveData.LastLevel = SceneManager.GetActiveScene().name;
        saveData.Gold = _runManager.Gold;
        saveData.SavingsAccountGold = _runManager.SavingsAccountGold;
        saveData.TotalInterestEarned = _runManager.InterestEarned;
        saveData.JourneySeed = _runManager.JourneySeed;
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

    List<string> PopulateAvailableEvents()
    {
        List<string> availableEventIds = new();
        foreach (JourneyEvent e in _runManager.AvailableEvents)
            availableEventIds.Add(e.Id);

        return availableEventIds;
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
            data.MaxHealth = c.MaxHealth;
            data.MaxMana = c.MaxMana;
            data.Armor = c.Armor;
            data.MovementRange = c.MovementRange;
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
            PurchasedGlobalUpgrades.Add(GameDatabase.GetGlobalUpgradeById(savedId));

        WasTutorialPlayed = saveData.WasTutorialPlayed;

        // run data
        if (saveData.JourneySeed != 0 && saveData.WasTutorialPlayed)
            _isRunActive = true;

        _runManager.PopulateRunFromSaveData(saveData);

        _currentLevel = saveData.LastLevel;
    }

    public void ClearRunData()
    {
        SaveData sd = new SaveData();

        // run data
        _isRunActive = false;

        // global data
        sd.Obols = Obols;
        sd.PurchasedGlobalUpgrades = PopulatePurchasedGlobalUpgrades();
        sd.WasTutorialPlayed = WasTutorialPlayed;

        // save data
        sd.LastLevel = "";
        sd.Gold = 0;
        sd.SavingsAccountGold = 0;
        sd.TotalInterestEarned = 0;
        sd.JourneySeed = 0;
        sd.VisitedJourneyNodes = null;
        sd.Characters = null;
        sd.ItemPouch = null;
        sd.AbilityPouch = null;

        if (FileManager.WriteToFile(PlayerPrefs.GetString("saveName"), sd.ToJson()))
            Debug.Log("Save successful");

    }

    public void ClearSaveData()
    {
        PurchasedGlobalUpgrades = new();
        Obols = 0;
        WasTutorialPlayed = false;
        _isRunActive = false;

        if (FileManager.WriteToFile(PlayerPrefs.GetString("saveName"), ""))
            Debug.Log("Cleared active save");

        LoadLevel(Scenes.MainMenu);
    }

}
