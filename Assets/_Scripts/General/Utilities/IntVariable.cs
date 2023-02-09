using System;
using UnityEngine;

[CreateAssetMenu]
public class IntVariable : ScriptableObject
{
#if UNITY_EDITOR
    [Multiline]
    public string DeveloperDescription = "";
#endif

    public event Action<int> OnValueChanged;
    public int Value;
    public void SetValue(int value)
    {
        Value = value;
        OnValueChanged?.Invoke(Value);
    }
    public void SetValue(IntVariable value)
    {
        Value = value.Value;
        OnValueChanged?.Invoke(Value);
    }
    public void ApplyChange(int amount)
    {
        Value += amount;
        OnValueChanged?.Invoke(Value);
    }
    public void ApplyChange(IntVariable amount)
    {
        Value += amount.Value;
        OnValueChanged?.Invoke(Value);
    }
}
