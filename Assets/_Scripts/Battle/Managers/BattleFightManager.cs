using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using DG.Tweening;
using System;
using Random = UnityEngine.Random;

public class BattleFightManager : Singleton<BattleFightManager>
{
    GameManager _gameManager;
    BattleManager _battleManager;
    BattleTooltipManager _battleTooltipManager;

    public int CurrentDifficulty { get; private set; }

    BattleLandTile _currentTile;

    public List<Fight> Fights = new();
    Fight _currentFight;

    void Start()
    {
        _gameManager = GameManager.Instance;
        _battleManager = BattleManager.Instance;
        _battleManager.OnOpponentEntityDeath += OnOpponentEntityDeath;
        _battleTooltipManager = BattleTooltipManager.Instance;

        CurrentDifficulty = 1;
    }

    public void InitializeWaves(BattleLandTile tile)
    {
        _currentTile = tile;
        StartCoroutine(TileFightCoroutine());
    }

    IEnumerator TileFightCoroutine()
    {
        CreateFight();
        yield return StartFight();
        CurrentDifficulty++;
    }

    void CreateFight()
    {
        Fight fight = ScriptableObject.CreateInstance<Fight>();
        fight.CreateFight(CurrentDifficulty);
        _currentFight = fight;
        Fights.Add(fight);
    }

    IEnumerator Countdown(int seconds)
    {
        for (int i = seconds; i > 0; i--)
        {
            _battleTooltipManager.ShowInfo(i.ToString(), 0.8f);
            yield return new WaitForSeconds(1);
        }
    }

    IEnumerator StartFight()
    {
        yield return Countdown(3);

        foreach (EnemyWave wave in _currentFight.EnemyWaves)
        {
            _battleTooltipManager.ShowInfo($"Wave {_currentFight.CurrentWaveIndex + 1}/{_currentFight.EnemyWaves.Count}", 1.5f);
            StartCoroutine(SpawnOpponentGroup(wave));
            yield return new WaitForSeconds(_currentFight.DelayBetweenWaves);
        }

    }

    void OnOpponentEntityDeath(BattleEntity _)
    {
        if (_battleManager.OpponentEntities.Count != 0) return;
        if (!_currentFight.IsFinished()) return;

        _battleTooltipManager.ShowInfo("Tile secured!", 1.5f);
        _currentTile.Secured();
    }

    IEnumerator SpawnOpponentGroup(EnemyWave group)
    {
        yield return SpawnMinions(group);
        _currentFight.SpawningGroupFinished();
    }

    IEnumerator SpawnMinions(EnemyWave group)
    {
        float theta = 0;
        float thetaStep = 2 * Mathf.PI / group.Minions.Count;
        for (int i = 0; i < group.Minions.Count; i++)
        {
            Minion m = group.Minions[i];
            m.InitializeBattle(1);

            float radius = _currentTile.Scale * 0.5f - 1;
            Vector3 center = _currentTile.transform.position;
            float x = Mathf.Cos(theta) * radius + center.x;
            float y = 1f;
            float z = Mathf.Sin(theta) * radius + center.z;
            Vector3 pos = new(x, y, z);

            BattleEntity be = SpawnEntity(m, pos);
            _battleManager.AddOpponentArmyEntity(be);
            theta += thetaStep;
            yield return new WaitForSeconds(0.1f);
        }
    }

    BattleEntity SpawnEntity(Entity entity, Vector3 spawnPos)
    {
        GameObject instance = Instantiate(entity.Prefab, spawnPos, transform.localRotation);
        BattleEntity be = instance.GetComponent<BattleEntity>();
        be.InitializeEntity(entity);
        return be;
    }
}
