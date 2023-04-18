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

    public GameDatabase GameDatabase;
    public HeroDatabase HeroDatabase;
    public QuestDatabase QuestDatabase;

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

    public Hero PlayerHero;
    public Hero FriendHero;
    public List<Hero> Troops { get; private set; } = new();
    [HideInInspector] public List<Item> PlayerItemPouch = new();

    [HideInInspector] public List<Ability> PlayerAbilityPouch = new();


    [SerializeField] List<AbilityNodeGraph> _abilityNodeGraphs = new();

    public Map Map;
    public Battle SelectedBattle; // HERE: battle testing { get; private set; }

    public event Action<int> OnDayPassed;
    public event Action<int> OnGoldChanged;
    public event Action<int> OnSpiceChanged;
    public event Action<Hero> OnHeroAddedToTroops;
    public event Action<Hero> OnHeroRemovedFromTroops;
    public event Action<string> OnLevelLoaded;
    public event Action OnNewSaveFileCreation;
    public event Action OnClearSaveData;
    public event Action<bool> OnTimerStateChanged;
    protected override void Awake()
    {
        Debug.Log($"Game manager Awake");
        base.Awake();
        _levelLoader = GetComponent<LevelLoader>();
        HeroDatabase.Initialize();

        // HERE: battle testing
         LoadFromSaveFile();
        SelectedBattle.Hero = PlayerHero;
    }

    void Start()
    {
        Debug.Log($"Game manager Start");
        // global save per 'game'
        if (PlayerPrefs.GetString("saveName").Length == 0)
            CreateNewSaveFile();
        else
            LoadFromSaveFile();
    }

    public void Play()
    {
        if (PlayerHero == null)
        {
            LoadLevel(Scenes.HeroCreation);
            return;
        }
        StartGame();
    }

    public void StartGame()
    {
        LoadLevel(Scenes.Battle);
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

    /* RESOURCES */
    public void PassDay()
    {
        Day += 1;

        OnDayPassed?.Invoke(Day);
        SaveJsonData();
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
    public List<Hero> GetAllHeroes()
    {
        List<Hero> all = new(Troops);
        all.Add(PlayerHero);
        all.Add(FriendHero);
        return all;
    }

    public void AddHeroToTroops(Hero hero)
    {
        Troops.Add(hero);
        hero.SetDayAddedToTroops(Day);
        OnHeroAddedToTroops?.Invoke(hero);
        SaveJsonData();
    }

    public void RemoveHeroFromTroops(Hero hero)
    {
        Troops.Remove(hero);
        OnHeroRemovedFromTroops?.Invoke(hero);
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

        PlayerHero = null;
        FriendHero = null;

        foreach (AbilityNodeGraph g in _abilityNodeGraphs)
            g.ResetNodes();

        Map templateMap = GameDatabase.GetMapById("59e25ea9-893a-420b-b64b-d2cd176e66e7");
        Map = Instantiate(templateMap);
        Map.Reset();

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

        if (PlayerHero != null)
            saveData.PlayerHero = PlayerHero.SerializeSelf();
        if (FriendHero != null)
            saveData.FriendHero = FriendHero.SerializeSelf();

        saveData.PlayerTroops = PopulateTroops();

        saveData.ItemPouch = PopulateItemPouch();

        saveData.AbilityPouch = PopulateAbilityPouch();

        saveData.AbilityNodeGraphs = PopulateAbilityNodeGraphs();

        saveData.MapData = Map.SerializeSelf();
    }

    List<HeroData> PopulateTroops()
    {
        List<HeroData> charData = new();
        foreach (Hero c in Troops)
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

        PlayerHero = (Hero)ScriptableObject.CreateInstance<Hero>();
        PlayerHero.CreateFromData(saveData.PlayerHero);

        FriendHero = (Hero)ScriptableObject.CreateInstance<Hero>();
        FriendHero.CreateFromData(saveData.FriendHero);

        Troops = new();
        foreach (HeroData data in saveData.PlayerTroops)
        {
            Hero hero = (Hero)ScriptableObject.CreateInstance<Hero>();
            hero.CreateFromData(data);
            Troops.Add(hero);
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
            Ability a = Instantiate(HeroDatabase.GetAbilityById(abilityData.TemplateId));
            a.name = abilityData.Name;
            PlayerAbilityPouch.Add(a);
        }

        foreach (AbilityNodeGraphData data in saveData.AbilityNodeGraphs)
            GetAbilityNodeGraphById(data.Id).LoadFromData(data);

        Map = ScriptableObject.CreateInstance<Map>();
        Map.LoadFromData(saveData.MapData);
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

        PlayerHero = null;
        FriendHero = null;
        Troops = new();
        PlayerItemPouch = new();
        PlayerAbilityPouch = new();

        foreach (AbilityNodeGraph g in _abilityNodeGraphs)
            g.ResetNodes();

        Map templateMap = GameDatabase.GetMapById("59e25ea9-893a-420b-b64b-d2cd176e66e7");
        Map = Instantiate(templateMap);
        Map.Reset();

        if (FileManager.WriteToFile(PlayerPrefs.GetString("saveName"), ""))
            Debug.Log("Cleared active save");

        LoadLevel(Scenes.MainMenu);
        OnClearSaveData?.Invoke();
    }

}
