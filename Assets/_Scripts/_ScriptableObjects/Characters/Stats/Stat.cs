using System.Collections.Generic;
using UnityEngine;
using System;

public class Stat : BaseScriptableObject
{
    public StatType StatType;

    public int BaseValue;
    public int BonusValue;

    public event Action<int> OnValueChanged;

    public int GetValue() { return BaseValue + BonusValue; }

    public void SetBaseValue(int value)
    {
        BaseValue = value;
        OnValueChanged?.Invoke(GetValue());
    }

    public void SetBonusValue(int value)
    {
        BonusValue = value;
        OnValueChanged?.Invoke(GetValue());
    }

    public void ApplyBaseValueChange(int value)
    {
        BaseValue += value;
        OnValueChanged?.Invoke(GetValue());
    }

    public void ApplyBonusValueChange(int value)
    {
        BonusValue += value;
        OnValueChanged?.Invoke(GetValue());
    }
}