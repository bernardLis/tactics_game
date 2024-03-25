using System.Collections.Generic;
using Lis.Core;
using Lis.Units;
using Lis.Units.Minion;
using UnityEngine;

namespace Lis.Battle.Fight
{
    public class EnemyWave : BaseScriptableObject
    {
        GameManager _gameManager;

        public List<EnemyGroup> EnemyGroups = new();

        public Unit RangedOpponent;

        /*
         * Wave should be point based
         * Minions cost 1 point, ranged opponents cost 5 points
         *
         */
        int _pointsLeft;
        Vector2Int _minionLevelRange;

        public void CreateWave(int points, Vector2Int minionLevelRange)
        {
            _gameManager = GameManager.Instance;

            _pointsLeft = points;
            _minionLevelRange = minionLevelRange;

            int val = Random.Range(0, 100);
            if (val < 20)
            {
                SingleElementWave();
                return;
            }

            if (points > 5 && val > 70)
                AddRangedOpponent();

            // split minion count evenly between elements
            int pointsPerElement = _pointsLeft / 4;
            int pointsLeftover = _pointsLeft % 4;
            CreateEnemyGroupOfElement(NatureName.Earth, pointsPerElement + pointsLeftover);
            CreateEnemyGroupOfElement(NatureName.Fire, pointsPerElement);
            CreateEnemyGroupOfElement(NatureName.Water, pointsPerElement);
            CreateEnemyGroupOfElement(NatureName.Wind, pointsPerElement);
        }

        void SingleElementWave()
        {
            int val = Random.Range(0, 4);
            switch (val)
            {
                case 0:
                    CreateEnemyGroupOfElement(NatureName.Earth, _pointsLeft);
                    break;
                case 1:
                    CreateEnemyGroupOfElement(NatureName.Fire, _pointsLeft);
                    break;
                case 2:
                    CreateEnemyGroupOfElement(NatureName.Water, _pointsLeft);
                    break;
                default:
                    CreateEnemyGroupOfElement(NatureName.Wind, _pointsLeft);
                    break;
            }
        }

        void AddRangedOpponent()
        {
            RangedOpponent = Instantiate(_gameManager.UnitDatabase.GetRandomRangedOpponent());
            RangedOpponent.InitializeBattle(1);
            _pointsLeft -= 5;
        }

        void CreateEnemyGroupOfElement(NatureName natureName, int points)
        {
            EnemyGroup group = CreateInstance<EnemyGroup>();
            group.NatureName = natureName;
            for (int i = 0; i < points; i++)
            {
                Minion minion = Instantiate(_gameManager.UnitDatabase.GetRandomMinionByElement(natureName));
                minion.Level.SetValue(Random.Range(_minionLevelRange.x, _minionLevelRange.y));
                minion.InitializeBattle(1);
                group.Minions.Add(minion);
            }

            EnemyGroups.Add(group);
        }
    }
}