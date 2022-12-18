using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class GameManager : PersistentSingleton<GameManager>, ISavable
{
    SaveData _originalSaveData;

    LevelLoader _levelLoader;

    public GameDatabase GameDatabase;

    // global data
    public bool WasTutorialPlayed { get; private set; }
    public int Seed { get; private set; }

    public int Day { get; private set; }
    public int Gold { get; private set; }

    public int Spice { get; private set; }

    public List<Item> ShopItems = new();
    public int ShopRerollPrice { get; private set; }

    public int TroopsLimit { get; private set; }
    public List<Character> PlayerTroops = new();
    [HideInInspector] public List<Item> PlayerItemPouch = new();
    [HideInInspector] public List<Ability> PlayerAbilityPouch = new();

    public List<Report> Reports = new();
    public List<Report> ReportsArchived = new();

    [SerializeField]
    List<CampBuilding> _campBuildings = new();

    [SerializeField]
    List<AbilityNodeGraph> _abilityNodeGraphs = new();

    public int CutsceneIndexToPlay = 0; // TODO: this is wrong, but for now it is ok

    public Quest ActiveQuest;

    public event Action<Report> OnReportAdded;
    public event Action<int> OnDayPassed;
    public event Action<int> OnGoldChanged;
    public event Action<int> OnSpiceChanged;
    public event Action<int> OnTroopsLimitChanged;
    public event Action<Character> OnCharacterAddedToTroops;
    public event Action<int> OnShopRerollPriceChanged;
    public event Action<string> OnLevelLoaded;

    protected override void Awake()
    {
        base.Awake();
        _levelLoader = GetComponent<LevelLoader>();

        // global save per 'game'
        if (PlayerPrefs.GetString("saveName").Length == 0)
            CreateNewSaveFile();
        else
            LoadFromSaveFile();
    }


    public void Play()
    {
        if (PlayerPrefs.GetString("saveName").Length == 0) // TODO: this does not work...
            LoadLevel(Scenes.Cutscene);
        else
            LoadLevel(Scenes.Dashboard);
    }

    public void BattleWon()
    {
        ActiveQuest.UpdateQuestState(QuestState.Finished);
        ActiveQuest.IsWon = true;
        ActiveQuest = null;
        PassDay();
    }

    public void BattleLost()
    {
        ActiveQuest.UpdateQuestState(QuestState.Finished);
        ActiveQuest = null;
        PassDay();
    }

    public void AddNewReport(Report r)
    {
        Reports.Add(r);
        OnReportAdded?.Invoke(r);
    }


    /* RESOURCES */
    public void PassDay()
    {
        Day += 1;
        ChangeSpiceValue(500); // HERE: vid

        if (Day % 7 == 0) // shop resets every 7th day
        {
            ResetShop();
            PayMaintenance();
        }
        if (Random.value > 0.5f)
            AddRandomQuest();
        if (Random.value > 0.5f)
            AddRecruit();
        AddShop();

        OnDayPassed?.Invoke(Day);
        SaveJsonData();
    }

    void PayMaintenance()
    {
        ChangeGoldValue(-GetCurrentMaintenanceCost());

        int cost = GetCurrentMaintenanceCost();

        Report r = ScriptableObject.CreateInstance<Report>();
        r.Initialize(ReportType.Text, null, null, $"Maintenance is paid: {cost}");
        AddNewReport(r);
    }

    public int GetCurrentMaintenanceCost() { return PlayerTroops.Count * 200 * 7; }

    void AddRandomQuest()
    {
        Quest q = ScriptableObject.CreateInstance<Quest>();
        q.CreateRandom();
        OnDayPassed += q.OnDayPassed;

        Report r = ScriptableObject.CreateInstance<Report>();
        r.Initialize(ReportType.Quest, q);
        AddNewReport(r);
    }

    void AddRecruit()
    {
        Recruit newRecruit = ScriptableObject.CreateInstance<Recruit>();
        newRecruit.CreateRandom();

        Report r = ScriptableObject.CreateInstance<Report>();
        r.Initialize(ReportType.Recruit, null, newRecruit);
        AddNewReport(r);
    }

    void AddShop()
    {
        Shop newShop = ScriptableObject.CreateInstance<Shop>();
        newShop.CreateShop();

        Report r = ScriptableObject.CreateInstance<Report>();
        r.Initialize(ReportType.Shop, null, null, null, null, newShop);
        AddNewReport(r);
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

    public void ChangeTroopsLimit(int change)
    {
        TroopsLimit += change;
        OnTroopsLimitChanged?.Invoke(change);
    }

    public List<CampBuilding> GetCampBuildings() { return _campBuildings; }
    public CampBuilding GetCampBuildingById(string id) { return _campBuildings.FirstOrDefault(x => x.Id == id); }

    public List<AbilityNodeGraph> GetAbilityNodeGraphs() { return _abilityNodeGraphs; }
    public AbilityNodeGraph GetAbilityNodeGraphById(string id) { return _abilityNodeGraphs.FirstOrDefault(x => x.Id == id); }

    /* Shop */
    public void RemoveItemFromShop(Item item)
    {
        ShopItems.Remove(item);
        SaveJsonData();
    }

    void ResetShop()
    {
        ChooseShopItems();
        ChangeShopRerollPrice(200);

        Report r = ScriptableObject.CreateInstance<Report>();
        r.Initialize(ReportType.Text, null, null, "New inventory in the shop! Visit us!");
        AddNewReport(r);
    }

    public void ChooseShopItems()
    {
        ShopItems = new();
        for (int i = 0; i < 6; i++)
        {
            Item item = GameDatabase.GetRandomItem();
            ShopItems.Add(item);
        }
    }

    public void ChangeShopRerollPrice(int newValue)
    {
        ShopRerollPrice = newValue;
        OnShopRerollPriceChanged?.Invoke(newValue);
    }

    /* Troops & pouches */
    public void AddCharacterToTroops(Character character)
    {
        PlayerTroops.Add(character);
        OnDayPassed += character.OnDayPassed;
        OnCharacterAddedToTroops?.Invoke(character);
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

    public void AddAbilityToPouch(Ability ability) { PlayerAbilityPouch.Add(ability); }

    public void RemoveAbilityFromPouch(Ability ability) { PlayerAbilityPouch.Remove(ability); }

    /* LEVELS */
    public void SetWasTutorialPlayed(bool was)
    {
        WasTutorialPlayed = was;
        SaveJsonData();
    }

    public void StartBattle(Quest quest)
    {
        ActiveQuest = quest;
        LoadLevel(ActiveQuest.SceneToLoad);
    }

    public void LoadLevel(string level)
    {
        _levelLoader.LoadLevel(level);
        OnLevelLoaded?.Invoke(level);
    }

    /*************
    * Saving and Loading
    * https://www.youtube.com/watch?v=uD7y4T4PVk0
    */

    List<Character> CreatePlayerTroops() // for the new save
    {
        List<Character> instantiatedTroops = new();

        List<Character> playerCharacters = new(GameDatabase.GetAllStarterTroops());
        PlayerTroops = new();
        foreach (Character character in playerCharacters)
        {
            Character instance = Instantiate(character);
            instance.InitializeStarterTroops();
            OnDayPassed += instance.OnDayPassed;
            instantiatedTroops.Add(instance);
        }

        return instantiatedTroops;
    }

    void CreateNewSaveFile()
    {
        Seed = System.Environment.TickCount;

        Day = 1;
        Gold = 0;
        Spice = 500;

        ChooseShopItems();
        ShopRerollPrice = 200;

        TroopsLimit = 5;
        PlayerTroops = CreatePlayerTroops();

        // TODO: // HERE: for now, I could hand craft 3 first quests or something...
        for (int i = 0; i < 3; i++)
            AddRandomQuest();

        foreach (CampBuilding b in _campBuildings)
        {
            b.ResetSelf();
            b.Initialize();
        }

        foreach (AbilityNodeGraph g in _abilityNodeGraphs)
            g.ResetNodes();

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

    // TODO: prime suspect for a rewrite
    public void PopulateSaveData(SaveData saveData)
    {
        // global data
        saveData.WasTutorialPlayed = WasTutorialPlayed;
        saveData.Seed = Seed;

        saveData.Day = Day;
        saveData.Gold = Gold;

        saveData.Spice = Spice;

        saveData.ShopItems = PopulateShopItems();
        saveData.ShopRerollPrice = ShopRerollPrice;

        saveData.TroopsLimit = TroopsLimit;
        saveData.PlayerTroops = PopulateCharacters();
        saveData.ItemPouch = PopulateItemPouch();
        saveData.AbilityPouch = PopulateAbilityPouch();

        saveData.Reports = PopulateReports();
        saveData.ReportsArchived = PopulateArchivedReports();

        saveData.CampBuildings = PopulateCampBuildings();
        saveData.AbilityNodeGraphs = PopulateAbilityNodeGraphs();
    }

    List<string> PopulateShopItems()
    {
        List<string> itemIds = new();
        foreach (Item i in ShopItems)
            itemIds.Add(i.Id);

        return itemIds;
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
        foreach (CampBuilding b in _campBuildings)
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

        // global data
        WasTutorialPlayed = saveData.WasTutorialPlayed;
        Seed = saveData.Seed;

        Day = saveData.Day;
        Gold = saveData.Gold;

        Spice = saveData.Spice;

        ShopRerollPrice = saveData.ShopRerollPrice;
        ShopItems = new();
        foreach (string itemReferenceId in saveData.ShopItems)
            ShopItems.Add(GameDatabase.GetItemById(itemReferenceId));

        TroopsLimit = saveData.TroopsLimit;
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

        foreach (CampBuildingData data in saveData.CampBuildings)
            GetCampBuildingById(data.Id).LoadFromData(data);

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

        WasTutorialPlayed = false;
        Seed = System.Environment.TickCount;

        Day = 1;
        Gold = 0;

        Spice = 0;

        ChooseShopItems();
        ShopRerollPrice = 200;

        TroopsLimit = 5;
        PlayerTroops = CreatePlayerTroops();
        PlayerItemPouch = new();
        PlayerAbilityPouch = new();

        CutsceneIndexToPlay = 0; // TODO: wrong but it's ok for now.

        Reports = new();
        ReportsArchived = new();

        foreach (CampBuilding b in _campBuildings)
            b.ResetSelf();
        foreach (AbilityNodeGraph g in _abilityNodeGraphs)
            g.ResetNodes();

        if (FileManager.WriteToFile(PlayerPrefs.GetString("saveName"), ""))
            Debug.Log("Cleared active save");

        LoadLevel(Scenes.MainMenu);
    }

}
