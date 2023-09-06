using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpponentGroup : BaseScriptableObject
{
    public Sprite Icon;
    public Element Element;

    public List<Minion> Minions = new();
    public List<Creature> Creatures = new();

    public void CreateGroup(Element element,
            int minions, Vector2Int minionLevelRange,
            int creatures, Vector2Int creatureLevelRange)
    {
        GameManager _gameManager = GameManager.Instance;

        Element = element;
        Icon = _gameManager.GameDatabase.GetOpponentGroupIcon(element, creatures > 0);

        List<Minion> minionList = new(_gameManager.HeroDatabase.GetAllMinionsByElement(element));
        List<Creature> creatureList = new(_gameManager.HeroDatabase.GetCreaturesByTierElement(0, element));

        for (int i = 0; i < minions; i++)
        {
            Minion minion = Instantiate(minionList[Random.Range(0, minionList.Count)]);
            minion.Level.SetValue(Random.Range(minionLevelRange.x, minionLevelRange.y));
            minion.InitializeBattle(1);
            Minions.Add(minion);
        }

        for (int i = 0; i < creatures; i++)
        {
            Creature creature = Instantiate(creatureList[Random.Range(0, creatureList.Count)]);
            creature.Level.SetValue(Random.Range(creatureLevelRange.x, creatureLevelRange.y));
            Creatures.Add(creature);
        }

    }
}
