using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWave : BaseScriptableObject
{
    public Sprite Icon;
    public Element Element;

    public List<Minion> Minions = new();
    public List<Creature> Creatures = new();

    public void CreateWave(int minions, Vector2Int minionLevelRange)
    {
        GameManager _gameManager = GameManager.Instance;

        List<Minion> minionList = new(_gameManager.EntityDatabase.GetAllMinions());

        for (int i = 0; i < minions; i++)
        {
            Minion minion = Instantiate(minionList[Random.Range(0, minionList.Count)]);
            minion.Level.SetValue(Random.Range(minionLevelRange.x, minionLevelRange.y));
            minion.InitializeBattle(1);
            Minions.Add(minion);
        }
    }
}
