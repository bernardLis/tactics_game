﻿using System;
using UnityEngine;

[CreateAssetMenu]
public class IntVariable : ScriptableObject
{
#if UNITY_EDITOR
    [Multiline]
    public string DeveloperDescription = "";
#endif

    public event Action<int> OnValueChanged;
    public int PreviousValue { get; private set; }
    [field: SerializeField] public int Value { get; private set; }
    public void SetValue(int newValue)
    {
        PreviousValue = Value;
        Value = newValue;
        OnValueChanged?.Invoke(Value);
    }
    
    public void SetValue(IntVariable value)
    {
        PreviousValue = Value;
        Value = value.Value;
        OnValueChanged?.Invoke(Value);
    }

    public void ApplyChange(int amount)
    {
        PreviousValue = Value;
        Value += amount;
        OnValueChanged?.Invoke(Value);
    }

    public void ApplyChange(IntVariable amount)
    {
        PreviousValue = Value;
        Value += amount.Value;
        OnValueChanged?.Invoke(Value);
    }
}
