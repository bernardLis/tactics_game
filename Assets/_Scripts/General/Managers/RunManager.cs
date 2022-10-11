using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class RunManager : Singleton<RunManager>
{
    GameManager _gameManager;
    public List<JourneyEvent> AvailableEvents { get; private set; }

    public bool WasJourneySetUp;
    public int JourneySeed = 0; // TODO: this is a bad idea, probably

    [HideInInspector] public List<JourneyPath> JourneyPaths = new();
    [HideInInspector] public List<JourneyNodeData> VisitedJourneyNodes = new();

    public int Gold { get; private set; }
    public int SavingsAccountGold { get; private set; }
    public int InterestEarned { get; private set; }
    [HideInInspector] public List<Character> PlayerTroops = new();
    [HideInInspector] public List<Item> PlayerItemPouch = new();
    [HideInInspector] public List<Ability> PlayerAbilityPouch = new();

    public JourneyNode CurrentNode { get; private set; }
    public JourneyNodeReward JourneyNodeReward; // HERE: RunManager has a set reward.  //{ get; private set; }

    public event Action<int> OnGoldChanged;
    public event Action<int> OnSavingsAccountChanged;

    protected override void Awake()
    {
        base.Awake();
        _gameManager = GameManager.Instance;

        _gameManager.OnLevelLoaded += OnLevelLoaded;
    }

    public void InitializeNewRun()
    {
        AvailableEvents = new(_gameManager.GameDatabase.GetAllEvents());

        JourneySeed = System.DateTime.Now.Millisecond;
        Random.InitState(JourneySeed);

        WasJourneySetUp = false;
        //  CurrentNodeData = new JourneyNodeData();
        JourneyPaths = new();
        VisitedJourneyNodes = new();

        CreatePlayerTroops();
        Gold = 0;
        SavingsAccountGold = 0;
        InterestEarned = 0;
        PlayerItemPouch = new();
        PlayerAbilityPouch = new();

        CurrentNode = null;
        JourneyNodeReward = null;

        foreach (GlobalUpgrade item in _gameManager.PurchasedGlobalUpgrades)
            if (item.UpgradeType == UpgradeType.Run)
                item.Initialize();

        _gameManager.SaveJsonData();
    }

    public void PopulateRunFromSaveData(SaveData saveData)
    {
        ChangeGoldValue(saveData.Gold);
        ChangeSavingsAccountValue(saveData.SavingsAccountGold);
        ChangeTotalInterestValue(saveData.TotalInterestEarned);

        JourneySeed = saveData.JourneySeed;
        Random.InitState(JourneySeed);

        //    CurrentNodeData = saveData.VisitedJourneyNodes[VisitedJourneyNodes.Count - 1];
        VisitedJourneyNodes = saveData.VisitedJourneyNodes;
        PlayerTroops = new();
        foreach (CharacterData data in saveData.Characters)
        {
            Character playerCharacter = (Character)ScriptableObject.CreateInstance<Character>();
            playerCharacter.Create(data);
            PlayerTroops.Add(playerCharacter);
        }

        PopulateAvailableEvents(saveData.AvailableEvents);
        PopulateItemPouch(saveData.ItemPouch);
        PopulateAbilityPouch(saveData.AbilityPouch);
    }

    public void PopulateAvailableEvents(List<string> eventIds)
    {
        if (_gameManager == null)
            _gameManager = GameManager.Instance;

        AvailableEvents = new();
        foreach (string id in eventIds)
            AvailableEvents.Add(_gameManager.GameDatabase.GetEventById(id));
    }

    public void PopulateItemPouch(List<string> itemIds)
    {
        PlayerItemPouch = new();
        foreach (string itemReferenceId in itemIds)
            PlayerItemPouch.Add(_gameManager.GameDatabase.GetItemByReference(itemReferenceId));
    }

    public void PopulateAbilityPouch(List<string> abilityIds)
    {
        PlayerAbilityPouch = new();
        foreach (string abilityReferenceId in abilityIds)
            PlayerAbilityPouch.Add(_gameManager.GameDatabase.GetAbilityByReferenceId(abilityReferenceId));
    }

    void OnLevelLoaded(string level)
    {
        if (level == Scenes.Journey)
            PayInterest();
    }

    void PayInterest()
    {
        int interest = Mathf.CeilToInt(SavingsAccountGold * 0.1f); // TODO: interest could be a variable
        InterestEarned += interest;
        ChangeSavingsAccountValue(interest);
    }

    public void ChangeTotalInterestValue(int i)
    {
        InterestEarned += i;
    }

    public void ChangeGoldValue(int o)
    {
        Gold += o;
        OnGoldChanged?.Invoke(Gold);
        if (_gameManager != null)
            _gameManager.SaveJsonData();
    }

    public void ChangeSavingsAccountValue(int o)
    {
        SavingsAccountGold += o;
        OnSavingsAccountChanged?.Invoke(SavingsAccountGold);
        if (_gameManager != null)
            _gameManager.SaveJsonData();
    }

    void CreatePlayerTroops()
    {
        List<Character> playerCharacters = new(_gameManager.GameDatabase.GetAllStarterTroops());
        PlayerTroops = new();
        // adding global upgrades to characters
        foreach (Character character in playerCharacters)
        {
            Character instance = Instantiate(character);
            foreach (GlobalUpgrade item in _gameManager.PurchasedGlobalUpgrades)
                if (item is GlobalCharacterUpgrade)
                {
                    GlobalCharacterUpgrade i = (GlobalCharacterUpgrade)item;
                    i.Initialize(instance);
                }
            PlayerTroops.Add(instance);
        }
    }

    public void SetPlayerTroops(List<Character> troops)
    {
        PlayerTroops = new(troops);
        _gameManager.SaveJsonData();
    }

    public void AddCharacterToTroops(Character character)
    {
        PlayerTroops.Add(character);
        _gameManager.SaveJsonData();
    }

    public void AddEnemyToAllBattleNodes(Brain b)
    {
        foreach (JourneyPath path in JourneyPaths)
        {
            foreach (JourneyNode node in path.Nodes)
            {
                if (node.NodeType == JourneyNodeType.Battle)
                {
                    BattleNode n = (BattleNode)node;
                    n.AddEnemy(b);
                }
            }
        }
    }

    public void AddItemToPouch(Item item)
    {
        PlayerItemPouch.Add(item);
        _gameManager.SaveJsonData();
    }

    public void RemoveItemFromPouch(Item item)
    {
        PlayerItemPouch.Remove(item);
        _gameManager.SaveJsonData();
    }

    public void AddAbilityToPouch(Ability ability)
    {
        PlayerAbilityPouch.Add(ability);
        _gameManager.SaveJsonData();
    }

    public void RemoveAbilityFromPouch(Ability ability)
    {
        PlayerAbilityPouch.Remove(ability);
        _gameManager.SaveJsonData();
    }

    public void LoadLevelFromNode(JourneyNode node)
    {
        CurrentNode = node;
        _gameManager.LoadLevel(node.SceneToLoad);
    }

    public void SetNodeReward(JourneyNodeReward r)
    {
        if (r == null)
            return;
        JourneyNodeReward clone = Instantiate(r);
        clone.Initialize();
        JourneyNodeReward = clone;
    }

    public void GetReward()
    {
        if (JourneyNodeReward != null)
            JourneyNodeReward.GetReward();

        JourneyNodeReward = null;
    }


    public JourneyEvent ChooseEvent()
    {
        JourneyEvent ev = null;

        // TODO: temporary solution for uncle to always catch up with you as a first event
        if (AvailableEvents.Count == _gameManager.GameDatabase.GetAllEvents().Length)
            ev = AvailableEvents[0];
        if (ev == null)
            ev = AvailableEvents[Random.Range(0, AvailableEvents.Count)];

        AvailableEvents.Remove(ev);
        return ev;
    }

#if UNITY_EDITOR

    [ContextMenu("Add 10 gold")]
    void Add10Gold()
    {
        ChangeGoldValue(10);
    }

    [ContextMenu("Remove 10 gold")]
    void Remove10Gold()
    {
        ChangeGoldValue(-10);
    }

    [ContextMenu("Add 10 obols")]
    void Add10Obols()
    {
        _gameManager.ChangeObolValue(10);
    }

    [ContextMenu("Remove 10 obols")]
    void Remove10Obols()
    {
        _gameManager.ChangeObolValue(-10);
    }

#endif

}
