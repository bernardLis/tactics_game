using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;
using DG.Tweening;

public class BattleManager : Singleton<BattleManager>
{
    GameManager _gameManager;

    BattleGrabManager _battleGrabManager;
    BattleHeroManager _battleHeroManager;
    BattleAbilityManager _battleAbilityManager;

    [SerializeField] Sound _battleMusic;
    public Battle LoadedBattle { get; private set; }

    public VisualElement Root { get; private set; }

    VisualElement _infoPanel;
    Label _timerLabel;
    Label _opponentsLeftLabel;

    public Transform EntityHolder;

    // skybox rotation https://forum.unity.com/threads/rotate-a-skybox.130639/
    int _rotationProperty;
    float _initRot;
    Material _skyMat;
    [SerializeField] float _skyboxRotationSpeed = 0.2f;

    public float BattleTime { get; private set; }

    Hero _playerHero;
    Hero _opponentHero;

    public List<BattleEntity> PlayerEntities = new();
    public List<BattleEntity> OpponentEntities = new();

    public List<BattleEntity> KilledPlayerEntities = new();
    public List<BattleEntity> KilledOpponentEntities = new();

    [HideInInspector] public List<Pickup> CollectedPickups = new();

    public bool BlockBattleEnd;
    public bool BattleFinalized { get; private set; }

    public event Action OnBattleInitialized;
    public event Action<BattleEntity> OnPlayerEntityAdded;
    public event Action<int> OnPlayerEntityDeath;
    public event Action<int> OnOpponentEntityDeath;
    public event Action OnBattleFinalized;

    protected override void Awake()
    {
        base.Awake();

        Root = GetComponent<UIDocument>().rootVisualElement;
        VisualElement bottomPanel = Root.Q<VisualElement>("bottomPanel");
        _infoPanel = Root.Q<VisualElement>("infoPanel");
        _timerLabel = _infoPanel.Q<Label>("timer");
        _opponentsLeftLabel = _infoPanel.Q<Label>("enemyCount");
    }

    void Start()
    {
        VFXCameraManager.Instance.gameObject.SetActive(false);

        _gameManager = GameManager.Instance;
        _gameManager.SaveJsonData();
        LoadedBattle = _gameManager.SelectedBattle;

        Root.Q<VisualElement>("vfx").pickingMode = PickingMode.Ignore;

        _rotationProperty = Shader.PropertyToID("_Rotation");
        _skyMat = RenderSettings.skybox;
        _initRot = _skyMat.GetFloat(_rotationProperty);

        AudioManager.Instance.PlayMusic(_battleMusic);

        _timerLabel.style.display = DisplayStyle.Flex;
        _opponentsLeftLabel.style.display = DisplayStyle.Flex;

#if UNITY_EDITOR
        GetComponent<BattleInputManager>().OnEnterClicked += CheatWinBattle;
#endif
    }

    void Update()
    {
        _skyMat.SetFloat(_rotationProperty, UnityEngine.Time.time * _skyboxRotationSpeed);
    }

    IEnumerator UpdateTimer()
    {
        while (true)
        {
            BattleTime += 1f;
            TimeSpan time = TimeSpan.FromSeconds(BattleTime);
            _timerLabel.text = $"{time.Minutes:D2}:{time.Seconds:D2}";
            yield return new WaitForSeconds(1f);
        }
    }

    public void Initialize(Hero playerHero, List<BattleEntity> playerArmy, List<BattleEntity> opponentArmy)
    {
        BattleFinalized = false;

        _battleGrabManager = GetComponent<BattleGrabManager>();
        _battleGrabManager.enabled = true;
        _battleGrabManager.Initialize();

        if (playerHero != null)
        {
            _playerHero = playerHero;

            _battleHeroManager = GetComponent<BattleHeroManager>();
            _battleHeroManager.enabled = true;
            _battleHeroManager.Initialize(playerHero);

            _battleAbilityManager = GetComponent<BattleAbilityManager>();
            _battleAbilityManager.enabled = true;
            _battleAbilityManager.Initialize(playerHero);
        }

        foreach (BattleEntity b in playerArmy)
            AddPlayerArmyEntity(b);

        foreach (BattleEntity b in opponentArmy)
            AddOpponentArmyEntity(b);

        StartCoroutine(UpdateTimer());

        OnBattleInitialized?.Invoke();

        _infoPanel.style.opacity = 0f;
        _infoPanel.style.display = DisplayStyle.Flex;
        DOTween.To(x => _infoPanel.style.opacity = x, 0, 1, 0.5f).SetDelay(0.5f);
    }

