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

    [SerializeField] Sound _battleMusic;
    public Battle LoadedBattle { get; private set; }

    public VisualElement Root { get; private set; }
    Label _fpsLabel;
    float _deltaTime;

    [SerializeField] Transform _entityHolder;

    // skybox rotation https://forum.unity.com/threads/rotate-a-skybox.130639/
    int _rotationProperty;
    float _initRot;
    Material _skyMat;
    [SerializeField] float _skyboxRotationSpeed = 0.2f;

    [SerializeField] TextMeshProUGUI _timerText;
    [SerializeField] TextMeshProUGUI _scoreText;

    public float BattleTime { get; private set; }

    Hero _playerHero;
    Hero _opponentHero;

    int _initialPlayerEntityCount;
    int _initialOpponentEntityCount;

    [SerializeField] GameObject _playerSpawnPoint;
    [SerializeField] GameObject _enemySpawnPoint;

    public List<BattleEntity> PlayerEntities = new();
    public List<BattleEntity> OpponentEntities = new();

    public List<BattleEntity> KilledPlayerEntities = new();
    public List<BattleEntity> KilledOpponentEntities = new();

    public bool IsEndingBattleBlocked;
    bool _battleFinalized = false;


    public event Action OnBattleFinalized;

    protected override void Awake()
    {
        base.Awake();

        Root = GetComponent<UIDocument>().rootVisualElement;
        _fpsLabel = Root.Q<Label>("fpsLabel");

        VisualElement bottomPanel = Root.Q<VisualElement>("bottomPanel");
    }
    void Start()
    {
        _gameManager = GameManager.Instance;
        _gameManager.SaveJsonData();
        LoadedBattle = _gameManager.SelectedBattle;

        Root.Q<VisualElement>("vfx").pickingMode = PickingMode.Ignore;

        _rotationProperty = Shader.PropertyToID("_Rotation");
        _skyMat = RenderSettings.skybox;
        _initRot = _skyMat.GetFloat(_rotationProperty);

#if UNITY_EDITOR
        GetComponent<BattleInputManager>().OnEnterClicked += WinBattle;
#endif
        AudioManager.Instance.PlayMusic(_battleMusic);
    }

    void Update()
    {
        BattleTime += Time.deltaTime;
        TimeSpan time = TimeSpan.FromSeconds(BattleTime);
        _timerText.text = $"{time.Minutes:D2}:{time.Seconds:D2}";

        _skyMat.SetFloat(_rotationProperty, UnityEngine.Time.time * _skyboxRotationSpeed);

        _deltaTime += (Time.deltaTime - _deltaTime) * 0.1f;
        float fps = 1.0f / _deltaTime;
        _fpsLabel.text = $"{Mathf.Ceil(fps)}";
    }

    public void Initialize(Hero playerHero, Hero opponentHero, List<ArmyGroup> playerArmy, List<ArmyGroup> opponentArmy)
    {
        _battleFinalized = false;

        GetComponent<BattleGrabManager>().Initialize();

        if (playerHero != null)
        {
            _playerHero = playerHero;
            GetComponent<BattleHeroManager>().Initialize(playerHero);
            GetComponent<BattleAbilityManager>().Initialize(playerHero);
        }

        if (opponentHero != null) _opponentHero = opponentHero;

        if (playerArmy == null) return;

        foreach (ArmyGroup ag in playerArmy)
            InstantiatePlayer(ag);
        foreach (ArmyGroup ag in opponentArmy)
            InstantiateOpponent(ag.ArmyEntity, ag.EntityCount);

        _initialPlayerEntityCount = PlayerEntities.Count;
        _initialOpponentEntityCount = OpponentEntities.Count;

        _scoreText.text = $"{_initialPlayerEntityCount} : {_initialOpponentEntityCount}";

        if (_gameManager == null) _gameManager = GameManager.Instance;
        _gameManager.ToggleTimer(true);

        GetComponent<BattleLogManager>().Initialize(PlayerEntities, OpponentEntities);
    }

    public void InstantiatePlayer(ArmyGroup ag)
    {
        ag.InitializeBattle();

        ArmyEntity entityInstance = Instantiate(ag.ArmyEntity);
        entityInstance.HeroInfluence(_playerHero);

        for (int i = 0; i < ag.EntityCount; i++)
        {
            Vector3 pos = _playerSpawnPoint.transform.position + new Vector3(Random.Range(-5, 5), 0, Random.Range(-5, 5));
            GameObject instance = Instantiate(ag.ArmyEntity.Prefab, pos, Quaternion.identity);
            instance.layer = 8;
            instance.transform.parent = _entityHolder;
            BattleEntity be = instance.GetComponent<BattleEntity>();
            be.Initialize(true, entityInstance, ref OpponentEntities);
            PlayerEntities.Add(be);
            be.OnEnemyKilled += ag.AddKill;
            be.OnDamageDealt += ag.AddDmgDealt;
            be.OnDamageTaken += ag.AddDmgTaken;
            be.OnDeath += OnPlayerDeath;
        }
    }

    void InstantiateOpponent(ArmyEntity entity, int count)
    {
        ArmyEntity entityInstance = Instantiate(entity);
        entityInstance.HeroInfluence(_opponentHero);

        for (int i = 0; i < count; i++)
        {
            Vector3 pos = _enemySpawnPoint.transform.position + new Vector3(Random.Range(-5, 5), 0, Random.Range(-5, 5));
            Quaternion rotation = Quaternion.Euler(0, 180, 0);
            GameObject instance = Instantiate(entity.Prefab, pos, rotation);
            instance.transform.parent = _entityHolder;
            BattleEntity be = instance.GetComponent<BattleEntity>();
            be.Initialize(false, entity, ref PlayerEntities);
            OpponentEntities.Add(be);
            be.OnDeath += OnEnemyDeath;
        }
    }

    void OnPlayerDeath(BattleEntity be, BattleEntity killer, Ability killerAbility)
    {
        KilledPlayerEntities.Add(be);
        PlayerEntities.Remove(be);
        _scoreText.text = $"{PlayerEntities.Count} : {OpponentEntities.Count}";

        if (PlayerEntities.Count == 0)
            StartCoroutine(BattleLost());
    }

    void OnEnemyDeath(BattleEntity be, BattleEntity killer, Ability killerAbility)
    {
        KilledOpponentEntities.Add(be);
        OpponentEntities.Remove(be);
        _scoreText.text = $"{PlayerEntities.Count} : {OpponentEntities.Count}";

        if (OpponentEntities.Count == 0)
            StartCoroutine(BattleWon());
    }

    IEnumerator BattleLost()
    {
        if (IsEndingBattleBlocked)
        {
            yield return FinalizeBattle();
            yield break;
        }
        
        ConfirmPopUp popUp = new();
        popUp.Initialize(Root, () => _gameManager.LoadScene(Scenes.MainMenu),
                "Oh... you lost, for now the only choice is to go to main menu, and try again. Do you want do it?");
        popUp.HideCancelButton();
        yield return null;

        // HERE: testing   
        //        _gameManager.ClearSaveData();
    }

    IEnumerator BattleWon()
    {
        if (IsEndingBattleBlocked)
        {
            yield return FinalizeBattle();
            yield break;
        }

        LoadedBattle.Won = true;

        VisualElement topPanel = Root.Q<VisualElement>("topPanel");
        topPanel.Clear();

        Label label = new("Battle won!");
        label.style.fontSize = 64;
        topPanel.Add(label);

        yield return FinalizeBattle();
    }

    IEnumerator FinalizeBattle()
    {
        _gameManager.BattleNumber++; // TODO: hihihihihi
        // if entities die "at the same time" it triggers twice
        if (_battleFinalized) yield break;
        _battleFinalized = true;

        // HERE: testing
        // yield return new WaitForSeconds(4f);
        yield return new WaitForSeconds(1f);

        ClearAllEntities();

        OnBattleFinalized?.Invoke();
    }

    void ClearAllEntities()
    {
        PlayerEntities.Clear();
        OpponentEntities.Clear();
        foreach (Transform child in _entityHolder.transform)
        {
            child.transform.DOKill(child.transform);
            GameObject.Destroy(child.gameObject);
        }
    }

#if UNITY_EDITOR
    [ContextMenu("Win Battle")]
    public void WinBattle()
    {
        List<BattleEntity> copy = new(OpponentEntities);
        foreach (BattleEntity be in copy)
        {
            StartCoroutine(be.Die());
        }
    }
#endif
}
