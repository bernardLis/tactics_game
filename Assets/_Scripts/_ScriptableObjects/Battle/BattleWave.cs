using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleWave : BaseScriptableObject
{
    public int NumberOfEnemies;
    public Vector2Int EnemyLevelRange;

    public List<Creature> Minions = new();

    public void Initialize()
    {
        for (int i = 0; i < NumberOfEnemies; i++)
        {
            Creature minion = Instantiate(GameManager.Instance.HeroDatabase.GetRandomMinion());
            minion.InitializeMinion(Random.Range(EnemyLevelRange.x, EnemyLevelRange.y));
            Minions.Add(minion);
        }
    }

    public List<Creature> GetAllCreaturesByElement(Element element)
    {
        List<Creature> minions = new();
        foreach (Creature minion in Minions)
            if (minion.Element == element)
                minions.Add(minion);
        return minions;
    }
}
