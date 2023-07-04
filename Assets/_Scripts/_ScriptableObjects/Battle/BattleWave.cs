using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleWave : BaseScriptableObject
{
    public int NumberOfEnemies;
    public Vector2Int EnemyLevelRange;

    public List<Creature> Creatures = new();

    public void Initialize()
    {
        for (int i = 0; i < NumberOfEnemies; i++)
        {
            Creature creature = Instantiate(GameManager.Instance.HeroDatabase.MeleeEnemy);
            creature.RandomizedEnemy(Random.Range(EnemyLevelRange.x, EnemyLevelRange.y));
            Creatures.Add(creature);
        }
    }

    public List<Creature> GetAllCreaturesByElement(Element element)
    {
        List<Creature> creatures = new();
        foreach (Creature creature in Creatures)
            if (creature.Element == element)
                creatures.Add(creature);
        return creatures;
    }
}
