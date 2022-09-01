using System.Collections.Generic;
using UnityEngine;
using System;

public enum StatType { Power, MaxHealth, MaxMana, Armor, MovementRange }

public class Stat
{
    public StatType Type;
    public int BaseValue;
    Character _character;

    [HideInInspector] public List<StatModifier> Modifiers = new();

    public event Action<StatModifier> OnModifierAdded;
    public event Action<StatModifier> OnModifierRemoved;

    // constructor https://i.redd.it/iuy9fxt300811.png
    public void Initialize(StatType type, int value, Character character)
    {
        Type = type;
        BaseValue = value;

        _character = character;
        character.OnCharacterLevelUp += OnCharacterLevelUp;
    }

    void OnCharacterLevelUp()
    {
        // TODO: this presumes that character keeps the names of stats same as stat types
        BaseValue = _character.GetStatValue(Type.ToString());
    }

    public int GetValue()
    {
        int finalValue = BaseValue;
        foreach (StatModifier m in Modifiers)
            finalValue += m.Value;

        // final value can't be negative
        finalValue = Mathf.Clamp(finalValue, 0, int.MaxValue);

        return finalValue;
    }

    public bool AddModifier(StatModifier modifier)
    {
        foreach (StatModifier s in Modifiers)
            if (s.Id == modifier.Id)
                return false; // prevents stacking of the same modifier

        Modifiers.Add(modifier);
        OnModifierAdded?.Invoke(modifier);
        return true;
    }

    public void RemoveModifier(StatModifier modifier)
    {
        Modifiers.Remove(modifier);
        OnModifierRemoved?.Invoke(modifier);
    }

    public void TurnEndDecrement()
    {
        for (int i = Modifiers.Count - 1; i >= 0; i--)
        {
            Modifiers[i].NumberOfTurns--;
            if (Modifiers[i].NumberOfTurns <= 0)
                RemoveModifier(Modifiers[i]);
        }
    }

    public List<StatModifier> GetActiveModifiers()
    {
        return Modifiers;
    }
}
