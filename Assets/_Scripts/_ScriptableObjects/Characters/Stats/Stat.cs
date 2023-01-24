using System.Collections.Generic;
using UnityEngine;
using System;

public class Stat
{
    public StatType Type;
    public int BaseValue;
    public Character Character;

    public int GetValue()
    {
        int finalValue = BaseValue;

        // final value can't be negative
        finalValue = Mathf.Clamp(finalValue, 0, int.MaxValue);

        return finalValue;
    }
}