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
    public List<BattleOpponentPortal> OpponentPortals = new();

    public List<BattleEntity> PlayerCreatures = new();
    public List<BattleEntity> OpponentEntities = new();

    public List<BattleEntity> KilledPlayerEntities = new();
    public List<BattleEntity> KilledOpponentEntities = new();

    public List<BattleTurret> PlayerTurrets = new();

    [HideInInspector] public List<Loot> CollectedLoots = new();

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
        VFXCameraManager.Instance.gameObject.SetActive(false);

        _gameManager = GameManager.Instance;
        _gameManager.SaveJsonData();
        LoadedBattle = _gameManager.CurrentBattle;
        Root.Q<VisualElement>("vfx").pickingMode = PickingMode.Ignore;

        _timerLabel.style.display = DisplayStyle.Flex;

#if UNITY_EDITOR
        GetComponent<BattleInputManager>().OnEnterClicked += LevelUpHero;
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
        OnGamePaused?.Invoke();
    }

    public void ResumeGame()
    {
        Debug.Log($"Resuming the game...");
        Time.timeScale = 1f;
        ResumeTimer();
        OnGameResumed?.Invoke();
    }

    void PauseTimer()
    {
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
            if (_battleTime >= _gameManager.CurrentBattle.Duration)
                StartCoroutine(BattleWon());
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

        b.InitializeBattle(0, ref OpponentEntities);
        b.gameObject.layer = 10;
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

        b.InitializeBattle(1, ref PlayerCreatures);
        b.gameObject.layer = 11;
        OpponentEntities.Add(b);
        b.OnDeath += OnOpponentDeath;
        OnOpponentEntityAdded?.Invoke(b);
    }

    void OnPlayerCreatureDeath(BattleEntity be, GameObject killer)
    {
        KilledPlayerEntities.Add(be);
        PlayerCreatures.Remove(be);
        _gameManager.PlayerHero.RemoveCreature((Creature)be.Entity);
        OnPlayerEntityDeath?.Invoke(be);
    }

    void OnOpponentDeath(BattleEntity be, GameObject killer)
    {
        KilledOpponentEntities.Add(be);
        OpponentEntities.Remove(be);
        OnOpponentEntityDeath?.Invoke(be);

        // TODO: price for experience
        if (killer == null)
        {
            _gameManager.PlayerHero.AddExp(be.Entity.Price);
            return;
        }

        if (killer.TryGetComponent(out BattleCreature creature))
        {
            int heroExpTax = Mathf.RoundToInt(be.Entity.Price * 0.25f);
            _gameManager.PlayerHero.AddExp(heroExpTax);
            creature.Creature.AddExp(be.Entity.Price - heroExpTax);
        }
    }

    public List<BattleEntity> GetAllies(BattleEntity battleEntity)
    {
        if (battleEntity.Team == 0) return PlayerCreatures;
        //if (battleEntity.Team == 1) 
        return OpponentEntities;
    }

    public void CollectLoot(Loot p) { CollectedLoots.Add(p); }

    public void LoseBattle() { StartCoroutine(BattleLost()); }
    public void WinBattle() { StartCoroutine(BattleWon()); }

    public bool IsPlayerTeamFull()
    {
        if (PlayerCreatures.Count >= BattleSpire.Instance.Spire.StoreyTroops.MaxTroopsTree.CurrentValue.Value)
            return true;
        return false;
    }

    public void AddPortal(BattleOpponentPortal portal) { OpponentPortals.Add(portal); }
    public void AddPlayerTurret(BattleTurret turret)
    {
        PlayerTurrets.Add(turret);
        OnPlayerTurretAdded?.Invoke(turret);
    }

    IEnumerator BattleLost()
    {
        // LoadedBattle.Won = false;

        BattleLostScreen lostScreen = new();
        yield return null;
        // ConfirmPopUp popUp = new();
        // popUp.Initialize(Root, () => _gameManager.ClearSaveData(),
        //         "Oh... you lost, for now the only choice is to go to main menu, and try again. Do you want do it?");
        // popUp.HideCancelButton();
        // yield return null;
        yield return FinalizeBattle();

    }

    IEnumerator BattleWon()
    {
        // LoadedBattle.Won = true;
        BattleWonScreen lostScreen = new();
        yield return null;

        // VisualElement topPanel = Root.Q<VisualElement>("topPanel");
        // topPanel.Clear();

        // Label label = new("Battle won!");
        // label.AddToClassList("battle__won-label");
        // label.style.opacity = 0;
        // DOTween.To(x => label.style.opacity = x, 0, 1, 0.5f);

        // topPanel.Add(label);

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
    public void CheatWinBattle()
    {
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
