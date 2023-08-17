using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(menuName = "ScriptableObject/Map/Battle")]
public class Battle : BaseScriptableObject
{
    GameManager _gameManager;

    public Hero Opponent;
    public List<BattleWave> Waves = new();

    public Spire Spire;

    public int Duration = 900; // seconds

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
    public void CreateRandom(int level)
    {
        _gameManager = GameManager.Instance;

        Spire = CreateInstance<Spire>();
        Spire.Initialize();

        Opponent = CreateInstance<Hero>();
        Opponent.CreateRandom(_gameManager.PlayerHero.Level.Value);
        Opponent.CreatureArmy.Clear();

        CreateWaves(level);
    }

    public void CreateWaves(int level)
    {
        // HERE: waves - make sure that there are no overlapping waves

        List<Element> availableElements = new(_gameManager.HeroDatabase.GetAllElements());
        Waves = new();
        for (int i = 0; i < 10; i++)
        {
            if (availableElements.Count == 0)
                availableElements = new(_gameManager.HeroDatabase.GetAllElements());

            Element element = availableElements[Random.Range(0, availableElements.Count)];
            availableElements.Remove(element);

            BattleWave wave = CreateInstance<BattleWave>();
            wave.CreateWave(element, level + i, i * Random.Range(30, 60));
            Waves.Add(wave);
        }

        foreach (BattleWave w in Waves)
        {
            Debug.Log($"wave: {w.Element}, start time {w.StartTime}, planned end time {w.StartTime + w.DelayBetweenGroups * w.OpponentGroups.Count}");
        }

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
        if (BattleModifiers == null) return false;

        foreach (BattleModifier modifier in BattleModifiers)
            if (modifier.BattleModifierType == modifierType)
                return true;
        return false;
    }
}