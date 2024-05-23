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

        public void CreateOption(int points)
        {
            // HERE: balance
            List<Unit> availableEnemies = new(GameManager.Instance.UnitDatabase.GetAllEnemies());
            GoldReward = points * Random.Range(8, 12);

            for (int i = 0; i < points; i++)
            {
                Unit u = Instantiate(availableEnemies[Random.Range(0, availableEnemies.Count)]);
                if (u is Enemy e && Random.value < 0.05f) e.SetMiniBoss();
                Army.Add(u);
            }
        }

        public void Choose()
        {
            IsChosen = true;
            OnChosen?.Invoke(this);
        }
    }
}