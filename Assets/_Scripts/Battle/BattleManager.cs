using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    [SerializeField] List<Stats> _enemyStats = new();
    [SerializeField] Stats _playerStats;

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
        for (int i = 0; i < _numberOfPlayersToSpawn; i++)
            InstantiatePlayer();
        for (int i = 0; i < _numberOfEnemiesToSpawn; i++)
            InstantiateEnemy();

        foreach (BattleEntity be in PlayerEntities)
            be.Initialize(_playerStats, ref EnemyEntities);
        foreach (BattleEntity be in EnemyEntities)
            be.Initialize(_enemyStats[Random.Range(0, _enemyStats.Count)], ref PlayerEntities);
    }

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


    void OnPlayerDeath(BattleEntity be) { PlayerEntities.Remove(be); }

    void OnEnemyDeath(BattleEntity be) { EnemyEntities.Remove(be); }


}
