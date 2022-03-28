using System.Collections.Generic;
using UnityEngine;

public enum StatType { Strength, Intelligence, Agility, Stamina, MaxHealth, MaxMana, Armor, MovementRange }

public class Stat
{
    public StatType Type;
    public int BaseValue;

    [HideInInspector] public List<StatModifier> Modifiers = new();

    // constructor https://i.redd.it/iuy9fxt300811.png
    public void Initialize(StatType type, int value)
    {
        Type = type;
        BaseValue = value;
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

    public void AddModifier(StatModifier modifier)
    {
        foreach (StatModifier s in Modifiers)
            if (s.Id == modifier.Id)
                return; // prevents stacking of the same modifier

        Modifiers.Add(modifier);
    }

    public void RemoveModifier(StatModifier modifier)
    {
        Modifiers.Remove(modifier);
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
