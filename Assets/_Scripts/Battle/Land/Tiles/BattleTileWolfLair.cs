using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BattleTileWolfLair : BattleTile
{
    [Header("Wolf Lair")]
    BattleWolfLair _wolfLair;
    public override void EnableTile()
    {
        base.EnableTile();
        _wolfLair = GetComponentInChildren<BattleWolfLair>();
        _wolfLair.Initialize();

    }

    public override void StartTileFight()
    {
        base.StartTileFight();

        _battleFightManager.OnWaveSpawned += OnWaveSpawned;
    }

    public override void Secured()
    {
        base.Secured();

        _battleFightManager.OnWaveSpawned -= OnWaveSpawned;
        _wolfLair.Secured();
    }

    void OnWaveSpawned()
    {
        _wolfLair.SpawnWave(_battleFightManager.CurrentDifficulty);
    }

}
