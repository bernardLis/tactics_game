using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;
using UnityEngine.UIElements;
using System.Collections;

using Unity.Services.Analytics;
using Unity.Services.Core;


public class GameManager : PersistentSingleton<GameManager>, ISavable
{
    LevelLoader _levelLoader;

    public GameDatabase GameDatabase;
    public EntityDatabase EntityDatabase;

    SaveData _originalSaveData;

    // settings
    public bool HideMenuEffects { get; private set; }
    public void SetHideMenuEffects(bool hide) { HideMenuEffects = hide; }

    // global data
    public bool WasIntroCutscenePlayed;
    public int Seed { get; private set; }

    public int BattleNumber;
    public int GoldAdvantage;

    public int TotalGoldCollected { get; private set; }
    public int Gold { get; private set; }
    public int Spice { get; private set; }

    public static Color PlayerTeamColor = Color.blue;
    public static Color OpponentTeamColor = Color.red;

    public Hero PlayerHero;

    public Battle CurrentBattle; // HERE: battle testing { get; private set; }

    public VisualElement Root { get; private set; }
    public List<FullScreenElement> OpenFullScreens = new();

    public event Action<int> OnGoldChanged;
    public event Action<int> OnSpiceChanged;

    public event Action<string> OnLevelLoaded;
    public event Action OnNewSaveFileCreation;
    public event Action OnClearSaveData;
    protected override void Awake()
    {
        base.Awake();
        Debug.Log($"Game manager Awake");

        // Services();
    }

    void Start()
    {

        Debug.Log($"Game manager Start");

        _levelLoader = GetComponent<LevelLoader>();
        Root = GetComponent<UIDocument>().rootVisualElement;

        // HERE: testing
        // global save per 'game'
        //  if (PlayerPrefs.GetString("saveName").Length == 0)
        //   {
        Helpers.SetUpHelpers(Root);
        CreateNewSaveFile();

        //  }
        //   else
        //     LoadFromSaveFile();
    }

    async void Services()
    {
        await UnityServices.InitializeAsync();
        // TODO: analytics - need opt in flow
        AnalyticsService.Instance.StartDataCollection();

        HandleCustomEvent();
    }

    void HandleCustomEvent()
    {
        Dictionary<string, object> parameters = new Dictionary<string, object>()
        {
            { "fabulousString", "hello there" },
            { "sparklingInt", 1337 },
            { "spectacularFloat", 0.451f },
            { "peculiarBool", true },
        };

        AnalyticsService.Instance.CustomData("gameStart", parameters);
    }


    public void Play()
    {
        if (PlayerHero == null)
        {
            LoadScene(Scenes.HeroCreation);
            return;
        }
        StartGame();
    }

    public void StartGame()
    {
        LoadScene(Scenes.Battle);
    }

    public void ChangePlayerTeamColor(Color col)
    {
        PlayerTeamColor = col;
    }

    public void ChangeOpponentTeamColor(Color col)
    {
        OpponentTeamColor = col;
    }


    /* RESOURCES */
    public void ChangeGoldValue(int o)
    {
        if (o == 0)
            return;

        if (o > 0) TotalGoldCollected += o;

        Gold += o;
        OnGoldChanged?.Invoke(Gold);
    }

    public void ChangeSpiceValue(int o)
    {
        if (o == 0)
            return;

        Spice += o;
        OnSpiceChanged?.Invoke(Spice);
    }


    /* LEVELS */
    public void LoadScene(string level)
    {
        Time.timeScale = 1f;
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

        BattleNumber = 0;
        Gold = 10000 + GoldAdvantage * 1000;
        Spice = 500;

        PlayerHero = null;

        // new save
        string guid = Guid.NewGuid().ToString();
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
        saveData.BattleNumber = BattleNumber;

        saveData.Gold = Gold;
        saveData.Spice = Spice;

        if (PlayerHero != null)
            saveData.PlayerHero = PlayerHero.SerializeSelf();
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
        Debug.Log($"Loading from save data");
        _originalSaveData = saveData; // stored for later

        // player prefs
        SetHideMenuEffects(PlayerPrefs.GetInt("HideMenuEffects") != 0);

        // global data
        WasIntroCutscenePlayed = saveData.WasIntroCutscenePlayed;
        Seed = saveData.Seed;
        BattleNumber = saveData.BattleNumber;

        Gold = saveData.Gold;
        Spice = saveData.Spice;

        PlayerHero = ScriptableObject.CreateInstance<Hero>();
        PlayerHero.LoadFromData(saveData.PlayerHero);
    }

    public void ClearSaveData()
    {
        //PlayerPrefs.DeleteAll();

        WasIntroCutscenePlayed = false;

        Seed = System.Environment.TickCount;
        BattleNumber = 0;

        Gold = GoldAdvantage * 1000;
        Spice = 500;

        PlayerHero = null;

        if (FileManager.WriteToFile(PlayerPrefs.GetString("saveName"), ""))
            Debug.Log("Cleared active save");

        LoadScene(Scenes.MainMenu);
        OnClearSaveData?.Invoke();
    }

}
