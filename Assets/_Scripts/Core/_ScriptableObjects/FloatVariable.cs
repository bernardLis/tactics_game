﻿using System;
using UnityEngine;

namespace Lis.Core
{
    [CreateAssetMenu]
    public class FloatVariable : ScriptableObject
    {
#if UNITY_EDITOR
        [Multiline]
        public string DeveloperDescription = "";
#endif
        public float Value;
        public float PreviousValue { get; private set; }

        public event Action<float> OnValueChanged;

        public void SetValue(float value)
        {
            PreviousValue = Value;
            Value = value;
            OnValueChanged?.Invoke(Value);
        }

        public void SetValue(FloatVariable value)
        {
            PreviousValue = Value;
            Value = value.Value;
            OnValueChanged?.Invoke(Value);
        }

        public void ApplyChange(float amount)
        {
            PreviousValue = Value;
            Value += amount;
            OnValueChanged?.Invoke(Value);
        }

        public void ApplyChange(FloatVariable amount)
        {
            PreviousValue = Value;
            Value += amount.Value;
            OnValueChanged?.Invoke(Value);
        }
    }
}