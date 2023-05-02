using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public class BattleManager : Singleton<BattleManager>
{
    GameManager _gameManager;
    public Battle LoadedBattle { get; private set; }

    VisualElement _root;

    [SerializeField] Transform _entityHolder;

    // skybox rotation https://forum.unity.com/threads/rotate-a-skybox.130639/
    int _rotationProperty;
    float _initRot;
    Material _skyMat;
    [SerializeField] float _skyboxRotationSpeed = 0.2f;

    [SerializeField] TextMeshProUGUI _timerText;
    [SerializeField] TextMeshProUGUI _scoreText;

    public float Time { get; private set; }

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

    bool _battleFinalized = false;

    public event Action OnBattleFinalized;
    void Start()
    {
        _gameManager = GameManager.Instance;
        LoadedBattle = _gameManager.SelectedBattle;

        _root = GetComponent<UIDocument>().rootVisualElement;
        _root.Q<VisualElement>("vfx").pickingMode = PickingMode.Ignore;

        _rotationProperty = Shader.PropertyToID("_Rotation");
        _skyMat = RenderSettings.skybox;
        _initRot = _skyMat.GetFloat(_rotationProperty);

        // HERE: for testing
        GetComponent<BattleInputManager>().OnEnterClicked += WinBattle;
    }

    void Update()
    {
        Time += UnityEngine.Time.deltaTime;
        TimeSpan time = TimeSpan.FromSeconds(Time);
        _timerText.text = $"{time.Minutes:D2}:{time.Seconds:D2}";

        _skyMat.SetFloat(_rotationProperty, UnityEngine.Time.time * _skyboxRotationSpeed);
    }

    public void Initialize(Hero playerHero, Hero opponentHero, List<ArmyGroup> playerArmy, List<ArmyGroup> opponentArmy)
    {
        _battleFinalized = false;

        if (playerHero != null) _playerHero = playerHero;
        if (opponentHero != null) _opponentHero = opponentHero;

        foreach (ArmyGroup ag in playerArmy)
            InstantiatePlayer(ag.ArmyEntity, ag.EntityCount);
        foreach (ArmyGroup ag in opponentArmy)
            InstantiateOpponent(ag.ArmyEntity, ag.EntityCount);

        _initialPlayerEntityCount = PlayerEntities.Count;
        _initialOpponentEntityCount = OpponentEntities.Count;

        _scoreText.text = $"{_initialPlayerEntityCount} : {_initialOpponentEntityCount}";

        if (_gameManager == null) _gameManager = GameManager.Instance;
        _gameManager.ToggleTimer(true);
    }

    public void InstantiatePlayer(ArmyEntity entity, int count)
    {
        ArmyEntity entityInstance = Instantiate(entity);
        entityInstance.HeroInfluence(_playerHero);

        for (int i = 0; i < count; i++)
        {
            Vector3 pos = _playerSpawnPoint.transform.position + new Vector3(Random.Range(-5, 5), 0, Random.Range(-5, 5));
            GameObject instance = Instantiate(entity.Prefab, pos, Quaternion.identity);
            instance.layer = 8;
            instance.transform.parent = _entityHolder;
            BattleEntity be = instance.GetComponent<BattleEntity>();
            be.Initialize(true, entityInstance, ref OpponentEntities);
            PlayerEntities.Add(be);
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

    void OnPlayerDeath(BattleEntity be)
    {
        KilledPlayerEntities.Add(be);
        PlayerEntities.Remove(be);
        _scoreText.text = $"{PlayerEntities.Count} : {OpponentEntities.Count}";

        if (PlayerEntities.Count == 0)
            BattleLost();
    }

    void OnEnemyDeath(BattleEntity be)
    {
        KilledOpponentEntities.Add(be);
        OpponentEntities.Remove(be);
        _scoreText.text = $"{PlayerEntities.Count} : {OpponentEntities.Count}";

        if (OpponentEntities.Count == 0)
            BattleWon();
    }

    void BattleLost()
    {
        StartCoroutine(FinalizeBattle());
    }

    void BattleWon()
    {
        LoadedBattle.Won = true;
        StartCoroutine(FinalizeBattle());
    }

    IEnumerator FinalizeBattle()
    {
        // if entities die "at the same time" it triggers twice
        if (_battleFinalized) yield break;
        _battleFinalized = true;

        yield return new WaitForSeconds(4f);

        if (_playerHero != null)
        {
            BattleResult r = new(_root);
        }
        yield return new WaitForSeconds(1f); // TODO: hehe
        ClearAllEntities();

        OnBattleFinalized?.Invoke();
    }

    void ClearAllEntities()
    {
        PlayerEntities.Clear();
        OpponentEntities.Clear();
        foreach (Transform child in _entityHolder.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
    }

#if UNITY_EDITOR
    [ContextMenu("Win Battle")]
    void WinBattle()
    {
        List<BattleEntity> copy = new(OpponentEntities);
        foreach (BattleEntity be in copy)
        {
            StartCoroutine(be.Die());
        }
    }
#endif
}
