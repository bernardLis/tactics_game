using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;
using DG.Tweening;
using Cinemachine;

public class BattleManager : Singleton<BattleManager>
{
    public static bool BlockBattleInput;

    GameManager _gameManager;

    BattleHeroManager _battleHeroManager;
    BattleIntroManager _battleIntroManager;

    [SerializeField] Sound _battleMusic;

    public Battle LoadedBattle { get; private set; }

    public VisualElement Root { get; private set; }

    VisualElement _infoPanel;
    Label _timerLabel;

    public Transform EntityHolder;

    public bool IsTimerOn { get; private set; }

    public Hero PlayerHero { get; private set; }
    public BattleHero BattleHero => _battleHeroManager.BattleHero;

    public List<BattleEntity> PlayerCreatures = new();
    public List<BattleEntity> OpponentEntities = new();

    public List<BattleEntity> KilledPlayerEntities = new();
    public List<BattleEntity> KilledOpponentEntities = new();

    public List<BattleTurret> PlayerTurrets = new();

    public List<BattlePickUp> Pickups = new();

    public bool BlockBattleEnd;
    public bool BattleFinalized { get; private set; }

    IEnumerator _timerCoroutine;
    int _battleTime;

    public event Action OnBattleInitialized;
    public event Action<BattleCreature> OnPlayerCreatureAdded;
    public event Action<BattleEntity> OnPlayerEntityDeath;
    public event Action<BattleEntity> OnOpponentEntityAdded;
    public event Action<BattleEntity> OnOpponentEntityDeath;
    public event Action<BattleTurret> OnPlayerTurretAdded;
    public event Action OnBattleFinalized;

    public event Action OnGamePaused;
    public event Action OnGameResumed;

    protected override void Awake()
    {
        base.Awake();

        Root = GetComponent<UIDocument>().rootVisualElement;

        _infoPanel = Root.Q<VisualElement>("infoPanel");
        _timerLabel = _infoPanel.Q<Label>("timer");
    }

    void Start()
    {
        // HERE: render texture issue unity must resolve
        // VFXCameraManager.Instance.gameObject.SetActive(false);

        _gameManager = GameManager.Instance;
        _gameManager.SaveJsonData();
        LoadedBattle = _gameManager.CurrentBattle;
        Root.Q<VisualElement>("vfx").pickingMode = PickingMode.Ignore;

        _timerLabel.style.display = DisplayStyle.Flex;

#if UNITY_EDITOR
        GetComponent<BattleInputManager>().OnEnterClicked += LevelUpHero;
        GetComponent<BattleInputManager>().OnContinueClicked += KillAllOpponents;

#endif
    }

    public void Initialize(Hero playerHero)
    {
        BattleFinalized = false;
        _battleTime = 0;

        if (playerHero != null)
        {
            PlayerHero = playerHero;
            _battleHeroManager = GetComponent<BattleHeroManager>();
            _battleHeroManager.enabled = true;
            _battleHeroManager.Initialize(playerHero);
        }

        ResumeTimer();

        _battleIntroManager = BattleIntroManager.Instance;
        if (_battleIntroManager != null)
            _battleIntroManager.OnIntroFinished += () => AudioManager.Instance.PlayMusic(_battleMusic);

        OnBattleInitialized?.Invoke();
    }

    public void PauseGame()
    {
        Debug.Log($"Pausing the game...");
        Time.timeScale = 0f;
        PauseTimer();
        BlockBattleInput = true;

        OnGamePaused?.Invoke();
    }

    public void ResumeGame()
    {
        Debug.Log($"Resuming the game...");
        Time.timeScale = 1f;
        ResumeTimer();
        BlockBattleInput = false;

        OnGameResumed?.Invoke();
    }

    void PauseTimer()
    {
        if (this == null) return;

        IsTimerOn = false;
        if (_timerCoroutine != null)
            StopCoroutine(_timerCoroutine);
    }

    void ResumeTimer()
    {
        IsTimerOn = true;
        if (_timerCoroutine != null)
            StopCoroutine(_timerCoroutine);

        _timerCoroutine = UpdateTimer();
        StartCoroutine(_timerCoroutine);
    }

    IEnumerator UpdateTimer()
    {
        while (true)
        {
            _battleTime++;
            int minutes = Mathf.FloorToInt(_battleTime / 60f);
            int seconds = Mathf.FloorToInt(_battleTime - minutes * 60);

            _timerLabel.text = string.Format("{0:00}:{1:00}", minutes, seconds);
            yield return new WaitForSeconds(1f);
        }
    }

