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
        public List<Minion> Minions = new();
        public Unit RangedOpponent;

        /*
         * Wave should be point based
         * Minions cost 1 point, ranged opponents cost 5 points
         *
         */
        Vector2Int _minionLevelRange;

        public void CreateWave(int points, Vector2Int minionLevelRange)
        {
            _gameManager = GameManager.Instance;

            _minionLevelRange = minionLevelRange;

            int val = Random.Range(0, 100);
            if (points > 5 && val > 70)
            {
                AddRangedOpponent();
                points -= 5;
            }

            for (int i = 0; i < points; i++)
            {
                int level = Random.Range(_minionLevelRange.x, _minionLevelRange.y);
                Minion minion = Instantiate(_gameManager.UnitDatabase.GetMinionByLevel(level));
                minion.Level.SetValue(level);
                minion.InitializeBattle(1);
                Minions.Add(minion);
            }

            // single element wave
            if (val < 20)
                SingleElementWave();
            //
            //
            // // split minion count evenly between elements
            // int pointsPerElement = _pointsLeft / 4;
            // int pointsLeftover = _pointsLeft % 4;
            // CreateEnemyGroupOfElement(NatureName.Earth, pointsPerElement + pointsLeftover);
            // CreateEnemyGroupOfElement(NatureName.Fire, pointsPerElement);
            // CreateEnemyGroupOfElement(NatureName.Water, pointsPerElement);
            // CreateEnemyGroupOfElement(NatureName.Wind, pointsPerElement);
        }

        void SingleElementWave()
        {
            NatureName[] natures =
                { NatureName.Earth, NatureName.Fire, NatureName.Water, NatureName.Wind };
            int val = Random.Range(0, 4);
            NatureName n = natures[val];

            foreach (Minion m in Minions)
                m.SetNature(n);

            if (RangedOpponent == null) return;
            RangedOpponent = Instantiate(_gameManager.UnitDatabase.GetRangedOpponentByNature(n));
        }

        void AddRangedOpponent()
        {
            RangedOpponent = Instantiate(_gameManager.UnitDatabase.GetRandomRangedOpponent());
            RangedOpponent.InitializeBattle(1);
        }
    }
}