using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;
using Object = UnityEngine.Object;

public class GameManager : PersistentSingleton<GameManager>, ISavable
{
    LevelLoader _levelLoader;

    // TODO: better set-up, something needs to track where you are in the game and save/load the info
    public Cutscene[] Cutscenes;
    int _currentCutSceneIndex = 0;

    public int Obols { get; private set; }


    [Header("Unity Setup")]
    public List<GlobalUpgrade> AllGlobalUpgrades;
    List<GlobalUpgrade> _purchasedGlobalUpgrades;

    public CharacterDatabase CharacterDatabase;
    [SerializeField] JourneyEvent[] AllEvents;

    List<JourneyEvent> _availableEvents;

    public bool WasJourneySetUp { get; private set; }
    public int JourneySeed { get; private set; } = 0; // TODO: this is a bad idea, probably
    public JourneyNodeData CurrentJourneyNode { get; private set; }
    [HideInInspector] public List<JourneyPath> JourneyPaths = new();
    [HideInInspector] public List<JourneyNodeData> VisitedJourneyNodes = new();

    [HideInInspector] public List<Character> PlayerTroops = new();
    public List<Item> PlayerItemPouch = new(); // HERE: normally [HideInInspector] and empty
    public List<Ability> PlayerAbilityPouch = new(); // HERE: normally [HideInInspector] and empty

    public int Gold { get; private set; }
    public JourneyNode CurrentNode; //TODO: //{ get; private set; }
    public JourneyNodeReward Reward { get; private set; }

    public string PlayerName { get; private set; }
    string _activeSave;
    string _currentLevel;

    public event Action<int> OnGoldChanged;
    public event Action<int> OnObolsChanged;

    protected override void Awake()
    {
        base.Awake();
        _levelLoader = GetComponent<LevelLoader>();

        // global save per 'game'
        if (PlayerPrefs.GetString("saveName").Length == 0)
            CreateNewSaveFile();
        else
            LoadFromSaveFile();

        // copy array to list;
        _availableEvents = new(AllEvents); // TODO: this should be membered per run

        // TODO: eee... I need a place to set player troops before the battle/journey start and then this should be gone
        // keeping it for now but I can create player troops when starting new run, taking into consideration all bonuses
        if (PlayerTroops.Count == 0)
            CreatePlayerTroops();

        // HERE: test
        Gold = 10;
    }

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

    public void LoadFromSaveFile()
    {
        Debug.Log("loading from save file");
        LoadJsonData(PlayerPrefs.GetString("saveName"));
    }

    public void PurchaseGlobalUpgrade(GlobalUpgrade upgrade)
    {
        _purchasedGlobalUpgrades.Add(upgrade);
        SaveJsonData();
    }

    public void StartNewRun(string playerName)
    {
        PlayerName = playerName;

        _currentLevel = Scenes.Cutscene;
        //StartGameFromSave(true);
    }
    /*
        public void StartGameFromSave(bool isNewGame = false)
        {
            if (!isNewGame)
                LoadJsonData(fileName);

            if (JourneySeed == 0)
                JourneySeed = System.DateTime.Now.Millisecond;

            _activeSave = fileName;

            // TODO: here you need to know what level to load...
            LoadLevel(_currentLevel);
        }
        */

    public Cutscene GetCurrentCutScene()
    {
        Cutscene c = Cutscenes[_currentCutSceneIndex];
        return c;
    }

    public void CutscenePlayed() // TODO: this for sure is not correct
    {
        _currentCutSceneIndex++;
    }

    public void LoadLevel(string level)
    {
        if (level == Scenes.Journey) // TODO: I want to save only on coming back to Journey, does it make sense?
            SaveJsonData();

        _levelLoader.LoadLevel(level);
    }

    public void LoadLevelFromNode(JourneyNode node)
    {
        CurrentNode = node;
        _levelLoader.LoadLevel(node.SceneToLoad);
    }

    void CreatePlayerTroops()
    {
        string path = "Characters";
        Object[] playerCharacters = Resources.LoadAll(path, typeof(Character));
        PlayerTroops = new();
        foreach (Character character in playerCharacters)
        {
            Character instance = Instantiate(character);
            foreach (GlobalUpgrade item in _purchasedGlobalUpgrades)
                item.Initialize(instance); // adding global upgrades to characters
            PlayerTroops.Add(instance);
        }
    }

