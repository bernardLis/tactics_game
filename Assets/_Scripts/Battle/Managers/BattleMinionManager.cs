using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using DG.Tweening;
using System;
using Random = UnityEngine.Random;

public class BattleMinionManager : Singleton<BattleMinionManager>
{
    [SerializeField] bool _noFights; // HERE: testing

    BattleManager _battleManager;
    BattleTooltipManager _battleTooltipManager;
    BattleAreaManager _battleAreaManager;

    public int CurrentDifficulty { get; private set; }

    Fight _currentFight;

    /* POOLS */
    [SerializeField] GameObject _projectilePrefab;
    [SerializeField] Transform _enemyProjectilePoolHolder;
    public List<BattleProjectileOpponent> Projectiles = new();

    [SerializeField] GameObject _minionPrefab;
    [SerializeField] Transform _minionPoolHolder;
    public List<BattleEntity> Minions = new();


    void Start()
    {
        _battleManager = BattleManager.Instance;
        _battleManager.OnOpponentEntityDeath += OnOpponentEntityDeath;
        _battleTooltipManager = BattleTooltipManager.Instance;
        _battleAreaManager = _battleManager.GetComponent<BattleAreaManager>();

        CurrentDifficulty = 1;

        CreateProjectilePool();
        CreateMinionPool();

    }

    public void Initialize()
    {
        CreateFight();
        StartCoroutine(StartFight());
    }

    void CreateProjectilePool()
    {
        Projectiles = new();
        for (int i = 0; i < 200; i++)
        {
            BattleProjectileOpponent p = Instantiate(_projectilePrefab, _enemyProjectilePoolHolder).GetComponent<BattleProjectileOpponent>();
            p.gameObject.SetActive(false);
            Projectiles.Add(p);
        }
    }

    void CreateMinionPool()
    {
        Minions = new();
        for (int i = 0; i < 200; i++)
        {
            BattleEntity be = Instantiate(_minionPrefab, _minionPoolHolder).GetComponent<BattleEntity>();
            be.gameObject.SetActive(false);
            Minions.Add(be);
        }
    }

    void CreateFight()
    {
        Fight fight = ScriptableObject.CreateInstance<Fight>();
        fight.CreateFight(CurrentDifficulty);
        _currentFight = fight;
    }

    IEnumerator StartFight()
    {
        // HERE: testing
        if (_noFights)
        {
            yield break;
        }

        yield return new WaitForSeconds(2f);

        while (true)
        {
            EnemyWave wave = _currentFight.EnemyWaves[_currentFight.CurrentWaveIndex];
            StartCoroutine(SpawnWave(wave));
            yield return new WaitForSeconds(_currentFight.DelayBetweenWaves);
        }
    }

    void OnOpponentEntityDeath(BattleEntity _)
    {
        if (_battleManager.OpponentEntities.Count != 0) return;
        if (!_currentFight.IsFinished()) return;
    }

    IEnumerator SpawnWave(EnemyWave wave)
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
            BattleEntity be = Minions.Find(x => !x.gameObject.activeSelf);
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