    public void AddPlayerArmyEntity(BattleEntity b)
    {
        b.transform.parent = EntityHolder;

        b.InitializeBattle(0, ref OpponentEntities);
        b.gameObject.layer = 10;
        PlayerEntities.Add(b);
        b.OnDeath += OnPlayerDeath;
        OnPlayerEntityAdded?.Invoke(b);
    }

    public void AddOpponentArmyEntities(List<BattleEntity> list)
    {
        foreach (BattleEntity b in list)
            AddOpponentArmyEntity(b);
    }

    public void AddOpponentArmyEntity(BattleEntity b)
    {
        b.transform.parent = EntityHolder;

        b.InitializeBattle(1, ref PlayerEntities);
        b.gameObject.layer = 11;
        OpponentEntities.Add(b);
        b.OnDeath += OnOpponentDeath;

        UpdateOpponentCountLabel();
    }

    void OnPlayerDeath(BattleEntity be, BattleEntity killer, Ability killerAbility)
    {
        KilledPlayerEntities.Add(be);
        PlayerEntities.Remove(be);
        OnPlayerEntityDeath?.Invoke(PlayerEntities.Count);

        if (BlockBattleEnd) return;
        if (PlayerEntities.Count == 0)
            StartCoroutine(BattleLost());
    }

    void OnOpponentDeath(BattleEntity be, BattleEntity killer, Ability killerAbility)
    {
        KilledOpponentEntities.Add(be);
        OpponentEntities.Remove(be);
        OnOpponentEntityDeath?.Invoke(OpponentEntities.Count);

        UpdateOpponentCountLabel();

        if (BlockBattleEnd) return;
        if (OpponentEntities.Count == 0)
            StartCoroutine(BattleWon());
    }

    void UpdateOpponentCountLabel()
    {
        _opponentsLeftLabel.text = $"Enemies Left: {OpponentEntities.Count}";

    }
    public List<BattleEntity> GetAllies(BattleEntity battleEntity)
    {
        if (battleEntity.Team == 0) return PlayerEntities;
        //if (battleEntity.Team == 1) 
        return OpponentEntities;
    }

    public void CollectPickup(Pickup p) { CollectedPickups.Add(p); }

    public void LoseBattle() { StartCoroutine(BattleLost()); }
    public void WinBattle() { StartCoroutine(BattleWon()); }

    IEnumerator BattleLost()
    {
        LoadedBattle.Won = false;

        ConfirmPopUp popUp = new();
        popUp.Initialize(Root, () => _gameManager.ClearSaveData(),
                "Oh... you lost, for now the only choice is to go to main menu, and try again. Do you want do it?");
        popUp.HideCancelButton();
        yield return null;
    }

    IEnumerator BattleWon()
    {
        LoadedBattle.Won = true;

        VisualElement topPanel = Root.Q<VisualElement>("topPanel");
        topPanel.Clear();

        Label label = new("Battle won!");
        label.AddToClassList("battle__won-label");
        label.style.opacity = 0;
        DOTween.To(x => label.style.opacity = x, 0, 1, 0.5f);

        topPanel.Add(label);

        if (_gameManager.BattleNumber == 8)
        {
            ConfirmPopUp popUp = new();
            popUp.style.backgroundColor = new StyleColor(new Color(0, 0, 0, 0.5f));
            popUp.Initialize(Root, () => StartCoroutine(FinalizeBattle()),
                    "You won the game! I owe you a beer for winning this prototype. You can continue playing or you can try another element. Btw. let me know what you think about this experience!.");
            popUp.HideCancelButton();
            yield break;
        }

        yield return FinalizeBattle();
    }

    IEnumerator FinalizeBattle()
    {
        _gameManager.BattleNumber++; // TODO: hihihihihi
        // if entities die "at the same time" it triggers twice
        if (BattleFinalized) yield break;
        BattleFinalized = true;

        yield return new WaitForSeconds(3f);

        ClearAllEntities();

        OnBattleFinalized?.Invoke();
    }

    public void ClearAllEntities()
    {
        PlayerEntities.Clear();
        OpponentEntities.Clear();
        foreach (Transform child in EntityHolder.transform)
        {
            child.transform.DOKill(child.transform);
            GameObject.Destroy(child.gameObject);
        }
    }

#if UNITY_EDITOR
    [ContextMenu("Win Battle")]
    public void CheatWinBattle()
    {
        List<BattleEntity> copy = new(OpponentEntities);
        foreach (BattleEntity be in copy)
        {
            StartCoroutine(be.Die());
        }
    }
    [ContextMenu("Alternative Win Battle")]
    public void WinBattleAlternative()
    {
        StartCoroutine(BattleWon());
    }
#endif

}
