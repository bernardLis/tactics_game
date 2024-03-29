using System;
using System.Collections.Generic;
using Lis.Core;
using Lis.Units.Hero;
using UnityEngine;

namespace Lis.Battle.Fight
{
    public class Fight : BaseScriptableObject
    {
        HeroController _heroController;

        public List<EnemyWave> EnemyWaves = new();
        public int CurrentWaveIndex;
        public float DelayBetweenWaves;

        public int CurrentDifficulty;

        public event Action OnWaveSpawned;

        public void CreateFight()
        {
            _heroController = BattleManager.Instance.HeroController;
            DelayBetweenWaves = 15;
            CreateWave();
        }

        void CreateWave()
        {
            // TODO: balance math minion spawning
            CurrentDifficulty = Mathf.FloorToInt(EnemyWaves.Count / 5);

            int points = 10 + EnemyWaves.Count * 2;
            points = Mathf.Clamp(points, 2, 300);

            EnemyWave wave = CreateInstance<EnemyWave>();
            wave.CreateWave(EnemyWaves.Count, points, CurrentDifficulty, _heroController.GetCurrentThreatLevel());
            EnemyWaves.Add(wave);
        }

        public void SpawningWaveFinished()
        {
            CurrentWaveIndex++;
            CreateWave();
            OnWaveSpawned?.Invoke();
        }

        public EnemyWave GetCurrentWave()
        {
            return EnemyWaves[CurrentWaveIndex];
        }
    }
}