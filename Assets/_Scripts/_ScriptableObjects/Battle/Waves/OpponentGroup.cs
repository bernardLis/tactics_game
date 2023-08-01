using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpponentGroup : BaseScriptableObject
{
    public List<Minion> Minions = new();
    public List<Creature> Creatures = new();

    public void CreateGroup(Element element, int minions, Vector2Int minionLevelRange, int creatures, Vector2Int creatureLevelRange)
    {
        GameManager _gameManager = GameManager.Instance;
        List<Minion> minionList = new(_gameManager.HeroDatabase.GetAllMinionsByElement(element));
        List<Creature> creatureList = new(_gameManager.HeroDatabase.GetCreaturesByTierElement(0, element));

        for (int i = 0; i < minions; i++)
        {
            Minion minion = Instantiate(minionList[Random.Range(0, minionList.Count)]);
            minion.InitializeMinion(Random.Range(minionLevelRange.x, minionLevelRange.y));
            Minions.Add(minion);
        }

        for (int i = 0; i < creatures; i++)
        {
            Creature creature = Instantiate(creatureList[Random.Range(0, creatureList.Count)]);
            creature.Level = Random.Range(creatureLevelRange.x, creatureLevelRange.y);
            Creatures.Add(creature);
        }
    }
}
