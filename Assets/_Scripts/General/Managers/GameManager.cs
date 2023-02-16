using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class GameManager : PersistentSingleton<GameManager>, ISavable
{
    public const float SecondsInDay = 20;

    LevelLoader _levelLoader;
    BuildingManager _buildingManager;

    public GameDatabase GameDatabase;

    SaveData _originalSaveData;

    // settings
    public bool HideMenuEffects { get; private set; }
    public void SetHideMenuEffects(bool hide) { HideMenuEffects = hide; }

    // global data
    public int Seed { get; private set; }

    public float TotalSeconds { get; private set; }
    public int Day { get; private set; }
    public float SecondsLeftInDay { get; private set; }
    public int Gold { get; private set; }
    public int Spice { get; private set; }

    public bool IsTimerOn { get; private set; }

    public List<Character> PlayerTroops = new();
    [HideInInspector] public List<Item> PlayerItemPouch = new();

    [HideInInspector] public List<Ability> PlayerAbilityPouch = new();

    public List<Report> Reports = new();
    public List<Report> ReportsArchived = new();

    [SerializeField] List<AbilityNodeGraph> _abilityNodeGraphs = new();

    public int CutsceneIndexToPlay = 0; // TODO: this is wrong, but for now it is ok

    public event Action<Report> OnReportAdded;
    public event Action<int> OnDayPassed;
    public event Action<int> OnGoldChanged;
    public event Action<int> OnSpiceChanged;
    public event Action<Character> OnCharacterAddedToTroops;
    public event Action<Character> OnCharacterRemovedFromTroops;
    public event Action<string> OnLevelLoaded;
    public event Action OnNewSaveFileCreation;
    public event Action OnClearSaveData;
    public event Action<bool> OnTimerStateChanged;
    protected override void Awake()
    {
        Debug.Log($"Game manager Awake");
        base.Awake();
        _levelLoader = GetComponent<LevelLoader>();
        _buildingManager = GetComponent<BuildingManager>();
    }

    void Start()
    {
        Debug.Log($"Game manager Start");
        GameDatabase.Initialize();
        // global save per 'game'
        if (PlayerPrefs.GetString("saveName").Length == 0)
            CreateNewSaveFile();
        else
            LoadFromSaveFile();
    }

    public void Play()
    {
        if (PlayerTroops.Count == 0)
        {
            LoadLevel(Scenes.CharacterCreation);
            return;
        }
        StartGame();
    }

    public void StartGame()
    {
        LoadLevel(Scenes.Dashboard);
        IsTimerOn = true;
    }

    public float GetCurrentTimeInSeconds() { return Day * SecondsInDay + SecondsInDay - SecondsLeftInDay; }

    public void ToggleTimer(bool isOn)
    {
        IsTimerOn = isOn;
        OnTimerStateChanged?.Invoke(IsTimerOn);
    }

    public void AddNewReport(Report r)
    {
        Reports.Add(r);
        OnReportAdded?.Invoke(r);
        SaveJsonData();
    }

    /* RESOURCES */
    public void PassDay()
    {
        Day += 1;

        if (Day % 7 == 0)
            PayWages();

        OnDayPassed?.Invoke(Day);
        SaveJsonData();
    }

    void PayWages()
    {
        ChangeGoldValue(-GetCurrentWages());

        int cost = GetCurrentWages();

        Report r = ScriptableObject.CreateInstance<Report>();
        r.Initialize(ReportType.Wages, null, null, null, null, null, null, null, PlayerTroops);
        AddNewReport(r);
    }

    public int GetCurrentWages()
    {
        int total = 0;
        foreach (Character c in PlayerTroops)
            total += c.WeeklyWage.Value;
        return total;
    }

    public void ChangeGoldValue(int o)
    {
        if (o == 0)
            return;

        Gold += o;
        OnGoldChanged?.Invoke(Gold);
        SaveJsonData();
    }

    public void ChangeSpiceValue(int o)
    {
        if (o == 0)
            return;

        Spice += o;
        OnSpiceChanged?.Invoke(o);
        SaveJsonData();
    }

    public List<AbilityNodeGraph> GetAbilityNodeGraphs() { return _abilityNodeGraphs; }
    public AbilityNodeGraph GetAbilityNodeGraphById(string id) { return _abilityNodeGraphs.FirstOrDefault(x => x.Id == id); }

    /* Troops & pouches */
    public void AddCharacterToTroops(Character character)
    {
        PlayerTroops.Add(character);
        character.SetDayAddedToTroops(Day);
        OnDayPassed += character.OnDayPassed;
        OnCharacterAddedToTroops?.Invoke(character);
        SaveJsonData();
    }

    public void RemoveCharacterFromTroops(Character character)
    {
        PlayerTroops.Remove(character);
        OnDayPassed -= character.OnDayPassed;
        OnCharacterRemovedFromTroops?.Invoke(character);
        SaveJsonData();
    }

    public void AddItemToPouch(Item item)
    {
        PlayerItemPouch.Add(item);
        SaveJsonData();
    }

    public void RemoveItemFromPouch(Item item)
    {
        PlayerItemPouch.Remove(item);
        SaveJsonData();
    }

    public void AddAbilityToPouch(Ability ability)
    {
        PlayerAbilityPouch.Add(ability);
        SaveJsonData();
    }

    public void RemoveAbilityFromPouch(Ability ability)
    {
        PlayerAbilityPouch.Remove(ability);
        SaveJsonData();
    }

    /* LEVELS */

    public void LoadLevel(string level)
    {
        _levelLoader.LoadLevel(level);
        OnLevelLoaded?.Invoke(level);
    }

    /*************
    * Saving and Loading
    * https://www.youtube.com/watch?v=uD7y4T4PVk0
    */
    void CreateNewSaveFile()
    {
        Debug.Log($"Creating new save file...");
        Seed = System.Environment.TickCount;

        SecondsLeftInDay = SecondsInDay;
        Day = 1;
        Gold = 10000;
        Spice = 500;

        foreach (AbilityNodeGraph g in _abilityNodeGraphs)
            g.ResetNodes();

        // new save
        string guid = System.Guid.NewGuid().ToString();
        string fileName = guid + ".dat";
        FileManager.CreateFile(fileName);
        PlayerPrefs.SetString("saveName", fileName);
        PlayerPrefs.Save();

        OnNewSaveFileCreation?.Invoke();
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

    // TODO: prime suspect for a rewrite
    public void PopulateSaveData(SaveData saveData)
    {
        // global data
        saveData.Seed = Seed;

        if (DashboardManager.Instance != null)
            saveData.SecondsLeftInDay = DashboardManager.Instance.DayTimer.GetTimeLeft();

        saveData.Day = Day;
        saveData.Gold = Gold;
        saveData.Spice = Spice;

        saveData.PlayerTroops = PopulateCharacters();

        saveData.ItemPouch = PopulateItemPouch();

        saveData.AbilityPouch = PopulateAbilityPouch();

        saveData.Reports = PopulateReports();
        saveData.ReportsArchived = PopulateArchivedReports();

        saveData.CampBuildings = PopulateCampBuildings();
        saveData.AbilityNodeGraphs = PopulateAbilityNodeGraphs();
    }

    List<CharacterData> PopulateCharacters()
    {
        List<CharacterData> charData = new();
        foreach (Character c in PlayerTroops)
            charData.Add(c.SerializeSelf());

        return charData;
    }

    List<ItemData> PopulateItemPouch()
    {
        List<ItemData> data = new();
        foreach (Item i in PlayerItemPouch)
            data.Add(i.SerializeSelf());

        return data;
    }

    List<AbilityData> PopulateAbilityPouch()
    {
        List<AbilityData> abilityData = new();
        foreach (Ability a in PlayerAbilityPouch)
            abilityData.Add(a.SerializeSelf());

        return abilityData;
    }

    List<ReportData> PopulateReports()
    {
        List<ReportData> reports = new();
        foreach (Report r in Reports)
            reports.Add(r.SerializeSelf());
        return reports;
    }

    List<ReportData> PopulateArchivedReports()
    {
        List<ReportData> reports = new();
        foreach (Report r in ReportsArchived)
            reports.Add(r.SerializeSelf());
        return reports;
    }

    List<CampBuildingData> PopulateCampBuildings()
    {
        List<CampBuildingData> data = new();
        List<CampBuilding> buildings = _buildingManager.GetAllCampBuildings();
        foreach (CampBuilding b in buildings)
            data.Add(b.SerializeSelf());
        return data;
    }

    List<AbilityNodeGraphData> PopulateAbilityNodeGraphs()
    {
        List<AbilityNodeGraphData> data = new();
        foreach (AbilityNodeGraph g in _abilityNodeGraphs)
            data.Add(g.SerializeSelf());
        return data;
    }

    void LoadJsonData(string fileName)
    {
        if (FileManager.LoadFromFile(fileName, out var json))
        {
            SaveData sd = new SaveData();
            sd.LoadFromJson(json);
            LoadFromSaveData(sd);
            return;
        }
        CreateNewSaveFile();
    }

    public void LoadFromSaveData(SaveData saveData)
    {
        _originalSaveData = saveData; // stored for later

        // player prefs
        SetHideMenuEffects(PlayerPrefs.GetInt("HideMenuEffects") != 0);

        // global data
        Seed = saveData.Seed;

        SecondsLeftInDay = saveData.SecondsLeftInDay;
        Day = saveData.Day;
        Gold = saveData.Gold;
        Spice = saveData.Spice;

        PlayerTroops = new();
        foreach (CharacterData data in saveData.PlayerTroops)
        {
            Character playerCharacter = (Character)ScriptableObject.CreateInstance<Character>();
            playerCharacter.CreateFromData(data);
            PlayerTroops.Add(playerCharacter);
        }

        PlayerItemPouch = new();
        foreach (ItemData d in saveData.ItemPouch)
        {
            Item item = (Item)ScriptableObject.CreateInstance<Item>();
            item.LoadFromData(d);
            PlayerItemPouch.Add(item);
        }

        PlayerAbilityPouch = new();
        foreach (AbilityData abilityData in saveData.AbilityPouch)
        {
            Ability a = Instantiate(GameDatabase.GetAbilityById(abilityData.TemplateId));
            a.name = abilityData.Name;
            PlayerAbilityPouch.Add(a);
        }

        LoadReports(saveData);

        _buildingManager.LoadAllBuildingsFromData(saveData.CampBuildings);

        foreach (AbilityNodeGraphData data in saveData.AbilityNodeGraphs)
            GetAbilityNodeGraphById(data.Id).LoadFromData(data);
    }

    void LoadReports(SaveData saveData)
    {
        Reports = new();
        foreach (ReportData rd in saveData.Reports)
        {
            Report report = ScriptableObject.CreateInstance<Report>();
            report.CreateFromData(rd);
            Reports.Add(report);
        }

        ReportsArchived = new();
        foreach (ReportData rd in saveData.ReportsArchived)
        {
            Report report = ScriptableObject.CreateInstance<Report>();
            report.CreateFromData(rd);
            ReportsArchived.Add(report);
        }
    }

    public void ClearSaveData()
    {
        PlayerPrefs.DeleteAll();

        Seed = System.Environment.TickCount;

        SecondsLeftInDay = SecondsInDay;
        Day = 1;
        Gold = 0;
        Spice = 0;

        PlayerTroops = new();
        PlayerItemPouch = new();
        PlayerAbilityPouch = new();

        CutsceneIndexToPlay = 0; // TODO: wrong but it's ok for now.

        Reports = new();
        ReportsArchived = new();

        foreach (AbilityNodeGraph g in _abilityNodeGraphs)
            g.ResetNodes();

        if (FileManager.WriteToFile(PlayerPrefs.GetString("saveName"), ""))
            Debug.Log("Cleared active save");

        LoadLevel(Scenes.MainMenu);
        OnClearSaveData?.Invoke();
    }

}
