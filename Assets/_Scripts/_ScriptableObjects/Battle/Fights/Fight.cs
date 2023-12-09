using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Fight : BaseScriptableObject
{
    public int Difficulty;
    public float LastWaveSpawnTime;

    public List<EnemyWave> EnemyWaves = new();
    public int CurrentWaveIndex;
    public float DelayBetweenWaves;

    public event Action OnWaveSpawned;
    public void CreateFight(int difficulty)
    {
        Difficulty = difficulty;
        DelayBetweenWaves = Random.Range(10, 20);

        CreateWaves();
    }

    void CreateWaves()
    {
        /*
            params that I can use:
            - number of minions
            - number of creatures
            - minion level range
            - creature level range
            - delay between each group spawn
        */
        // TODO: math for wave difficulty

        int numberOfWaves = 10;
        for (int i = 0; i < numberOfWaves; i++)
        {
            int numberOfMinions = 50;//2 + Mathf.FloorToInt(difficulty * i * 1.1f);
            numberOfMinions = Mathf.Clamp(numberOfMinions, 2, 50);
            Vector2Int minionLevelRange = new Vector2Int(1, Difficulty + 1);

            EnemyWave wave = CreateInstance<EnemyWave>();
            wave.CreateWave(numberOfMinions, minionLevelRange);
            EnemyWaves.Add(wave);
        }
    }

    public bool IsFinished()
    {
        return CurrentWaveIndex >= EnemyWaves.Count;
    }

    public void SpawningWaveFinished()
    {
        CurrentWaveIndex++;
        LastWaveSpawnTime = BattleManager.Instance.GetTime();
        if (CurrentWaveIndex < EnemyWaves.Count)
            CreateWaves();
        OnWaveSpawned?.Invoke();
    }

    public EnemyWave GetCurrentOpponentGroup()
    {
        return EnemyWaves[CurrentWaveIndex];
    }
}
