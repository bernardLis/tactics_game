using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class GameManager : PersistentSingleton<GameManager>, ISavable
{
    public const float SecondsInDay = 10;

    LevelLoader _levelLoader;
    BuildingManager _buildingManager;

    public GameDatabase GameDatabase;

    SaveData _originalSaveData;

    // settings
    public bool HideMenuEffects { get; private set; }
    public void SetHideMenuEffects(bool hide) { HideMenuEffects = hide; }

    // global data
    public bool WasIntroCutscenePlayed;
    public int Seed { get; private set; }

    public float TotalSeconds { get; private set; }
    public int Day { get; private set; }
    public float SecondsLeftInDay { get; private set; }
    public int Gold { get; private set; }
    public int Spice { get; private set; }

    public bool IsTimerOn { get; private set; }

    public Character PlayerCharacter;
    public Character FriendCharacter;
    public List<Character> Troops { get; private set; } = new();
    [HideInInspector] public List<Item> PlayerItemPouch = new();

    [HideInInspector] public List<Ability> PlayerAbilityPouch = new();

    public List<Report> Reports = new();
    public List<Report> ReportsArchived = new();

    [SerializeField] List<AbilityNodeGraph> _abilityNodeGraphs = new();

    public Battle SelectedBattle { get; private set; }

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
        if (PlayerCharacter == null)
        {
            LoadLevel(Scenes.CharacterCreation);
            return;
        }
        StartGame();
    }

    public void StartGame()
    {
        LoadLevel(Scenes.Map);
        IsTimerOn = true;
    }

    public DateTime GetCurrentDateTime()
    {
        DateTime d = ScriptableObject.CreateInstance<DateTime>();
        d.Day = Day;
        d.Seconds = SecondsInDay - SecondsLeftInDay;
        return d;
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
        r.Initialize(ReportType.Wages, null, null, null, null, null, null, null, Troops);
        AddNewReport(r);
    }

    public int GetCurrentWages()
    {
        int total = 0;
        foreach (Character c in Troops)
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
    public List<Character> GetAllCharacters()
    {
        List<Character> all = new(Troops);
        all.Add(PlayerCharacter);
        all.Add(FriendCharacter);
        return all;
    }

    public void AddCharacterToTroops(Character character)
    {
        Troops.Add(character);
        character.SetDayAddedToTroops(Day);
        OnCharacterAddedToTroops?.Invoke(character);
        SaveJsonData();
    }

    public void RemoveCharacterFromTroops(Character character)
    {
        Troops.Remove(character);
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

    public void LoadBattle(Battle b)
    {
        Debug.Log($"Loading battle {b}");
        SelectedBattle = b;
        SaveJsonData();
        LoadLevel("Battle");
    }

    public void LoadMap()
    {
        Debug.Log($"loading map after battle, won? {SelectedBattle.Won}");
        LoadLevel("Map");
        SaveJsonData();
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

        PlayerCharacter = null;
        FriendCharacter = null;

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
        FileManager.WriteToFile(PlayerPrefs.GetString("saveName"), sd.ToJson());
        //  if (FileManager.WriteToFile(PlayerPrefs.GetString("saveName"), sd.ToJson()))
        //    Debug.Log("Save successful");
    }

    // TODO: prime suspect for a rewrite
    public void PopulateSaveData(SaveData saveData)
    {
        // global data
        saveData.WasIntroCutscenePlayed = WasIntroCutscenePlayed;
        saveData.Seed = Seed;

        if (DashboardManager.Instance != null)
            saveData.SecondsLeftInDay = DashboardManager.Instance.DayTimer.GetTimeLeft();

        saveData.Day = Day;
        saveData.Gold = Gold;
        saveData.Spice = Spice;

        if (PlayerCharacter != null)
            saveData.PlayerCharacter = PlayerCharacter.SerializeSelf();
        if (FriendCharacter != null)
            saveData.FriendCharacter = FriendCharacter.SerializeSelf();

        saveData.PlayerTroops = PopulateTroops();

        saveData.ItemPouch = PopulateItemPouch();

        saveData.AbilityPouch = PopulateAbilityPouch();

        saveData.Reports = PopulateReports();
        saveData.ReportsArchived = PopulateArchivedReports();

        saveData.CampBuildings = PopulateCampBuildings();
        saveData.AbilityNodeGraphs = PopulateAbilityNodeGraphs();
    }

    List<CharacterData> PopulateTroops()
    {
        List<CharacterData> charData = new();
        foreach (Character c in Troops)
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
        WasIntroCutscenePlayed = saveData.WasIntroCutscenePlayed;
        Seed = saveData.Seed;

        SecondsLeftInDay = saveData.SecondsLeftInDay;
        Day = saveData.Day;
        Gold = saveData.Gold;
        Spice = saveData.Spice;

        PlayerCharacter = (Character)ScriptableObject.CreateInstance<Character>();
        PlayerCharacter.CreateFromData(saveData.PlayerCharacter);

        FriendCharacter = (Character)ScriptableObject.CreateInstance<Character>();
        FriendCharacter.CreateFromData(saveData.FriendCharacter);

        Troops = new();
        foreach (CharacterData data in saveData.PlayerTroops)
        {
            Character character = (Character)ScriptableObject.CreateInstance<Character>();
            character.CreateFromData(data);
            Troops.Add(character);
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
            report.LoadFromData(rd);
            Reports.Add(report);
        }

        ReportsArchived = new();
        foreach (ReportData rd in saveData.ReportsArchived)
        {
            Report report = ScriptableObject.CreateInstance<Report>();
            report.LoadFromData(rd);
            ReportsArchived.Add(report);
        }
    }

    public void ClearSaveData()
    {
        PlayerPrefs.DeleteAll();

        WasIntroCutscenePlayed = false;

        Seed = System.Environment.TickCount;

        SecondsLeftInDay = SecondsInDay;
        Day = 1;
        Gold = 0;
        Spice = 0;

        PlayerCharacter = null;
        FriendCharacter = null;
        Troops = new();
        PlayerItemPouch = new();
        PlayerAbilityPouch = new();

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
