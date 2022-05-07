using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : PersistentSingleton<GameManager>, ISavable
{
    LevelLoader _levelLoader;

    [Header("Unity Setup")]
    public CharacterDatabase CharacterDatabase;
    [SerializeField] JourneyEvent[] AllEvents;

    List<JourneyEvent> _availableEvents;

    public bool WasJourneySetUp { get; private set; }
    public int JourneySeed { get; private set; } = 0; // TODO: this is a bad idea, probably
    public JourneyNodeData CurrentJourneyNode { get; private set; }
    [HideInInspector] public List<JourneyPath> JourneyPaths = new();
    [HideInInspector] public List<JourneyNodeData> VisitedJourneyNodes = new();

    [HideInInspector] public List<Character> PlayerTroops = new();

    public int Obols { get; private set; }
    public JourneyNode CurrentNode { get; private set; }
    public JourneyNodeReward Reward { get; private set; }

    public string PlayerName { get; private set; }
    string _activeSave;

    protected override void Awake()
    {
        base.Awake();
        _levelLoader = GetComponent<LevelLoader>();

        // copy array to list;
        _availableEvents = new(AllEvents); // TODO: this should be membered per save

        // TODO: eee... I need a place to set player troops before the battle/journey start and then this should be gone
        if (PlayerTroops.Count == 0)
            CreatePlayerTroops();
    }

    public void StartNewGame(string activeSave, string playerName)
    {
        PlayerName = playerName;

        StartGameFromSave(activeSave, true);
    }

    public void StartGameFromSave(string fileName, bool isNewGame = false)
    {
        if (!isNewGame)
            LoadJsonData(fileName);

        if (JourneySeed == 0)
            JourneySeed = System.DateTime.Now.Millisecond;

        _activeSave = fileName;

        LoadLevel("Journey");
    }

    public void LoadLevel(string level)
    {
        if (level == "Journey") // TODO: I want to save only on coming back to Journey, does it make sense?
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
        foreach (Character character in playerCharacters)// TODO: should I clone them?
            PlayerTroops.Add(character);
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

    public void SetObols(int o)
    {
        Obols = o;
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

    void SaveJsonData()
    {
        SaveData sd = new SaveData();
        PopulateSaveData(sd);
        if (FileManager.WriteToFile(_activeSave, sd.ToJson()))
            Debug.Log("Save successful");
    }

    public void PopulateSaveData(SaveData saveData)
    {
        saveData.PlayerName = PlayerName;
        saveData.Obols = Obols;
        saveData.JourneySeed = JourneySeed;
        saveData.CurrentJourneyNode = CurrentJourneyNode;
        saveData.VisitedJourneyNodes = VisitedJourneyNodes;
        saveData.Characters = PopulateCharacters();
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
            data.Strength = c.Strength;
            data.Intelligence = c.Intelligence;
            data.Agility = c.Agility;
            data.Stamina = c.Stamina;
            data.Body = c.Body.name;
            data.Weapon = c.Weapon.name;

            List<string> abilityReferenceIds = new();
            foreach (Ability a in c.Abilities)
            {
                abilityReferenceIds.Add(a.ReferenceID);
            }
            data.AbilityReferenceIds = new(abilityReferenceIds);

            charData.Add(data);
        }

        return charData;
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
        PlayerName = saveData.PlayerName;
        Obols = saveData.Obols;
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
    }

    // TODO:
    public void ClearSaveData()
    {
        if (FileManager.WriteToFile(_activeSave, ""))
            Debug.Log("Cleared active save");
    }

}
