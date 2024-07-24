using System.Collections.Generic;
using Lis.Core;
using Lis.Units.Enemy;
using Lis.Units.Hero;
using Random = UnityEngine.Random;

namespace Lis.Arena.Fight
{
    public class Wave : BaseScriptableObject
    {
        public Dictionary<string, int> Army = new();
        public int ActiveLockerRoomCount;

        UnitDatabase _unitDatabase;

        public void Initialize(int points, Hero hero)
        {
            _unitDatabase = GameManager.Instance.UnitDatabase;

            ActiveLockerRoomCount = Random.Range(1, 5);
            // limits enemies scariness to fight number
            List<Enemy> availableEnemies = new(_unitDatabase.GetAllEnemies());
            availableEnemies.RemoveAll(e => e.ScarinessRank > hero.Level.Value);

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

        void AddEnemyToArmy(Enemy enemy)
        {
            if (!Army.TryAdd(enemy.Id, 1))
                Army[enemy.Id]++;
        }
    }
}