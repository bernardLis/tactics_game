using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StatType { Strength, Intelligence, Agility, Stamina, MaxHealth, MaxMana, Armor, MovementRange }

[System.Serializable]
public class Stat
{
    [SerializeField] public StatType type;
    [SerializeField] public int baseValue;

    [HideInInspector] public List<StatModifier> modifiers = new();

    // constructor https://i.redd.it/iuy9fxt300811.png
    public Stat(StatType _type) { type = _type; }

    public int GetValue()
    {
        int finalValue = baseValue;
        foreach (StatModifier m in modifiers)
            finalValue += m.value;

        // final value can't be negative
        finalValue = Mathf.Clamp(finalValue, 0, int.MaxValue);

        return finalValue;
    }

    public void AddModifier(StatModifier _modifier)
    {
        modifiers.Add(_modifier);
    }
    public void RemoveModifier(StatModifier _modifier)
    {
        modifiers.Remove(_modifier);
    }
    public void TurnEndDecrement()
    {
        for (int i = modifiers.Count - 1; i >= 0; i--)
        {
            modifiers[i].numberOfTurns--;
            if (modifiers[i].numberOfTurns <= 0)
                RemoveModifier(modifiers[i]);
        }
    }

    public List<StatModifier> GetActiveModifiers()
    {
        return modifiers;
    }
}
