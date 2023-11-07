using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BattleBuildingBoss : BattleBuilding
{
    [SerializeField] Transform _bossSpawnPoint;
    [SerializeField] Boss _bossOriginal;
    [SerializeField] BattleEntitySpawner _battleEntitySpawnerPrefab;

    BattleBoss _boss;

    public override void Initialize(Building building)
    {
        base.Initialize(building);
        _battleFightManager.OnBossFightStarted += OnBossFightStarted;
    }

    protected override void ShowBuilding()
    {
        // TODO: could pillars falling one by one
    }

    void OnBossFightStarted()
    {
        BattleEntitySpawner spawner = Instantiate(_battleEntitySpawnerPrefab,
                                                    _bossSpawnPoint.position,
                                                    Quaternion.identity);
        Boss boss = Instantiate(_bossOriginal);
        spawner.SpawnEntities(new List<Entity> { boss }, team: 1);
        spawner.OnSpawnComplete += list =>
        {
            _boss = list[0] as BattleBoss;
            _battleManager.AddOpponentArmyEntity(_boss);
        };
    }
}
