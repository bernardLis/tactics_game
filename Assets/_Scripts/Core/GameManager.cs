using System;
using System.Collections.Generic;
using Lis.Core.Utilities;
using Lis.Units.Hero;
using Lis.Upgrades;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

namespace Lis.Core
{
    public class GameManager : PersistentSingleton<GameManager>, ISavable
    {
        LevelLoader _levelLoader;

        public GameDatabase GameDatabase;

        [FormerlySerializedAs("EntityDatabase")]
        public UnitDatabase UnitDatabase;

        public UpgradeBoard UpgradeBoard;
        public Stats GameStats;

        // global data
        int _seed;

        public int Gold { get; private set; }

        public Battle.Battle CurrentBattle { get; private set; }

        public VisualElement Root { get; private set; }
        public readonly List<FullScreenElement> OpenFullScreens = new();

        public event Action<int> OnGoldChanged;

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
            Helpers.SetUpHelpers(Root);

            // HERE: testing
            // global save per 'game'
            if (PlayerPrefs.GetString("saveName").Length == 0)
                CreateNewSaveFile();
            else
                LoadFromSaveFile();

            UpgradeBoard.Initialize();
            UnitDatabase.Initialize();

            CurrentBattle = Instantiate(GameDatabase.SampleBattle);
            CurrentBattle.Initialize(1); // necessary for testing

        }

        // using Unity.Services.Analytics;
        // using Unity.Services.Core;
        // async void Services()
        // {
        //     await UnityServices.InitializeAsync();
        //     // TODO: analytics - need opt in flow
        //     AnalyticsService.Instance.StartDataCollection();
        //
        //     HandleCustomEvent();
        // }
        //
        // void HandleCustomEvent()
        // {
        //     Dictionary<string, object> parameters = new Dictionary<string, object>()
        //     {
        //         { "fabulousString", "hello there" },
        //         { "sparklingInt", 1337 },
        //         { "spectacularFloat", 0.451f },
        //         { "peculiarBool", true },
        //     };
        //
        //     AnalyticsService.Instance.CustomData("gameStart", parameters);
        // }

        public void SetHero(Hero hero)
        {
            CurrentBattle.SelectedHero = hero;
        }

        public void Play()
        {
            StartGame();
        }

        public void StartGame()
        {
            CurrentBattle.Initialize(1);
            PlayerPrefs.SetInt(CurrentBattle.SelectedHero.Id, CurrentBattle.SelectedHero.TimesPicked + 1);
            LoadScene(Scenes.Battle);
        }

        /* RESOURCES */
        public void ChangeGoldValue(int o)
        {
            if (o == 0) return;

            Gold += o;
            OnGoldChanged?.Invoke(Gold);
        }

        /* LEVELS */
        public void LoadScene(string level)
        {
            OpenFullScreens.Clear();
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
            _seed = Environment.TickCount;

            Gold = 2137;

            UpgradeBoard.Reset();

            // new save
            string guid = Guid.NewGuid().ToString();
            string fileName = guid + ".dat";
            FileManager.CreateFile(fileName);
            PlayerPrefs.SetString("saveName", fileName);
            PlayerPrefs.Save();

            OnNewSaveFileCreation?.Invoke();
            SaveJsonData();
        }

        void LoadFromSaveFile()
        {
            LoadJsonData(PlayerPrefs.GetString("saveName"));
        }

        public void SaveJsonData()
        {
            SaveData sd = new SaveData();
            PopulateSaveData(sd);
            FileManager.WriteToFile(PlayerPrefs.GetString("saveName"), sd.ToJson());
            // if (FileManager.WriteToFile(PlayerPrefs.GetString("saveName"), sd.ToJson()))
            //     Debug.Log("Save successful");
        }

        public void PopulateSaveData(SaveData saveData)
        {
            // global data
            saveData.Seed = _seed;

            saveData.Gold = Gold;
            saveData.GlobalUpgradeBoard = UpgradeBoard.SerializeSelf();
            saveData.GameStats = GameStats.SerializeSelf();
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
            // global data
            _seed = saveData.Seed;

            Gold = saveData.Gold;
            UpgradeBoard.LoadFromData(saveData.GlobalUpgradeBoard);
            GameStats.LoadFromData(saveData.GameStats);
        }

        public void ClearSaveData()
        {
            _seed = Environment.TickCount;

            Gold = 2137;
            UpgradeBoard.Reset();
            GameStats.Reset();

            if (FileManager.WriteToFile(PlayerPrefs.GetString("saveName"), ""))
                Debug.Log("Cleared active save");

            LoadScene(Scenes.MainMenu);
            OnClearSaveData?.Invoke();
        }
    }
}