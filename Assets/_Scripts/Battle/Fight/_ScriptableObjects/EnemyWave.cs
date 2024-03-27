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
        public void CreateWave(int waveIndex, int points, int minionLevel)
        {
            _gameManager = GameManager.Instance;

            int val = Random.Range(0, 100);
            if (points > 10 && val > 70)
            {
                AddRangedOpponent();
                points -= 5;
            }

            List<Minion> availableMinions = new(_gameManager.UnitDatabase.GetMinionsByLevelRange(minionLevel));
            for (int i = 0; i < points; i++)
            {
                Minion minion = Instantiate(availableMinions[Random.Range(0, availableMinions.Count)]);
                minion.Level.SetValue(minionLevel);
                minion.InitializeBattle(1);
                Minions.Add(minion);
            }

            // single element wave
            if (val < 10) SingleElementWave();

            // mini boss
            if (waveIndex > 1 && waveIndex % 5 == 0)
                Minions[Random.Range(0, Minions.Count)].SetMiniBoss();
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