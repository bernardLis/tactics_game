using System;
using System.Collections.Generic;
using Lis.Core.Utilities;
using Lis.HeroCreation;
using Lis.Units.Hero;
using Lis.Upgrades;
using Unity.Services.Analytics;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.UIElements;

namespace Lis.Core
{
    public class GameManager : PersistentSingleton<GameManager>, ISavable
    {
        public GameDatabase GameDatabase;
        public UnitDatabase UnitDatabase;
        public UpgradeBoard UpgradeBoard;
        public Stats GameStats;
        public readonly List<FullScreenElement> OpenFullScreens = new();
        LevelLoader _levelLoader;

        // global data
        int _seed;

        public List<VisualHero> VisualHeroes = new();
        public VisualHero CurrentVisualHero;

        public int Gold { get; private set; }

        public Campaign Campaign { get; private set; }

        public VisualElement Root { get; private set; }

        protected override void Awake()
        {
            base.Awake();
            Root = GetComponent<UIDocument>().rootVisualElement;
            Debug.Log(Application.persistentDataPath);

            Debug.Log("Game manager Awake");
            RunServices();
        }

        void Start()
        {
            Debug.Log("Game manager Start");
            _levelLoader = GetComponent<LevelLoader>();
            Helpers.SetUpHelpers(Root);

            // HERE: testing
            // global save per 'game'
            if (PlayerPrefs.GetString("saveName").Length == 0)
                CreateNewSaveFile();
            else
                LoadFromSaveFile();

            UpgradeBoard.Initialize();

            Campaign = Instantiate(GameDatabase.SampleCampaign);
            Campaign.Initialize(CurrentVisualHero); // TODO: normally only when starting a new game
        }

        public void ResetCurrentHero()
        {
            CurrentVisualHero = ScriptableObject.CreateInstance<VisualHero>();
            CurrentVisualHero.Initialize();
            CurrentVisualHero.RandomizeOutfit();
        }

        public void AddCurrentVisualHero()
        {
            VisualHeroes.Add(CurrentVisualHero);
        }

        public void SelectHero(VisualHero hero)
        {
            CurrentVisualHero = hero;
        }

        public void RemoveVisualHero(VisualHero hero)
        {
            VisualHeroes.Remove(hero);
            SaveJsonData();
        }

        List<VisualHeroData> PopulateVisualHeroes()
        {
            List<VisualHeroData> visualHeroes = new();
            foreach (VisualHero h in VisualHeroes)
            {
                if (h == null) continue;
                visualHeroes.Add(h.SerializeSelf());
            }

            return visualHeroes;
        }

        List<VisualHero> DeserializeVisualHeroes(List<VisualHeroData> visualHeroData)
        {
            List<VisualHero> visualHeroes = new();
            foreach (VisualHeroData data in visualHeroData)
            {
                VisualHero hero = ScriptableObject.CreateInstance<VisualHero>();
                hero.LoadFromData(data);
                visualHeroes.Add(hero);
            }

            return visualHeroes;
        }

        public void PopulateSaveData(SaveData saveData)
        {
            // global data
            saveData.Seed = _seed;

            saveData.Gold = Gold;
            saveData.VisualHeroes = PopulateVisualHeroes();
            saveData.GlobalUpgradeBoard = UpgradeBoard.SerializeSelf();
            saveData.GameStats = GameStats.SerializeSelf();
        }

        public void LoadFromSaveData(SaveData saveData)
        {
            Debug.Log("Loading from save data");
            // global data
            _seed = saveData.Seed;

            Gold = saveData.Gold;
            VisualHeroes = DeserializeVisualHeroes(saveData.VisualHeroes);
            UpgradeBoard.LoadFromData(saveData.GlobalUpgradeBoard);
            GameStats.LoadFromData(saveData.GameStats);
        }

        public event Action<int> OnGoldChanged;

        public event Action OnNewSaveFileCreation;
        public event Action OnClearSaveData;

        async void RunServices()
        {
            await UnityServices.InitializeAsync();
            // TODO: analytics - need opt in flow
            // HERE: turn on data collection
            // if (PlayerPrefs.GetInt("isDataCollectionAllowed", 1) != 0)
            //     AnalyticsService.Instance.StartDataCollection();

            SendTestEvent();
        }

        void SendTestEvent()
        {
            var parameters = new Dictionary<string, object>
            {
                { "myWonderfulParameter", "hello there" }
            };

            AnalyticsService.Instance.CustomData("myGameStart", parameters);
        }

        public void StartGame()
        {
            Gold = 0;
            Campaign.Initialize(CurrentVisualHero);

            LoadScene(Scenes.Map);
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
        }

        /*************
         * Saving and Loading
         * https://www.youtube.com/watch?v=uD7y4T4PVk0
         */
        void CreateNewSaveFile()
        {
            Debug.Log("Creating new save file...");
            _seed = Environment.TickCount;

            Gold = 0;

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
            SaveData sd = new();
            PopulateSaveData(sd);
            FileManager.WriteToFile(PlayerPrefs.GetString("saveName"), sd.ToJson());
            // if (FileManager.WriteToFile(PlayerPrefs.GetString("saveName"), sd.ToJson()))
            //     Debug.Log("Save successful");
        }

        void LoadJsonData(string fileName)
        {
            if (FileManager.LoadFromFile(fileName, out string json))
            {
                SaveData sd = new();
                sd.LoadFromJson(json);
                LoadFromSaveData(sd);
                return;
            }

            CreateNewSaveFile();
        }

        public void ClearSaveData()
        {
            _seed = Environment.TickCount;

            Gold = 0;
            UpgradeBoard.Reset();
            GameStats.Reset();

            if (FileManager.WriteToFile(PlayerPrefs.GetString("saveName"), ""))
                Debug.Log("Cleared active save");

            LoadScene(Scenes.MainMenu);
            OnClearSaveData?.Invoke();
        }
    }
}