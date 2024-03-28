using System;
using System.Collections.Generic;
using Lis.Core;
using Lis.Units.Hero;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Lis.Battle.Fight
{
    public class Fight : BaseScriptableObject
    {
        public List<EnemyWave> EnemyWaves = new();
        public int CurrentWaveIndex;
        public float DelayBetweenWaves;

        public int CurrentDifficulty;

        public event Action OnWaveSpawned;

        public void CreateFight()
        {
            DelayBetweenWaves = 15; // Random.Range(10, 20);
            CreateWaves();
        }

        void CreateWaves()
        {
            const int numberOfWaves = 2;
            for (int i = 0; i < numberOfWaves; i++)
            {
                // TODO: balance math minion spawning
                CurrentDifficulty = Mathf.FloorToInt(EnemyWaves.Count / 5);

                int points = 10 + EnemyWaves.Count * 2;
                points = Mathf.Clamp(points, 2, 300);

                EnemyWave wave = CreateInstance<EnemyWave>();
                wave.CreateWave(EnemyWaves.Count, points, CurrentDifficulty);
                EnemyWaves.Add(wave);
            }
        }

        public void SpawningWaveFinished()
        {
            CurrentWaveIndex++;
            if (CurrentWaveIndex < EnemyWaves.Count)
                CreateWaves();
            OnWaveSpawned?.Invoke();
        }

        public EnemyWave GetCurrentWave()
        {
            return EnemyWaves[CurrentWaveIndex];
        }
    }
}