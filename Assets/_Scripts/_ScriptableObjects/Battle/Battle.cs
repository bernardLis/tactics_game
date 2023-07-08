using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(menuName = "ScriptableObject/Map/Battle")]
public class Battle : BaseScriptableObject
{
    public BattleType BattleType;
    public Hero Opponent;
    public List<BattleWave> Waves = new();

    // modifiers
    public float CreatureSpeedMultiplier = 1f;
    public float ElementalDamageMultiplier = 1f;
    public float CreatureAbilityCooldown = 1f;
    public float AbilityDamage = 1f;
    public float AbilityScale = 1f;
    public float AbilityManaCost = 1f;
    public float AbilityCooldown = 1f;


    public List<BattleModifier> BattleModifiers = new();

    public bool Won;

    public event Action<BattleModifier> OnBattleModifierAdded;
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

        // TODO: math for wave difficulty
        int waveCount = Random.Range(1, 5);
        for (int i = 0; i < waveCount; i++)
        {
            BattleWave wave = ScriptableObject.CreateInstance<BattleWave>();

            wave.NumberOfEnemies = Random.Range(10, 25);
            wave.EnemyLevelRange = new Vector2Int(1, 5);
            wave.Initialize();
            Waves.Add(wave);
        }

        foreach (Creature c in GameManager.Instance.HeroDatabase.GetAllMinions())
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

    public void AddModifier(BattleModifier modifier)
    {
        BattleModifiers.Add(modifier);
        OnBattleModifierAdded?.Invoke(modifier);

        if (modifier.BattleModifierType == BattleModifierType.CreatureSpeed)
            CreatureSpeedMultiplier *= modifier.Multiplier;
        if (modifier.BattleModifierType == BattleModifierType.ElementDamage)
            ElementalDamageMultiplier *= modifier.Multiplier;
        if (modifier.BattleModifierType == BattleModifierType.CreatureAbilityCooldown)
            CreatureAbilityCooldown *= modifier.Multiplier;
        if (modifier.BattleModifierType == BattleModifierType.AbilityDamage)
            AbilityDamage *= modifier.Multiplier;
        if (modifier.BattleModifierType == BattleModifierType.AbilityAOE)
            AbilityScale *= modifier.Multiplier;
        if (modifier.BattleModifierType == BattleModifierType.AbilityManaCost)
            AbilityManaCost *= modifier.Multiplier;
        if (modifier.BattleModifierType == BattleModifierType.AbilityCooldown)
            AbilityCooldown *= modifier.Multiplier;
    }

    public bool HasModifierOfType(BattleModifierType modifierType)
    {
        foreach (BattleModifier modifier in BattleModifiers)
            if (modifier.BattleModifierType == modifierType)
                return true;
        return false;
    }
}