using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UIElements;

public class BattleManager : MonoBehaviour
{
    GameManager _gameManager;
    Battle _loadedBattle;

    VisualElement _root;

    // skybox rotation https://forum.unity.com/threads/rotate-a-skybox.130639/
    int _rotationProperty;
    float _initRot;
    Material _skyMat;
    [SerializeField] float _skyboxRotationSpeed = 0.2f;

    [SerializeField] TextMeshProUGUI _textMesh; // HERE: something smarter

    [SerializeField] List<ArmyEntity> _enemyStats = new();
    [SerializeField] List<ArmyEntity> _playerStats;

    [SerializeField] GameObject _playerPrefab;
    [SerializeField] GameObject _enemyPrefab;

    [SerializeField] int _numberOfPlayersToSpawn;
    [SerializeField] int _numberOfEnemiesToSpawn;

    [SerializeField] GameObject _playerSpawnPoint;
    [SerializeField] GameObject _enemySpawnPoint;

    public List<BattleEntity> PlayerEntities = new();
    public List<BattleEntity> EnemyEntities = new();

    void Start()
    {
        _gameManager = GameManager.Instance;
        _loadedBattle = _gameManager.SelectedBattle;

        _root = GetComponent<UIDocument>().rootVisualElement;

        _numberOfEnemiesToSpawn = _loadedBattle.NumberOfMeleeEnemies + _loadedBattle.NumberOfRangedEnemies;

        _rotationProperty = Shader.PropertyToID("_Rotation");
        _skyMat = RenderSettings.skybox;
        _initRot = _skyMat.GetFloat(_rotationProperty);

        _textMesh.text = $"{_numberOfPlayersToSpawn} : {_numberOfEnemiesToSpawn}";

        foreach (ArmyGroup ag in _loadedBattle.Character.ArmyGroups)
            InstantiatePlayer(ag.ArmyEntity);
        for (int i = 0; i < _numberOfEnemiesToSpawn; i++)
            InstantiateEnemy();

        InitializeEnemies();
    }

    void InitializeEnemies()
    {
        for (int i = 0; i < _numberOfEnemiesToSpawn; i++)
        {
            if (i <= _loadedBattle.NumberOfMeleeEnemies)
                EnemyEntities[i].Initialize(_enemyStats[0], ref PlayerEntities);
            else
                EnemyEntities[i].Initialize(_enemyStats[1], ref PlayerEntities);
        }
    }

    void Update() => _skyMat.SetFloat(_rotationProperty, Time.time * _skyboxRotationSpeed);

    void InstantiatePlayer(ArmyEntity entity)
    {
        Vector3 pos = _playerSpawnPoint.transform.position + new Vector3(Random.Range(-5, 5), Random.Range(-5, 5));
        GameObject instance = Instantiate(_playerPrefab, pos, Quaternion.identity);
        BattleEntity be = instance.GetComponent<BattleEntity>();
        be.Initialize(entity, ref EnemyEntities);
        PlayerEntities.Add(be);
        be.OnDeath += OnPlayerDeath;
    }

    void InstantiateEnemy()
    {
        Vector3 pos = _enemySpawnPoint.transform.position + new Vector3(Random.Range(-5, 5), Random.Range(-5, 5));
        GameObject instance = Instantiate(_enemyPrefab, pos, Quaternion.identity);
        BattleEntity be = instance.GetComponent<BattleEntity>();
        EnemyEntities.Add(be);
        be.OnDeath += OnEnemyDeath;
    }

    void OnPlayerDeath(BattleEntity be)
    {
        PlayerEntities.Remove(be);
        _textMesh.text = $"{PlayerEntities.Count} : {EnemyEntities.Count}";

        if (PlayerEntities.Count == 0)
            BattleLost();
    }

    void OnEnemyDeath(BattleEntity be)
    {
        EnemyEntities.Remove(be);
        _textMesh.text = $"{PlayerEntities.Count} : {EnemyEntities.Count}";

        if (EnemyEntities.Count == 0)
            BattleWon();
    }

    void BattleLost()
    {
        StartCoroutine(FinalizeBattle(EnemyEntities));
    }

    void BattleWon()
    {
        _loadedBattle.Won = true;
        StartCoroutine(FinalizeBattle(PlayerEntities));
    }

    IEnumerator FinalizeBattle(List<BattleEntity> entities)
    {
        yield return new WaitForSeconds(2f);
        BattleResult r = new(_root, _loadedBattle, entities);
    }
}
