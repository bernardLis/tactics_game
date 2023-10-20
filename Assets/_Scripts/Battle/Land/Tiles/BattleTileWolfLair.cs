using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleTileWolfLair : BattleTile
{
    [Header("Wolf Lair")]
    [SerializeField] BattleWolfLair _wolfLair;
    public override void EnableTile()
    {
        base.EnableTile();
        _wolfLair = GetComponentInChildren<BattleWolfLair>();
        _wolfLair.Initialize();
    }

    public override void StartTileFight()
    {
        base.StartTileFight();

        _battleWaveManager.OnWaveSpawned += OnWaveSpawned;
    }

    public override void Secured()
    {
        base.Secured();

        _battleWaveManager.OnWaveSpawned -= OnWaveSpawned;
    }

    void OnWaveSpawned()
    {
        _wolfLair.SpawnWave(_battleWaveManager.CurrentDifficulty);
    }

}