    public void SetPlayerTroops(List<Character> troops)
    {
        PlayerTroops = new(troops);
    }

    public void JourneyWasSetUp(bool was) // TODO: better naming
    {
        WasJourneySetUp = was;
    }

    public void SetJourneySeed(int s)
    {
        JourneySeed = s;
    }

    public void SetCurrentJourneyNode(JourneyNodeData n)
    {
        VisitedJourneyNodes.Add(n);
        CurrentJourneyNode = n;
    }

    public void ChangeObolValue(int o)
    {
        Obols += o;
        OnObolsChanged?.Invoke(Obols);
        SaveJsonData();
    }

    public void ChangeGoldValue(int o)
    {
        Gold += o;
        OnGoldChanged?.Invoke(Gold);
        SaveJsonData();
    }

    public void AddItemToPouch(Item item)
    {
        PlayerItemPouch.Add(item);
        SaveJsonData();
    }

    public void SetNodeReward(JourneyNodeReward r)
    {
        Reward = r;
    }

    public JourneyEvent ChooseEvent()
    {
        JourneyEvent ev = _availableEvents[Random.Range(0, _availableEvents.Count)];
        _availableEvents.Remove(ev);
        return ev;
    }

    /*************
    * Saving and Loading
    * https://www.youtube.com/watch?v=uD7y4T4PVk0
    */

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
        saveData.PurchasedGlobalUpgrades = PopulatePurchasedGlobalUpgrades();


        // run data
        saveData.LastLevel = SceneManager.GetActiveScene().name;
        saveData.CutSceneIndex = _currentCutSceneIndex;
        saveData.Gold = Gold;
        saveData.JourneySeed = JourneySeed;
        saveData.CurrentJourneyNode = CurrentJourneyNode;
        saveData.VisitedJourneyNodes = VisitedJourneyNodes;
        saveData.Characters = PopulateCharacters();
        saveData.ItemPouch = PopulateItemPouch();
        saveData.AbilityPouch = PopulateAbilityPouch();
    }

    List<string> PopulatePurchasedGlobalUpgrades()
    {
        List<string> ids = new();
        foreach (GlobalUpgrade upgrade in _purchasedGlobalUpgrades)
            ids.Add(upgrade.Id);

        return ids;
    }

    List<CharacterData> PopulateCharacters()
    {
        List<CharacterData> charData = new();
        foreach (Character c in PlayerTroops)
        {
            CharacterData data = new();
            data.ReferenceID = c.ReferenceID;
            data.CharacterName = c.CharacterName;
            data.Portrait = c.Portrait.name;
            data.Level = c.Level;
            data.Experience = c.Experience;
            data.Power = c.Power;
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
        foreach (Item i in PlayerItemPouch)
            itemReferenceIds.Add(i.ReferenceID);

        return itemReferenceIds;
    }

    List<string> PopulateAbilityPouch()
    {
        List<string> abilityReferenceIds = new();
        foreach (Ability a in PlayerAbilityPouch)
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
            _purchasedGlobalUpgrades.Add(AllGlobalUpgrades.First(x => x.Id == savedId));

        // run data
        _currentLevel = saveData.LastLevel;
        _currentCutSceneIndex = saveData.CutSceneIndex;
        Gold = saveData.Gold;
        JourneySeed = saveData.JourneySeed;
        CurrentJourneyNode = saveData.CurrentJourneyNode;
        VisitedJourneyNodes = saveData.VisitedJourneyNodes;
        PlayerTroops = new();
        foreach (CharacterData data in saveData.Characters)
        {
            Character playerCharacter = (Character)ScriptableObject.CreateInstance<Character>();
            playerCharacter.Create(data);
            PlayerTroops.Add(playerCharacter);
        }

        PlayerItemPouch = new();
        foreach (string itemReferenceId in saveData.ItemPouch)
            PlayerItemPouch.Add(CharacterDatabase.GetItemByReference(itemReferenceId));

        PlayerAbilityPouch = new();
        foreach (string abilityReferenceId in saveData.AbilityPouch)
            PlayerAbilityPouch.Add(CharacterDatabase.GetAbilityByReferenceID(abilityReferenceId));
    }

    // TODO:
    public void ClearSaveData()
    {
        if (FileManager.WriteToFile(_activeSave, ""))
            Debug.Log("Cleared active save");
    }

}
