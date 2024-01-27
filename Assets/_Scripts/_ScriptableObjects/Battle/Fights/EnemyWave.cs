using System.Collections.Generic;
using UnityEngine;

namespace Lis
{
    public class EnemyWave : BaseScriptableObject
    {
        GameManager _gameManager;

        public List<EnemyGroup> EnemyGroups = new();

        public void CreateWave(int minionCount, Vector2Int minionLevelRange)
        {
            _gameManager = GameManager.Instance;

            int val = Random.Range(0, 100);
            switch (val)
            {
                case < 5:
                    CreateEnemyGroupOfElement(ElementName.Earth, minionCount, minionLevelRange);
                    return;
                case >= 5 and < 10:
                    CreateEnemyGroupOfElement(ElementName.Fire, minionCount, minionLevelRange);
                    return;
                case >= 10 and < 15:
                    CreateEnemyGroupOfElement(ElementName.Water, minionCount, minionLevelRange);
                    return;
                case >= 15 and < 20:
                    CreateEnemyGroupOfElement(ElementName.Wind, minionCount, minionLevelRange);
                    return;
            }

            // split minion count evenly between elements
            int minionsPerElement = minionCount / 4;
            int minionsLeft = minionCount % 4;
            CreateEnemyGroupOfElement(ElementName.Earth, minionsPerElement + minionsLeft, minionLevelRange);
            CreateEnemyGroupOfElement(ElementName.Fire, minionsPerElement, minionLevelRange);
            CreateEnemyGroupOfElement(ElementName.Water, minionsPerElement, minionLevelRange);
            CreateEnemyGroupOfElement(ElementName.Wind, minionsPerElement, minionLevelRange);
        }

        void CreateEnemyGroupOfElement(ElementName elementName, int minionCount, Vector2Int minionLevelRange)
        {
            EnemyGroup group = CreateInstance<EnemyGroup>();
            group.ElementName = elementName;
            for (int i = 0; i < minionCount; i++)
            {
                Minion minion = Instantiate(_gameManager.EntityDatabase.GetRandomMinionByElement(elementName));
                minion.Level.SetValue(Random.Range(minionLevelRange.x, minionLevelRange.y + 1));
                minion.InitializeBattle(1);
                group.Minions.Add(minion);
            }

            EnemyGroups.Add(group);
        }


        // void AddRangedOpponent()
        // {
        //     Creature rangedOpponent = Instantiate(_gameManager.EntityDatabase.RangedOpponent);
        //     rangedOpponent.InitializeBattle(1);
        //     Creatures.Add(rangedOpponent);
        // }
    }
}