using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BattleManager : MonoBehaviour
{

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
        _rotationProperty = Shader.PropertyToID("_Rotation");
        _skyMat = RenderSettings.skybox;
        _initRot = _skyMat.GetFloat(_rotationProperty);

        _textMesh.text = $"{_numberOfPlayersToSpawn} : {_numberOfEnemiesToSpawn}";

        for (int i = 0; i < _numberOfPlayersToSpawn; i++)
            InstantiatePlayer();
        for (int i = 0; i < _numberOfEnemiesToSpawn; i++)
            InstantiateEnemy();

        foreach (BattleEntity be in PlayerEntities)
            be.Initialize(_playerStats[Random.Range(0, _playerStats.Count)], ref EnemyEntities);
        foreach (BattleEntity be in EnemyEntities)
            be.Initialize(_enemyStats[Random.Range(0, _enemyStats.Count)], ref PlayerEntities);
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
    }

    void OnEnemyDeath(BattleEntity be)
    {
        EnemyEntities.Remove(be);
        _textMesh.text = $"{PlayerEntities.Count} : {EnemyEntities.Count}";
    }


}
