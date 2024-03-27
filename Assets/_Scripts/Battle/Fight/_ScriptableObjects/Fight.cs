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

        Hero _hero;
        public event Action OnWaveSpawned;

        public void CreateFight()
        {
            _hero = BattleManager.Instance.Hero;

            DelayBetweenWaves = Random.Range(10, 20);
            CreateWaves();
        }

        void CreateWaves()
        {
            const int numberOfWaves = 2;
            for (int i = 0; i < numberOfWaves; i++)
            {
                int points = 6 + EnemyWaves.Count + i;
                points = Mathf.Clamp(points, 2, 300);

                EnemyWave wave = CreateInstance<EnemyWave>();
                wave.CreateWave(EnemyWaves.Count + i, points, _hero.Level.Value);
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