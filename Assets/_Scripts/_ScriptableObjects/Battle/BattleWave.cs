using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleWave : BaseScriptableObject
{
    public int NumberOfEnemies;
    public Vector2Int EnemyLevelRange;

    public List<Minion> Minions = new();

    public void Initialize()
    {
        for (int i = 0; i < NumberOfEnemies; i++)
        {
            Minion minion = Instantiate(GameManager.Instance.HeroDatabase.GetRandomMinion());
            minion.InitializeMinion(Random.Range(EnemyLevelRange.x, EnemyLevelRange.y));
            Minions.Add(minion);
        }
    }

    public List<Minion> GetAllMinionsByElement(Element element)
    {
        List<Minion> minions = new();
        foreach (Minion minion in Minions)
            if (minion.Element == element)
                minions.Add(minion);
        return minions;
    }

    public int GetNumberOfMinionsByName(string name)
    {
        int count = 0;
        foreach (Minion minion in Minions)
            if (minion.Name == name)
                count++;
        return count;
    }
}
