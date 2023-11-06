using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BattleBuildingBoss : BattleBuilding
{
    [SerializeField] Transform _bossSpawnPoint;
    [SerializeField] Creature _bossOriginal;
    [SerializeField] BattleEntitySpawner _battleEntitySpawnerPrefab;

    BattleCreature _boss;

    public override void Initialize(Building building)
    {
        base.Initialize(building);
        _battleFightManager.OnBossFightStarted += OnBossFightStarted;
    }

    void OnBossFightStarted()
    {
        BattleEntitySpawner spawner = Instantiate(_battleEntitySpawnerPrefab,
                                                    _bossSpawnPoint.position,
                                                    Quaternion.identity);
        Creature boss = Instantiate(_bossOriginal);
        spawner.SpawnEntities(new List<Entity> { boss }, team: 1);
        spawner.OnSpawnComplete += list =>
        {
            _boss = list[0] as BattleCreature;
            _boss.transform.DOScale(3, 1f).SetEase(Ease.OutBack);
            _battleManager.AddOpponentArmyEntity(_boss);
        };
    }
}
