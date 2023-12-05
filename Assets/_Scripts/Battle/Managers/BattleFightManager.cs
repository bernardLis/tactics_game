using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using DG.Tweening;
using System;
using Random = UnityEngine.Random;

public class BattleFightManager : Singleton<BattleFightManager>
{
    [SerializeField] bool _noFights; // HERE: testing

    BattleManager _battleManager;
    BattleTooltipManager _battleTooltipManager;
    BattleAreaManager _battleAreaManager;

    public int CurrentDifficulty { get; private set; }
    public bool IsFightActive { get; private set; }
    BattleTile _currentTile;

    public List<Fight> Fights = new();
    Fight _currentFight;

    /* POOLS */
    [SerializeField] GameObject _projectilePrefab;
    [SerializeField] Transform _enemyProjectilePoolHolder;
    public List<BattleProjectileOpponent> Projectiles = new();

    [SerializeField] GameObject _minionPrefab;
    [SerializeField] Transform _minionPoolHolder;
    public List<BattleEntity> Minions = new();

    public event Action OnFightStarted;
    public event Action OnFightEnded;
    public event Action OnWaveSpawned;
    public event Action OnBossFightStarted;
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


    public void InitializeFight(BattleTile tile)
    {
        _currentTile = tile;
        if (_battleManager.IsBossFight())
        {
            StartCoroutine(BossFightCoroutine());
            return;
        }
        StartCoroutine(TileFightCoroutine());
    }

    IEnumerator BossFightCoroutine()
    {
        _battleTooltipManager.ShowGameInfo($"Boss fight!", 1.5f);
        yield return new WaitForSeconds(1.5f);

        IsFightActive = true;
        OnBossFightStarted?.Invoke();

        StartCoroutine(SpawnMinionsOnBossTile());
    }

    IEnumerator SpawnMinionsOnBossTile()
    {
        // last purchased tile
        BattleTile tile = _battleAreaManager.UnlockedTiles[_battleAreaManager.UnlockedTiles.Count - 1];
        while (IsFightActive)
        {
            // HERE: boss testing
            int numberOfMinions = 25;//2 + Mathf.FloorToInt(difficulty * i * 1.1f);
            numberOfMinions = Mathf.Clamp(numberOfMinions, 2, 50);
            Vector2Int minionLevelRange = new Vector2Int(1, 1);

            EnemyWave wave = ScriptableObject.CreateInstance<EnemyWave>();
            wave.CreateWave(numberOfMinions, minionLevelRange);

            yield return SpawnMinions(wave, tile);
            yield return new WaitForSeconds(Random.Range(20, 40));
        }
    }

    IEnumerator TileFightCoroutine()
    {
        // HERE: testing
        if (_noFights)
        {
            yield return new WaitForSeconds(1f);
            OnFightEnded?.Invoke();
            yield break;
        }

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
        _battleTooltipManager.ShowGameInfo($"Wave {_currentFight.CurrentWaveIndex + 1}/{_currentFight.EnemyWaves.Count}", 1.5f);
        yield return new WaitForSeconds(1.5f);

        IsFightActive = true;
        OnFightStarted?.Invoke();

        foreach (EnemyWave wave in _currentFight.EnemyWaves)
        {
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
        yield return SpawnMinions(group, _currentTile);
        _currentFight.SpawningGroupFinished();
    }

    public IEnumerator SpawnMinions(EnemyWave group, BattleTile tile)
    {
        for (int i = 0; i < group.Minions.Count; i++)
        {
            Minion m = group.Minions[i];
            m.InitializeBattle(1);

            Vector3 pos = tile.GetMinionPosition(i, group.Minions.Count);

            // BattleEntity be = SpawnEntity(m, pos);
            BattleEntity be = Minions.Find(x => !x.gameObject.activeSelf);
            be.InitializeEntity(m, 1);
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
