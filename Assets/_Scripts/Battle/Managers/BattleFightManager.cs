using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using DG.Tweening;
using System;
using Random = UnityEngine.Random;

public class BattleFightManager : Singleton<BattleFightManager>
{
    BattleManager _battleManager;
    BattleTooltipManager _battleTooltipManager;

    public int CurrentDifficulty { get; private set; }
    public bool IsFightActive { get; private set; }
    BattleTile _currentTile;

    public List<Fight> Fights = new();
    Fight _currentFight;

    public event Action OnFightStarted;
    public event Action OnFightEnded;
    public event Action OnWaveSpawned;
    void Start()
    {
        _battleManager = BattleManager.Instance;
        _battleManager.OnOpponentEntityDeath += OnOpponentEntityDeath;
        _battleTooltipManager = BattleTooltipManager.Instance;

        CurrentDifficulty = 1;
    }

    public void InitializeFight(BattleTile tile)
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
            _battleTooltipManager.ShowGameInfo(i.ToString(), 0.8f);
            yield return new WaitForSeconds(1);
        }
    }

    IEnumerator StartFight()
    {
        IsFightActive = true;
        OnFightStarted?.Invoke();

        yield return Countdown(1); // HERE: testing 3


        foreach (EnemyWave wave in _currentFight.EnemyWaves)
        {
            _battleTooltipManager.ShowGameInfo($"Wave {_currentFight.CurrentWaveIndex + 1}/{_currentFight.EnemyWaves.Count}", 1.5f);
            StartCoroutine(SpawnOpponentGroup(wave));
            OnWaveSpawned?.Invoke();
            yield return new WaitForSeconds(_currentFight.DelayBetweenWaves);
        }
    }

    void OnOpponentEntityDeath(BattleEntity _)
    {
        if (_battleManager.OpponentEntities.Count != 0) return;
        if (!_currentFight.IsFinished()) return;

        IsFightActive = false;
        _battleTooltipManager.ShowGameInfo("Tile secured!", 1.5f);
        OnFightEnded?.Invoke();
    }

    IEnumerator SpawnOpponentGroup(EnemyWave group)
    {
        yield return SpawnMinions(group);
        _currentFight.SpawningGroupFinished();
    }

    IEnumerator SpawnMinions(EnemyWave group)
    {
        for (int i = 0; i < group.Minions.Count; i++)
        {
            Minion m = group.Minions[i];
            m.InitializeBattle(1);

            Vector3 pos = _currentTile.GetMinionPosition(i, group.Minions.Count);

            BattleEntity be = SpawnEntity(m, pos);
            _battleManager.AddOpponentArmyEntity(be);
            yield return new WaitForSeconds(0.05f);
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
