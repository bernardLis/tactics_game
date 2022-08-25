using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class RunManager : Singleton<RunManager>
{
    GameManager _gameManager;
    List<JourneyEvent> _availableEvents;

    public bool WasJourneySetUp;
    public int JourneySeed = 0; // TODO: this is a bad idea, probably
    public JourneyNodeData CurrentJourneyNode;
    [HideInInspector] public List<JourneyPath> JourneyPaths = new();
    [HideInInspector] public List<JourneyNodeData> VisitedJourneyNodes = new();

    public int Gold { get; private set; }
    public int SavingsAccountGold { get; private set; }
    public int InterestEarned { get; private set; }
    [HideInInspector] public List<Character> PlayerTroops = new();
    [HideInInspector] public List<Item> PlayerItemPouch = new();
    [HideInInspector] public List<Ability> PlayerAbilityPouch = new();

    public JourneyNode CurrentNode { get; private set; }
    public JourneyNodeReward JourneyNodeReward { get; private set; }

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
        _availableEvents = new(_gameManager.GameDatabase.GetAllEvents());

        JourneySeed = System.DateTime.Now.Millisecond;
        WasJourneySetUp = false;
        CurrentJourneyNode = new JourneyNodeData();
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

    public void SetCurrentJourneyNode(JourneyNodeData n)
    {
        VisitedJourneyNodes.Add(n);
        CurrentJourneyNode = n;
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

        if (_availableEvents.Count == _gameManager.GameDatabase.GetAllEvents().Length)
            ev = _availableEvents[0];
        if (ev == null)
            ev = _availableEvents[Random.Range(0, _availableEvents.Count)];

        _availableEvents.Remove(ev);
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
