using System.Collections.Generic;
using Lis.Core;
using Lis.Units.Creature;
using UnityEngine;

namespace Lis.Battle.Fight
{
    public class Fight : BaseScriptableObject
    {
        [HideInInspector] public List<Creature> OpponentArmy = new();

        public void Initialize(int points)
        {
            // TODO: for now
            List<Creature> availableCreatures = new(GameManager.Instance.UnitDatabase.GetAllOpponentCreatures());
            for (int i = 0; i < points; i++)
                OpponentArmy.Add(Instantiate(availableCreatures[Random.Range(0, availableCreatures.Count)]));
        }
        /*
         *         public void CreateWave(int waveIndex, int points, int minionLevel, int threatLevel)
        {
            _gameManager = GameManager.Instance;

            WaveIndex = waveIndex;
            Points = points;
            MinionLevel = minionLevel;

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
                minion.Level.SetValue(minionLevel + threatLevel);
                minion.SetRandomNature();
                minion.InitializeBattle(1);
                Minions.Add(minion);
            }

            // single element wave
            if (val < 10) SingleElementWave();

            // mini boss
            if (waveIndex > 1 && waveIndex % 5 == 0)
                AddMiniBoss();
        }

        void AddRangedOpponent()
        {
            RangedOpponent = Instantiate(_gameManager.UnitDatabase.GetRandomRangedOpponent());
            RangedOpponent.InitializeBattle(1);
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

        void AddMiniBoss()
        {
            List<Minion> minions = _gameManager.UnitDatabase.GetAllMinions();
            minions = minions.OrderBy(m => m.LevelRange.x).ToList();
            int index = Mathf.FloorToInt(MinionLevel - 1);
            if (index >= minions.Count) index = minions.Count - 1;
            Minion miniBoss = Instantiate(minions[index]);

            miniBoss.Level.SetValue(MinionLevel);
            miniBoss.SetRandomNature();
            miniBoss.InitializeBattle(1);
            miniBoss.SetMiniBoss();
            Minions.Add(miniBoss);
        }

         */
    }
}