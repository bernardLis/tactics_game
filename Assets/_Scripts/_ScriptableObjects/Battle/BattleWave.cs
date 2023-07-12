using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleWave : BaseScriptableObject
{
    GameManager _gameManager;

    public int Difficulty;

    // TODO: math for wave difficulty
    public int NumberOfMinions;
    public Vector2Int MinionLevelRange;
    public int NumberOfCreatures;
    public Vector2Int CreatureLevelRange;

    public List<Minion> Minions = new();
    public List<Creature> Creatures = new();


    public void CreateWave(int difficulty)
    {
        _gameManager = GameManager.Instance;

        Difficulty = difficulty;

        NumberOfMinions = Random.Range(10, 25);
        MinionLevelRange = new Vector2Int(1, 5);

        // TODO: math for wave difficulty
        CreatureLevelRange = new Vector2Int(1, 3);
        if (difficulty > 11)
            NumberOfCreatures = Random.Range(1, 5);
        if (difficulty >= 3 && difficulty <= 11)
        {
            if (difficulty % 3 == 0)
                NumberOfCreatures = 1;
            if (difficulty % 3 == 1)
                NumberOfCreatures = 2;
            if (difficulty % 3 == 2)
                NumberOfCreatures = 3;
        }

        Initialize();
    }

    void Initialize()
    {
        for (int i = 0; i < NumberOfMinions; i++)
        {
            Minion minion = Instantiate(GameManager.Instance.HeroDatabase.GetRandomMinion());
            minion.InitializeMinion(Random.Range(MinionLevelRange.x, MinionLevelRange.y));
            Minions.Add(minion);
        }

        for (int i = 0; i < NumberOfCreatures; i++)
        {
            Creature c = Instantiate(GameManager.Instance.HeroDatabase.GetRandomCreature());
            // difficulty 3,4,5 -> easy element
            if (Difficulty >= 3 && Difficulty <= 5)
            {
                Element el = _gameManager.PlayerHero.Element.StrongAgainst;
                List<Creature> all = new(_gameManager.HeroDatabase.GetStartingArmy(el).Creatures);
                c = Instantiate(all[Random.Range(0, all.Count)]);
            }
            // difficulty 6,7,8 -> medium element
            if (Difficulty >= 6 && Difficulty <= 8)
            {
                List<Element> elements = new(_gameManager.HeroDatabase.GetAllElements());
                elements.Remove(_gameManager.PlayerHero.Element);
                elements.Remove(_gameManager.PlayerHero.Element.StrongAgainst);
                elements.Remove(_gameManager.PlayerHero.Element.WeakAgainst);

                Element el = elements[0];
                List<Creature> all = new(_gameManager.HeroDatabase.GetStartingArmy(el).Creatures);
                c = Instantiate(all[Random.Range(0, all.Count)]);
            }

            // difficulty 9,10,11 -> hard element
            if (Difficulty >= 9 && Difficulty <= 11)
            {
                Element el = _gameManager.PlayerHero.Element.WeakAgainst;
                List<Creature> all = new(_gameManager.HeroDatabase.GetStartingArmy(el).Creatures);
                c = Instantiate(all[Random.Range(0, all.Count)]);
            }

            c.Level = Random.Range(CreatureLevelRange.x, CreatureLevelRange.y);
            Creatures.Add(c);
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
