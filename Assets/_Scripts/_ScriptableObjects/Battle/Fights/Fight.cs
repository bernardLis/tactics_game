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
        /*
        params that I can use:
        - number of minions
        - number of creatures
        - minion level range
        - creature level range
        - delay between each group spawn
        */
        // TODO: math for wave difficulty

        DelayBetweenWaves = Random.Range(10, 20);
        int numberOfWaves = Random.Range(1, 2);
        for (int i = 0; i < numberOfWaves; i++)
        {
            int numberOfMinions = 25;//2 + Mathf.FloorToInt(difficulty * i * 1.1f);
            numberOfMinions = Mathf.Clamp(numberOfMinions, 2, 50);
            Vector2Int minionLevelRange = new Vector2Int(1, difficulty + 1);

            EnemyWave wave = CreateInstance<EnemyWave>();
            wave.CreateWave(numberOfMinions, minionLevelRange);
            EnemyWaves.Add(wave);
        }
    }

    public bool IsFinished()
    {
        return CurrentWaveIndex >= EnemyWaves.Count;
    }

    int GetNumberOfCreatures(int i, int numberOfGroups, int difficulty)
    {
        if (i != numberOfGroups - 1) return 0; // only last group has a creature

        if (difficulty == 1) return 0;

        if (difficulty == 2 || difficulty == 3 || difficulty == 4 || difficulty == 5) return 1;
        return 2;
    }

    public void SpawningGroupFinished()
    {
        CurrentWaveIndex++;
        LastWaveSpawnTime = BattleManager.Instance.GetTime();
        OnWaveSpawned?.Invoke();
    }

    public EnemyWave GetCurrentOpponentGroup()
    {
        return EnemyWaves[CurrentWaveIndex];
    }
}
