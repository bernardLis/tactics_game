using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using DG.Tweening;
using System;
using Random = UnityEngine.Random;

public class BattleMinionManager : PoolManager<BattleEntity>
{
    BattleManager _battleManager;
    BattleAreaManager _battleAreaManager;

    public int CurrentDifficulty { get; private set; }

    Fight _currentFight;


    [SerializeField] GameObject _minionPrefab;


    public void Initialize()
    {
        _battleManager = BattleManager.Instance;
        _battleManager.OnOpponentEntityDeath += OnOpponentEntityDeath;
        _battleAreaManager = _battleManager.GetComponent<BattleAreaManager>();

        CurrentDifficulty = 1;

        CreatePool(_minionPrefab);

        CreateFight();
        StartCoroutine(StartFight());
    }

    void CreateFight()
    {
        Fight fight = ScriptableObject.CreateInstance<Fight>();
        fight.CreateFight(CurrentDifficulty);
        _currentFight = fight;
    }

    IEnumerator StartFight()
    {
        yield return new WaitForSeconds(2f);

        while (true)
        {
            // HERE: testing
            if (_battleManager.IsGameLoopBlocked)
            {
                yield return new WaitForSeconds(_currentFight.DelayBetweenWaves);
                continue;
            }
            SpawnWave();
            yield return new WaitForSeconds(_currentFight.DelayBetweenWaves);
        }
    }

    void OnOpponentEntityDeath(BattleEntity _)
    {
        if (_battleManager.OpponentEntities.Count != 0) return;
        if (!_currentFight.IsFinished()) return;
    }

    public void SpawnWave()
    {
        EnemyWave wave = _currentFight.EnemyWaves[_currentFight.CurrentWaveIndex];
        StartCoroutine(SpawnWaveCoroutine(wave));
    }

    IEnumerator SpawnWaveCoroutine(EnemyWave wave)
    {
        // spawn minions on tiles next to the player or on the same tile
        List<BattleTile> tiles = _battleAreaManager.GetTilesAroundPlayer();
        yield return SpawnMinions(wave, tiles);
        _currentFight.SpawningWaveFinished();
    }

    public IEnumerator SpawnMinions(EnemyWave wave, List<BattleTile> tiles)
    {
        BattleTile tile = tiles[Random.Range(0, tiles.Count)];
        for (int i = 0; i < wave.Minions.Count; i++)
        {
            Minion m = wave.Minions[i];
            m.InitializeBattle(1);

            Vector3 pos = tile.GetMinionPosition(i, wave.Minions.Count);

            // BattleEntity be = SpawnEntity(m, pos);
            BattleEntity be = GetObjectFromPool();
            if (be == null)
            {
                Debug.LogError("No more minions in pool");
                yield break;
            }
            be.InitializeEntity(m, 1);
            be.transform.position = pos;
            be.gameObject.SetActive(true);
            _battleManager.AddOpponentArmyEntity(be);
            yield return new WaitForSeconds(0.05f);
        }

        for (int i = 0; i < wave.Creatures.Count; i++)
        {
            Creature c = wave.Creatures[i];
            c.InitializeBattle(1);

            Vector3 pos = tile.GetPositionRandom(i, wave.Creatures.Count);

            BattleEntity be = SpawnEntity(c, pos);
            be.InitializeEntity(c, 1);
            be.transform.position = pos;
            be.gameObject.SetActive(true);
            _battleManager.AddOpponentArmyEntity(be);
            yield return new WaitForSeconds(0.05f);
        }
    }

    BattleEntity SpawnEntity(Entity entity, Vector3 spawnPos)
    {
        GameObject instance = Instantiate(entity.Prefab, spawnPos, transform.localRotation);
        BattleEntity be = instance.GetComponent<BattleEntity>();
        return be;
    }
}
