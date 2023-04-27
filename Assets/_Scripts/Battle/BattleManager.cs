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

    [SerializeField] Material _playerMaterial;
    [SerializeField] Material _enemyMaterial;

    int _initialPlayerEntityCount;
    int _initialEnemyEntityCount;

    [SerializeField] GameObject _playerSpawnPoint;
    [SerializeField] GameObject _enemySpawnPoint;

    public List<BattleEntity> PlayerEntities = new();
    public List<BattleEntity> EnemyEntities = new();

    public List<BattleEntity> KilledPlayerEntities = new();
    public List<BattleEntity> KilledEnemyEntities = new();

    void Start()
    {
        _gameManager = GameManager.Instance;
        LoadedBattle = _gameManager.SelectedBattle;

        _root = GetComponent<UIDocument>().rootVisualElement;
        _root.Q<VisualElement>("vfx").pickingMode = PickingMode.Ignore;

        _initialEnemyEntityCount = LoadedBattle.GetTotalNumberOfEnemies();
        _initialPlayerEntityCount = _gameManager.PlayerHero.GetTotalNumberOfArmyEntities();

        _rotationProperty = Shader.PropertyToID("_Rotation");
        _skyMat = RenderSettings.skybox;
        _initRot = _skyMat.GetFloat(_rotationProperty);

        _scoreText.text = $"{_initialPlayerEntityCount} : {_initialEnemyEntityCount}";

        foreach (ArmyGroup ag in _gameManager.PlayerHero.Army)
            InstantiatePlayer(ag.ArmyEntity, ag.EntityCount);
        foreach (ArmyGroup ag in LoadedBattle.Opponent.Army)
            InstantiateEnemy(ag.ArmyEntity, ag.EntityCount);

        _gameManager.ToggleTimer(true);

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

    public void InstantiatePlayer(ArmyEntity entity, int count)
    {
        ArmyEntity entityInstance = Instantiate(entity);
        entityInstance.HeroInfluence(_gameManager.PlayerHero);

        for (int i = 0; i < count; i++)
        {
            Vector3 pos = _playerSpawnPoint.transform.position + new Vector3(Random.Range(-5, 5), Random.Range(-5, 5));
            GameObject instance = Instantiate(entity.Prefab, pos, Quaternion.identity);
            instance.layer = 8;
            instance.transform.parent = _entityHolder;
            BattleEntity be = instance.GetComponent<BattleEntity>();
            be.Initialize(_playerMaterial, _gameManager.PlayerHero, entityInstance, ref EnemyEntities);
            PlayerEntities.Add(be);
            be.OnDeath += OnPlayerDeath;
        }
    }

    void InstantiateEnemy(ArmyEntity entity, int count)
    {
        for (int i = 0; i < count; i++)
        {
            Vector3 pos = _enemySpawnPoint.transform.position + new Vector3(Random.Range(-5, 5), Random.Range(-5, 5));
            GameObject instance = Instantiate(entity.Prefab, pos, Quaternion.identity);
            instance.transform.parent = _entityHolder;
            BattleEntity be = instance.GetComponent<BattleEntity>();
            be.Initialize(_enemyMaterial, LoadedBattle.Opponent, entity, ref PlayerEntities);
            EnemyEntities.Add(be);
            be.OnDeath += OnEnemyDeath;
        }
    }

    void OnPlayerDeath(BattleEntity be)
    {
        KilledPlayerEntities.Add(be);
        PlayerEntities.Remove(be);
        _scoreText.text = $"{PlayerEntities.Count} : {EnemyEntities.Count}";

        if (PlayerEntities.Count == 0)
            BattleLost();
    }

    void OnEnemyDeath(BattleEntity be)
    {
        KilledEnemyEntities.Add(be);
        EnemyEntities.Remove(be);
        _scoreText.text = $"{PlayerEntities.Count} : {EnemyEntities.Count}";

        if (EnemyEntities.Count == 0)
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
        yield return new WaitForSeconds(2f);
        BattleResult r = new(_root);
    }

#if UNITY_EDITOR
    [ContextMenu("Win Battle")]
    void WinBattle()
    {
        List<BattleEntity> copy = new(EnemyEntities);
        foreach (BattleEntity be in copy)
        {
            StartCoroutine(be.Die());
        }
    }
#endif
}
