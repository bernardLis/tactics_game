using System;
using System.Collections.Generic;
using Lis.Core;
using Lis.Units;
using Lis.Units.Enemy;
using Random = UnityEngine.Random;

namespace Lis.Battle.Fight
{
    public class FightOption : BaseScriptableObject
    {
        public List<Unit> Army = new();
        public int GoldReward;

        public bool IsChosen;
        public event Action<FightOption> OnChosen;

        UnitDatabase _unitDatabase;

        public void CreateOption(int points, int fightNumber)
        {
            _unitDatabase = GameManager.Instance.UnitDatabase;

            GoldReward = (fightNumber + 1) * Random.Range(8, 12);
            List<Enemy> availableEnemies = new(_unitDatabase.GetAllEnemies());
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

            // limits enemies scariness to fight number
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
            for (int i = 0; i < Random.Range(5, 10); i++)
            {
                AddEnemyToArmy(_unitDatabase.GetEnemyByName("Mushroom"));
            }
        }

        void AddEnemyToArmy(Enemy enemy)
        {
            Enemy newEnemy = Instantiate(enemy);
            newEnemy.InitializeBattle(1);
            Army.Add(newEnemy);
        }

        public void Choose()
        {
            IsChosen = true;
            OnChosen?.Invoke(this);
        }
    }
}