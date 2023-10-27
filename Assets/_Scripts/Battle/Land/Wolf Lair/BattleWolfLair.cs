using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class BattleWolfLair : BattleBuilding
{

    [SerializeField] BattleEntitySpawner _spawnerPrefab;
    [SerializeField] Transform _spawnPoint;

    public List<BattleCreature> _friendlyWolves = new();

    public override void Initialize(Building building)
    {
        base.Initialize(building);

        _building.OnUpgradePurchased += () => StartProductionCoroutine();

        transform.localScale = Vector3.zero;
        transform.DOScale(Vector3.one, 1f)
                            .SetEase(Ease.OutBack)
                            .SetDelay(2.5f);

        transform.LookAt(_battleManager.GetComponent<BattleHeroManager>().BattleHero.transform.position);
    }

    public override void SpawnWave()
    {
        int difficulty = _battleFightManager.CurrentDifficulty;
        BuildingUpgrade bu = _building.GetCurrentUpgrade();

        // TODO: difficulty
        List<Entity> entitiesToSpawn = new();
        for (int i = 0; i < difficulty * 3; i++)
            entitiesToSpawn.Add(Instantiate(bu.ProducedCreature));

        BattleEntitySpawner spawner = Instantiate(_spawnerPrefab, _spawnPoint.position, transform.rotation);
        spawner.SpawnEntities(entitiesToSpawn);
        spawner.OnSpawnComplete += (l) =>
        {
            _battleManager.AddOpponentArmyEntities(l);
            spawner.DestroySelf();
        };
    }

    protected override IEnumerator ProductionCoroutine()
    {
        while (_friendlyWolves.Count < _building.GetCurrentUpgrade().ProductionLimit)
        {
            SpawnFriendlyWolf();
            yield return ProductionDelay();
        }
        _productionCoroutine = null;
    }

    void SpawnFriendlyWolf()
    {
        BattleEntitySpawner spawner = Instantiate(_spawnerPrefab,
                                _spawnPoint.position, transform.rotation);

        Creature wolf = Instantiate(_building.GetCurrentUpgrade().ProducedCreature);
        spawner.SpawnEntities(new List<Entity>() { wolf });
        spawner.OnSpawnComplete += (l) =>
        {
            // now I need to track the spawned wolf
            BattleCreature bc = l[0] as BattleCreature;
            _friendlyWolves.Add(bc);
            // if it dies, and coroutine is inactive - restart coroutine
            bc.OnDeath += (_, __) =>
            {
                _friendlyWolves.Remove(bc);
                StartProductionCoroutine();
            };

            _battleManager.AddPlayerArmyEntities(l);
            spawner.DestroySelf();
        };
    }
}
