using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BattleManager : MonoBehaviour
{
    GameManager _gameManager;
    Battle _loadedBattle;

    // skybox rotation https://forum.unity.com/threads/rotate-a-skybox.130639/
    int _rotationProperty;
    float _initRot;
    Material _skyMat;
    [SerializeField] float _skyboxRotationSpeed = 0.2f;

    [SerializeField] TextMeshProUGUI _textMesh; // HERE: something smarter

    [SerializeField] List<Stats> _enemyStats = new();
    [SerializeField] List<Stats> _playerStats;

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

        _numberOfEnemiesToSpawn = _loadedBattle.NumberOfMeleeEnemies + _loadedBattle.NumberOfRangedEnemies;
        _numberOfPlayersToSpawn = _loadedBattle.Character.NumberOfMeleeArmy + _loadedBattle.Character.NumberOfMeleeArmy;

        _rotationProperty = Shader.PropertyToID("_Rotation");
        _skyMat = RenderSettings.skybox;
        _initRot = _skyMat.GetFloat(_rotationProperty);

        _textMesh.text = $"{_numberOfPlayersToSpawn} : {_numberOfEnemiesToSpawn}";

        for (int i = 0; i < _numberOfPlayersToSpawn; i++)
            InstantiatePlayer();
        for (int i = 0; i < _numberOfEnemiesToSpawn; i++)
            InstantiateEnemy();

        InitializePlayers();
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

    void InitializePlayers()
    {
        for (int i = 0; i < _numberOfPlayersToSpawn; i++)
        {
            if (i <= _loadedBattle.NumberOfMeleeEnemies)
                PlayerEntities[i].Initialize(_playerStats[0], ref EnemyEntities);
            else
                PlayerEntities[i].Initialize(_playerStats[1], ref EnemyEntities);
        }
    }

    void Update() => _skyMat.SetFloat(_rotationProperty, Time.time * _skyboxRotationSpeed);

    void InstantiatePlayer()
    {
        Vector3 pos = _playerSpawnPoint.transform.position + new Vector3(Random.Range(-5, 5), Random.Range(-5, 5));
        GameObject instance = Instantiate(_playerPrefab, pos, Quaternion.identity);
        BattleEntity be = instance.GetComponent<BattleEntity>();
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
        StartCoroutine(FinalizeBattle());
    }

    void BattleWon()
    {
        _loadedBattle.Won = true;
        StartCoroutine(FinalizeBattle());
    }

    IEnumerator FinalizeBattle()
    {
        yield return new WaitForSeconds(2f);
        _gameManager.LoadMap();
    }
}
