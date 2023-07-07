using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Map/Battle")]
public class Battle : BaseScriptableObject
{
    public BattleType BattleType;
    public Hero Opponent;
    public List<BattleWave> Waves = new();

    public bool IsObstacleActive;

    public bool Won;

    public void CreateRandomDuel(int level)
    {
        BattleType = BattleType.Duel;

        GameManager gameManager = GameManager.Instance;

        Opponent = ScriptableObject.CreateInstance<Hero>();
        Opponent.CreateRandom(gameManager.PlayerHero.Level.Value);

        if (gameManager.BattleNumber == 1)
        {
            Opponent.Army.Clear();
            // get starting army of neutral element
            List<Element> elements = new(gameManager.HeroDatabase.GetAllElements());
            elements.Remove(gameManager.PlayerHero.Element);
            elements.Remove(gameManager.PlayerHero.Element.StrongAgainst);
            elements.Remove(gameManager.PlayerHero.Element.WeakAgainst);
            Opponent.Army = new(gameManager.HeroDatabase.GetStartingArmy(elements[0]).Creatures);
        }
        if (gameManager.BattleNumber == 2)
        {
            // get starting army of element our here is weak to
            Opponent.Army = new(gameManager.HeroDatabase.GetStartingArmy(gameManager.PlayerHero.Element.WeakAgainst).Creatures);
        }
    }

    public void CreateRandomWaves(int level)
    {
        BattleType = BattleType.Waves;

        GameManager gameManager = GameManager.Instance;

        Opponent = ScriptableObject.CreateInstance<Hero>();
        Opponent.CreateRandom(gameManager.PlayerHero.Level.Value);
        Opponent.Army.Clear();

        int waveCount = Random.Range(1, 2); // HERE: waves 
        for (int i = 0; i < waveCount; i++)
        {
            BattleWave wave = ScriptableObject.CreateInstance<BattleWave>();

            // TODO: math for wave difficulty
            wave.NumberOfEnemies = Random.Range(10, 25);
            wave.EnemyLevelRange = new Vector2Int(1, 5);
            wave.Initialize();
            Waves.Add(wave);
        }

        foreach (Creature c in GameManager.Instance.HeroDatabase.AllMinions)
        {
            int numberOfMinions = GetTotalNumberOfMinionsByName(c.Name);
            if (numberOfMinions > 0)
            {
                // HERE: minions should be addable to army with count not level
                Creature creature = Instantiate(c);
                creature.Level = numberOfMinions;
                Opponent.Army.Add(creature);
            }
        }
    }

    public int GetTotalNumberOfMinionsByName(string minionName)
    {
        int total = 0;
        foreach (BattleWave wave in Waves)
            total += wave.GetNumberOfMinionsByName(minionName);
        return total;
    }
}