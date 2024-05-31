using System;
using System.Collections.Generic;
using Lis.Core;
using Lis.Units;
using Lis.Units.Enemy;
using UnityEngine;
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

            int pointsLeft = points;
            int tries = 0;
            while (pointsLeft > 0)
            {
                Enemy enemy = availableEnemies[Random.Range(0, availableEnemies.Count)];
                tries++;
                if (tries > 100) break;
                if (enemy.Price > pointsLeft) continue;

                pointsLeft -= enemy.Price;
                Enemy newEnemy = Instantiate(enemy);
                newEnemy.InitializeBattle(1);
                Army.Add(newEnemy);
            }
        }

        void CreateFirstFight()
        {
            int val = Random.Range(0, 2);
            if (val == 0)
            {
                Army.Add(_unitDatabase.GetEnemyByName("Boar"));
                Army.Add(_unitDatabase.GetEnemyByName("Boar"));
            }
            else
            {
                Army.Add(_unitDatabase.GetEnemyByName("Stump"));
            }
        }

        public void Choose()
        {
            IsChosen = true;
            OnChosen?.Invoke(this);
        }
    }
}