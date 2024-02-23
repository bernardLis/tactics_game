using System;
using System.Collections.Generic;
using Unity.Services.Analytics;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.UIElements;

namespace Lis
{
    public class GameManager : PersistentSingleton<GameManager>, ISavable
    {
        LevelLoader _levelLoader;

        public GameDatabase GameDatabase;
        public EntityDatabase EntityDatabase;
        public UpgradeBoard UpgradeBoard;
        public GameObject PoolManagerPrefab;

        SaveData _originalSaveData;

        // global data
        public int Seed { get; private set; }
        public int BattleNumber;

        public int TotalGoldCollected { get; private set; }
        public int Gold { get; private set; }

        public Battle CurrentBattle; // HERE: battle testing { get; private set; }

        public VisualElement Root { get; private set; }
        public List<FullScreenElement> OpenFullScreens = new();

        public Hero SelectedHero;

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

            // HERE: testing
            // global save per 'game'
            //  if (PlayerPrefs.GetString("saveName").Length == 0)
            //   {
            Helpers.SetUpHelpers(Root);
            CreateNewSaveFile();

            UpgradeBoard.Initialize();
            GameDatabase.InitializeBuildings();
            EntityDatabase.Initialize();
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
            StartGame();
        }

        public void StartGame()
        {
            BattleNumber++;
            PlayerPrefs.SetInt(SelectedHero.Id, SelectedHero.TimesPicked + 1);
            LoadScene(Scenes.Battle);
        }

        /* RESOURCES */
        public void ChangeGoldValue(int o)
        {
            if (o == 0) return;
            if (o > 0) TotalGoldCollected += o;

            Gold += o;
            OnGoldChanged?.Invoke(Gold);
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
            Seed = Environment.TickCount;

            BattleNumber = 0;
            Gold = 10000;

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

        public void LoadFromSaveFile()
        {
            LoadJsonData(PlayerPrefs.GetString("saveName"));
        }

        public void SaveJsonData()
        {
            SaveData sd = new SaveData();
            PopulateSaveData(sd);
            FileManager.WriteToFile(PlayerPrefs.GetString("saveName"), sd.ToJson());
            //  if (FileManager.WriteToFile(PlayerPrefs.GetString("saveName"), sd.ToJson()))
            //    Debug.Log("Save successful");
        }

        public void PopulateSaveData(SaveData saveData)
        {
            // global data
            saveData.Seed = Seed;
            saveData.BattleNumber = BattleNumber;

            saveData.Gold = Gold;
            saveData.GlobalUpgradeBoard = UpgradeBoard.SerializeSelf();
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

            // global data
            Seed = saveData.Seed;
            BattleNumber = saveData.BattleNumber;

            Gold = saveData.Gold;
            UpgradeBoard.LoadFromData(saveData.GlobalUpgradeBoard);
        }

        public void ClearSaveData()
        {
            Seed = Environment.TickCount;
            BattleNumber = 0;

            Gold = 1000;
            UpgradeBoard.Reset();

            if (FileManager.WriteToFile(PlayerPrefs.GetString("saveName"), ""))
                Debug.Log("Cleared active save");

            LoadScene(Scenes.MainMenu);
            OnClearSaveData?.Invoke();
        }
    }
}