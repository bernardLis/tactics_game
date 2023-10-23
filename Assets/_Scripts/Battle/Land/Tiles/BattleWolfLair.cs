using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleWolfLair : MonoBehaviour
{
    BattleManager _battleManager;
    ObjectShaders _objectShaders;

    [SerializeField] Creature _wolf;
    [SerializeField] int _wolfCount;
    [SerializeField] BattleEntitySpawner _spawnerPrefab;
    [SerializeField] Transform _spawnPoint;

    public void Initialize()
    {
        _battleManager = BattleManager.Instance;
        _objectShaders = GetComponent<ObjectShaders>();

        _objectShaders.GrayScale();
        // rotate lair to face player
        transform.LookAt(_battleManager.GetComponent<BattleHeroManager>().BattleHero.transform.position);
    }

    public void SpawnWave(int difficulty)
    {
        List<Entity> entitiesToSpawn = new();
        for (int i = 0; i < _wolfCount; i++)
            entitiesToSpawn.Add(Instantiate(_wolf));

        BattleEntitySpawner spawner = Instantiate(_spawnerPrefab, _spawnPoint.position, transform.rotation);
        spawner.SpawnEntities(entitiesToSpawn);
        spawner.OnSpawnComplete += (l) =>
        {
            _battleManager.AddOpponentArmyEntities(l);
            spawner.DestroySelf();
        };
    }

    public void Secured()
    {
        _objectShaders.LitShader();

    }
}
