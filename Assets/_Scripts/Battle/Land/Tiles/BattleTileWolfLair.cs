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
        _wolfLair.transform.localScale = Vector3.zero;
        _wolfLair.Initialize();

        _wolfLair.transform.DOScale(Vector3.one, 1f)
                            .SetEase(Ease.OutBack)
                            .SetDelay(2.5f);
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
