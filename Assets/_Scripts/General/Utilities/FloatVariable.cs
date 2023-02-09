using System;
using UnityEngine;

[CreateAssetMenu]
public class FloatVariable : ScriptableObject
{
#if UNITY_EDITOR
    [Multiline]
    public string DeveloperDescription = "";
#endif

    public event Action<float> OnValueChanged;
    public float Value;
    public void SetValue(float value)
    {
        Value = value;
        OnValueChanged?.Invoke(Value);
    }
    public void SetValue(FloatVariable value)
    {
        Value = value.Value;
        OnValueChanged?.Invoke(Value);
    }
    public void ApplyChange(float amount)
    {
        Value += amount;
        OnValueChanged?.Invoke(Value);
    }
    public void ApplyChange(FloatVariable amount)
    {
        Value += amount.Value;
        OnValueChanged?.Invoke(Value);
    }
}
