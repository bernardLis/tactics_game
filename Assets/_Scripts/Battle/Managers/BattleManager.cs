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

    BattleHeroManager _battleHeroManager;
    
    [SerializeField] Sound _battleMusic;

    public Battle LoadedBattle { get; private set; }

    public VisualElement Root { get; private set; }

    VisualElement _infoPanel;
    Label _timerLabel;


    public Transform EntityHolder;

    // skybox rotation https://forum.unity.com/threads/rotate-a-skybox.130639/
    int _rotationProperty;
    float _initRot;
    Material _skyMat;
    [SerializeField] float _skyboxRotationSpeed = 0.2f;

    public float BattleTime { get; private set; }

    bool _timerIsOn;

    public List<BattleEntity> PlayerEntities = new();
    public List<BattleEntity> OpponentEntities = new();

    public List<BattleEntity> KilledPlayerEntities = new();
    public List<BattleEntity> KilledOpponentEntities = new();

    [HideInInspector] public List<Pickup> CollectedPickups = new();

    public bool BlockBattleEnd;
    public bool BattleFinalized { get; private set; }

    public event Action OnBattleInitialized;
    public event Action<BattleCreature> OnPlayerCreatureAdded;
    public event Action<int> OnPlayerEntityDeath;
    public event Action<int> OnOpponentEntityDeath;
    public event Action OnBattleFinalized;

    public event Action OnGamePaused;
    public event Action OnGameResumed;

    protected override void Awake()
    {
        base.Awake();

        Root = GetComponent<UIDocument>().rootVisualElement;
        VisualElement bottomPanel = Root.Q<VisualElement>("bottomPanel");
        _infoPanel = Root.Q<VisualElement>("infoPanel");
        _timerLabel = _infoPanel.Q<Label>("timer");
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

#if UNITY_EDITOR
        GetComponent<BattleInputManager>().OnEnterClicked += CheatWinBattle;
#endif
    }

    void Update()
    {
        _skyMat.SetFloat(_rotationProperty, UnityEngine.Time.time * _skyboxRotationSpeed);
    }

    public void PauseGame()
    {
        Time.timeScale = 0f;
        OnGamePaused?.Invoke();
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        OnGameResumed?.Invoke();
    }

    IEnumerator UpdateTimer()
    {
        while (_timerIsOn)
        {
            BattleTime += 1f;
            TimeSpan time = TimeSpan.FromSeconds(BattleTime);
            _timerLabel.text = $"{time.Minutes:D2}:{time.Seconds:D2}";
            if (time.Minutes >= _gameManager.SelectedBattle.Duration)
                BattleWon();
            yield return new WaitForSeconds(1f);
        }
    }

    public void Initialize(Hero playerHero)
    {
        BattleFinalized = false;

        if (playerHero != null)
        {
            _battleHeroManager = GetComponent<BattleHeroManager>();
            _battleHeroManager.enabled = true;
            _battleHeroManager.Initialize(playerHero);
        }
        _timerIsOn = true;
        StartCoroutine(UpdateTimer());
        OnBattleInitialized?.Invoke();
    }


    public void AddPlayerArmyEntities(List<BattleEntity> list)
    {
        foreach (BattleEntity b in list)
            AddPlayerArmyEntity(b);
    }

    public void AddPlayerArmyEntity(BattleEntity b)
    {
        b.transform.parent = EntityHolder;

        b.InitializeBattle(0, ref OpponentEntities);
        b.gameObject.layer = 10;
        PlayerEntities.Add(b);
        b.OnDeath += OnPlayerDeath;
        if (b is BattleCreature)
            OnPlayerCreatureAdded?.Invoke((BattleCreature)b);
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
    }

    void OnPlayerDeath(BattleEntity be, BattleEntity killer, Ability killerAbility)
    {
        KilledPlayerEntities.Add(be);
        PlayerEntities.Remove(be);
        OnPlayerEntityDeath?.Invoke(PlayerEntities.Count);
    }

    void OnOpponentDeath(BattleEntity be, BattleEntity killer, Ability killerAbility)
    {
        KilledOpponentEntities.Add(be);
        OpponentEntities.Remove(be);
        OnOpponentEntityDeath?.Invoke(OpponentEntities.Count);

        // TODO: price for experience
        if (killer is BattleCreature)
        {
            Debug.Log($"killer is creature");
            int gain = Mathf.RoundToInt(be.Entity.Price * 0.5f);
            _gameManager.PlayerHero.AddExp(gain);
            ((BattleCreature)killer).Creature.AddExp(gain);
            return;
        }
        _gameManager.PlayerHero.AddExp(be.Entity.Price);
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
    [ContextMenu("Kill All Opponents")]
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
