using System;
using System.Collections.Generic;
using Lis.Core;
using Lis.Units.Enemy;
using Random = UnityEngine.Random;

namespace Lis.Arena.Fight
{
    public class FightOption : BaseScriptableObject
    {
        public Dictionary<string, int> ArmyPerWave = new();
        public int NumberOfWaves;
        public int GoldReward;

        public bool IsChosen;

        UnitDatabase _unitDatabase;
        public event Action<FightOption> OnChosen;

        public void CreateOption(int points, int fightNumber)
        {
            _unitDatabase = GameManager.Instance.UnitDatabase;

            GoldReward = (fightNumber + 1) * Random.Range(8, 12);
            if (fightNumber == 0)
            {
                CreateFirstFight();
                return;
            }

            if (fightNumber == 1)
            {
                CreateSecondFight();
                return;
            }

            NumberOfWaves = Random.Range(1, 4);
            points /= NumberOfWaves;

            // limits enemies scariness to fight number
            List<Enemy> availableEnemies = new(_unitDatabase.GetAllEnemies());
            availableEnemies.RemoveAll(e => e.ScarinessRank > fightNumber - 1);

            int pointsLeft = points;
            int tries = 0;
            while (pointsLeft > 0)
            {
                Enemy enemy = availableEnemies[Random.Range(0, availableEnemies.Count)];
                tries++;
                if (tries > 100) break;
                if (enemy.Price > pointsLeft) continue;

                pointsLeft -= enemy.Price;
                AddEnemyToArmy(enemy);
            }
        }

        void CreateFirstFight()
        {
            NumberOfWaves = 1;
            int val = Random.Range(0, 2);
            if (val == 0)
            {
                AddEnemyToArmy(_unitDatabase.GetEnemyByName("Wildboar"));
                AddEnemyToArmy(_unitDatabase.GetEnemyByName("Wildboar"));
            }
            else
            {
                AddEnemyToArmy(_unitDatabase.GetEnemyByName("Stump"));
            }
        }

        void CreateSecondFight()
        {
            NumberOfWaves = 1;
            for (int i = 0; i < Random.Range(5, 10); i++) AddEnemyToArmy(_unitDatabase.GetEnemyByName("Mushroom"));
        }

        void AddEnemyToArmy(Enemy enemy)
        {
            if (!ArmyPerWave.TryAdd(enemy.Id, 1))
                ArmyPerWave[enemy.Id]++;
        }

        public void Choose()
        {
            IsChosen = true;
            OnChosen?.Invoke(this);
        }
    }
}