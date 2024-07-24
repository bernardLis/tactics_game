using System.Collections.Generic;
using Lis.Core;
using Lis.Units;
using Lis.Units.Enemy;
using Lis.Units.Pawn;
using Lis.Units.Peasant;
using UnityEngine;

namespace Lis.Other
{
    public class TestFight : BaseScriptableObject
    {
        public List<Unit> PlayerArmy = new();
        public List<Unit> EnemyArmy = new();

        public int Points;

        public int FightDuration;
        public bool PlayerWon;

        public List<Unit> Survivors;

        UnitDatabase _unitDatabase;

        public void CreateTestFight(int points)
        {
            _unitDatabase = GameManager.Instance.UnitDatabase;

            Points = points;
            CreatePlayerArmy(points);
            CreateEnemyArmy(points);
        }

        void CreatePlayerArmy(int points)
        {
            Peasant peasant = _unitDatabase.Peasant;
            var pawns = _unitDatabase.GetAllPawns();

            int pointsLeft = points;
            int tries = 0;
            while (pointsLeft > 0)
            {
                tries++;
                if (tries > 100)
                    break;
                // I got 1 peasant and 12 pawns, so 13 different units to choose from
                // pawns are split into 4 groups, each group has 3 upgrades
                int random = Random.Range(0, 13);
                if (random == 0)
                {
                    Peasant instance = Instantiate(peasant);
                    instance.InitializeFight(0);
                    pointsLeft -= 100;
                    PlayerArmy.Add(instance);
                    continue;
                }

                Pawn pawn = pawns[Random.Range(0, pawns.Count)];
                int upgradeIndex = Random.Range(0, 3);
                PawnUpgrade upgrade = pawn.Upgrades[upgradeIndex];
                if (upgrade.Price > pointsLeft)
                    continue;

                pointsLeft -= upgrade.Price;
                Pawn newPawn = Instantiate(pawn);
                newPawn.InitializeFight(0);
                newPawn.SetUpgrade(upgradeIndex);
                PlayerArmy.Add(newPawn);
            }
        }

        void CreateEnemyArmy(int points)
        {
            var enemies = _unitDatabase.GetAllEnemies();
            int pointsLeft = points;
            int tries = 0;
            while (pointsLeft > 0)
            {
                Enemy enemy = enemies[Random.Range(0, enemies.Count)];
                tries++;
                if (tries > 100)
                    break;
                if (enemy.Price > pointsLeft)
                    continue;

                pointsLeft -= enemy.Price;
                Enemy newEnemy = Instantiate(enemy);
                newEnemy.InitializeFight(1);
                EnemyArmy.Add(newEnemy);
            }
        }

        public void SetSurvivors(List<Unit> survivors)
        {
            Survivors = new(survivors);
        }
    }
}