    public float GetTime() { return _battleTime; }

    public void AddPlayerArmyEntities(List<BattleEntity> list)
    {
        foreach (BattleEntity b in list)
            AddPlayerArmyEntity(b);
    }

    public void AddPlayerArmyEntity(BattleEntity b)
    {
        b.transform.parent = EntityHolder;

        b.InitializeBattle(ref OpponentEntities);
        if (b is BattleHero hero) b.gameObject.layer = 8;
        else b.gameObject.layer = 10;
        PlayerCreatures.Add(b);
        b.OnDeath += OnPlayerCreatureDeath;
        if (b is BattleCreature creature)
            OnPlayerCreatureAdded?.Invoke(creature);
    }

    public void AddOpponentArmyEntities(List<BattleEntity> list)
    {
        foreach (BattleEntity b in list)
            AddOpponentArmyEntity(b);
    }

    public void AddOpponentArmyEntity(BattleEntity b)
    {
        b.transform.parent = EntityHolder;

        b.InitializeBattle(ref PlayerCreatures);
        b.gameObject.layer = 11;
        OpponentEntities.Add(b);
        b.OnDeath += OnOpponentDeath;
        OnOpponentEntityAdded?.Invoke(b);
    }

    void OnPlayerCreatureDeath(BattleEntity be, EntityFight killer)
    {
        KilledPlayerEntities.Add(be);
        PlayerCreatures.Remove(be);
        OnPlayerEntityDeath?.Invoke(be);
    }

    void OnOpponentDeath(BattleEntity be, EntityFight killer)
    {
        KilledOpponentEntities.Add(be);
        OpponentEntities.Remove(be);
        OnOpponentEntityDeath?.Invoke(be);
    }

    public List<BattleEntity> GetAllies(BattleEntity battleEntity)
    {
        if (battleEntity.Team == 0) return PlayerCreatures;
        //if (battleEntity.Team == 1) 
        return OpponentEntities;
    }

    public List<BattleEntity> GetOpponents(int team)
    {
        if (team == 0) return OpponentEntities;
        //if (battleEntity.Team == 1) 
        return PlayerCreatures;
    }

    public void LoseBattle() { StartCoroutine(BattleLost()); }
    public void WinBattle() { StartCoroutine(BattleWon()); }

    public void AddPlayerTurret(BattleTurret turret)
    {
        PlayerTurrets.Add(turret);
        OnPlayerTurretAdded?.Invoke(turret);
    }

    public void AddPickup(BattlePickUp pickup)
    {
        Pickups.Add(pickup);
        pickup.OnPickedUp += () => Pickups.Remove(pickup);
    }

    IEnumerator BattleLost()
    {
        BattleLostScreen lostScreen = new();
        yield return FinalizeBattle();
    }

    IEnumerator BattleWon()
    {
        BattleWonScreen wonScreen = new();
        wonScreen.OnFinishedPlaying += () => StartCoroutine(FinalizeBattle());
        yield return null;
    }

    IEnumerator FinalizeBattle()
    {
        // if entities die "at the same time" it triggers twice
        if (BattleFinalized) yield break;
        BattleFinalized = true;

        _gameManager.BattleNumber++; // TODO: hihihihihi

        yield return new WaitForSeconds(3f);

        // ClearAllEntities();

        OnBattleFinalized?.Invoke();
    }

    public void ClearAllEntities()
    {
        PlayerCreatures.Clear();
        OpponentEntities.Clear();
        foreach (Transform child in EntityHolder.transform)
        {
            child.transform.DOKill(child.transform);
            Destroy(child.gameObject);
        }
    }

#if UNITY_EDITOR
    [ContextMenu("Kill All Opponents")]
    public void KillAllOpponents()
    {
        if (this == null) return;
        List<BattleEntity> copy = new(OpponentEntities);
        foreach (BattleEntity be in copy)
        {
            StartCoroutine(be.Die());
        }
    }
    [ContextMenu("Force Win Battle")]
    public void WinBattleAlternative()
    {
        StartCoroutine(BattleWon());
    }

    [ContextMenu("Force Lose Battle")]
    public void LoseBattleAlternative()
    {
        StartCoroutine(BattleLost());
    }

    [ContextMenu("Level up hero")]
    public void LevelUpHero()
    {
        _gameManager.PlayerHero.AddExp(_gameManager.PlayerHero.ExpForNextLevel.Value);
    }

#endif

}
