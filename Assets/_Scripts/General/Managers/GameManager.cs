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

    public Map Map;
    public Battle SelectedBattle; // HERE: battle testing { get; private set; }

    public event Action<int> OnDayPassed;
    public event Action<int> OnGoldChanged;
    public event Action<int> OnSpiceChanged;
    
    public event Action<string> OnLevelLoaded;
    public event Action OnNewSaveFileCreation;
    public event Action OnClearSaveData;
    public event Action<bool> OnTimerStateChanged;
    protected override void Awake()
    {
        Debug.Log($"Game manager Awake");
        base.Awake();
        _levelLoader = GetComponent<LevelLoader>();

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

        saveData.MapData = Map.SerializeSelf();
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
        PlayerHero.LoadFromData(saveData.PlayerHero);

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

        Map templateMap = GameDatabase.GetMapById("59e25ea9-893a-420b-b64b-d2cd176e66e7");
        Map = Instantiate(templateMap);
        Map.Reset();

        if (FileManager.WriteToFile(PlayerPrefs.GetString("saveName"), ""))
            Debug.Log("Cleared active save");

        LoadLevel(Scenes.MainMenu);
        OnClearSaveData?.Invoke();
    }

}
