using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using DG.Tweening;
using System;
using Random = UnityEngine.Random;
using System.Threading;

public class BattleWaveManager : Singleton<BattleWaveManager>
{
    GameManager _gameManager;
    BattleManager _battleManager;

    public int CurrentDifficulty { get; private set; }
    public int CurrentWaveIndex { get; private set; }

    BattleLandTile _currentTile;

    public List<BattleWave> Waves = new();
    BattleWave _currentWave;

    void Start()
    {
        _gameManager = GameManager.Instance;
        _battleManager = BattleManager.Instance;
        _battleManager.OnOpponentEntityDeath += OnOpponentEntityDeath;

        CurrentDifficulty = 1;
    }

    public void InitializeWaves(BattleLandTile tile)
    {
        _currentTile = tile;
        StartCoroutine(TileFightCoroutine());

    }

    IEnumerator TileFightCoroutine()
    {
        CurrentWaveIndex = 0;
        CreateWaves();
        yield return StartWave();

        // spawn opponent groups on tile edge
        // on wave end
        CurrentDifficulty++;
    }

    void CreateWaves()
    {
        int wavesCount = 1;
        for (int i = 0; i < wavesCount; i++)
        {
            BattleWave wave = ScriptableObject.CreateInstance<BattleWave>();
            wave.CreateWave(CurrentDifficulty);
            Waves.Add(wave);
        }
    }

    IEnumerator Countdown(int seconds)
    {
        for (int i = seconds; i > 0; i--)
        {
            Debug.Log(i);
            yield return new WaitForSeconds(1);
        }
    }

    IEnumerator StartWave()
    {
        yield return Countdown(3);

        Debug.Log($"starting wave {CurrentWaveIndex}");

        _currentWave = Waves[CurrentWaveIndex];
        foreach (OpponentGroup g in _currentWave.OpponentGroups)
        {
            Debug.Log($"spawning group {g}");

            StartCoroutine(SpawnOpponentGroup(g));
            yield return new WaitForSeconds(_currentWave.DelayBetweenGroups);
        }

    }

    void OnOpponentEntityDeath(BattleEntity _)
    {
        if (_battleManager.OpponentEntities.Count != 0) return;
        if (!_currentWave.IsFinished()) return;

        if (CurrentWaveIndex < Waves.Count - 1)
        {
            CurrentWaveIndex++;
            StartCoroutine(StartWave());
        }
        else
        {
            Debug.Log("Tile secured");
            _currentTile.Secured();
        }

    }

    IEnumerator SpawnOpponentGroup(OpponentGroup group)
    {
        yield return SpawnMinions(group);
        _currentWave.SpawningGroupFinished();
    }

    IEnumerator SpawnMinions(OpponentGroup group)
    {
        Debug.Log($"spawning minions {group.Minions.Count}");

        float theta = 0;
        float thetaStep = 2 * Mathf.PI / group.Minions.Count;
        for (int i = 0; i < group.Minions.Count; i++)
        {
            Minion m = group.Minions[i];
            m.InitializeBattle(1);

            float radius = _currentTile.transform.localScale.x * 5 - 2; // magic 5
            Vector3 center = _currentTile.transform.position;
            float x = Mathf.Cos(theta) * radius + center.x;
            float y = 1f;
            float z = Mathf.Sin(theta) * radius + center.z;
            Vector3 pos = new(x, y, z);
            //pos (-4.00, 1.00, 0.00)
            // want to spawn at 20
            Debug.Log($"theta {theta}");
            Debug.Log($"pos {pos}");

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
