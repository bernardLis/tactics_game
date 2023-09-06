using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(menuName = "ScriptableObject/Map/Battle")]
public class Battle : BaseScriptableObject
{
    GameManager _gameManager;

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

        // Spire = CreateInstance<Spire>();
        // Spire.Initialize();

        Waves = new();
        CreateWaves();
    }

    public void CreateWaves()
    {
        List<Element> availableElements = new(_gameManager.HeroDatabase.GetAllElements());
        for (int i = 0; i < 24; i++)
        {
            if (availableElements.Count == 0)
                availableElements = new(_gameManager.HeroDatabase.GetAllElements());

            Element element = GetWaveElement(availableElements);
            availableElements.Remove(element);

            BattleWave wave = CreateInstance<BattleWave>();
            float startTime = GetWaveStartTime(element, Waves.Count);
            int difficulty = 1 + Mathf.FloorToInt(Waves.Count * 0.25f);// every 4 waves, difficulty increases by 1
            wave.CreateWave(element, difficulty, startTime);
            Waves.Add(wave);
        }

        foreach (BattleWave w in Waves)
        {
            Debug.Log($"wave: {w.Element} | diff {w.Difficulty} | start time {w.StartTime} | planned end time {w.GetPlannedEndTime()}");
        }
    }

    public Element GetWaveElement(List<Element> elements)
    {
        if (Waves.Count == 0) return _gameManager.PlayerHero.Element.StrongAgainst;

        Element lastWaveElement = Waves[Waves.Count - 1].Element;
        elements.Remove(lastWaveElement);

        // no waves of the same element in a row
        if (elements.Count > 0)
            return elements[Random.Range(0, elements.Count)];

        return _gameManager.HeroDatabase.GetRandomElement();
    }

    public float GetWaveStartTime(Element element, int waveIndex)
    {
        if (waveIndex == 0) return 25; // first wave starts at 25 seconds (actually it starts when intro ends)

        // wave starts in the "middle" of the previous wave
        BattleWave previousWave = Waves[waveIndex - 1];
        float waveSpawnFactor = 1f - waveIndex * 0.08f; // how quickly waves spawn after each other

        waveSpawnFactor = Mathf.Clamp(waveSpawnFactor, 0.1f, 1f);
        //  float startTime = (previousWave.StartTime + previousWave.GetPlannedEndTime()) * factor;
        float startTime = previousWave.StartTime + (previousWave.GetPlannedEndTime() - previousWave.StartTime) * waveSpawnFactor;

        // the wave can't start before the previous one of the same element ends
        for (int i = Waves.Count - 1; i >= 0; i--)
            if (Waves[i].Element == element && Waves[i].GetPlannedEndTime() > startTime)
                return Waves[i].GetPlannedEndTime() + 5;
        return startTime;
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