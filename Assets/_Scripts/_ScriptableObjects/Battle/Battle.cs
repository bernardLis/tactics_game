using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(menuName = "ScriptableObject/Map/Battle")]
public class Battle : BaseScriptableObject
{
    public Hero Opponent;
    public List<BattleWave> Waves = new();

    public Base Base;

    public int Duration = 15; // minutes

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
        GameManager gameManager = GameManager.Instance;

        Base = ScriptableObject.CreateInstance<Base>();
        Base.Initialize();

        Opponent = ScriptableObject.CreateInstance<Hero>();
        Opponent.CreateRandom(gameManager.PlayerHero.Level.Value);
        Opponent.CreatureArmy.Clear();
    }

    public BattleWave GetWave(int difficulty)
    {
        BattleWave wave = ScriptableObject.CreateInstance<BattleWave>();
        wave.CreateWave(difficulty);
        Waves.Add(wave);

        return wave;
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
        if (BattleModifiers == null) return false;

        foreach (BattleModifier modifier in BattleModifiers)
            if (modifier.BattleModifierType == modifierType)
                return true;
        return false;
    }
}