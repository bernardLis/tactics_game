using System;
using UnityEngine;

namespace Lis
{
    [CreateAssetMenu]
    public class Vector2IntVariable : ScriptableObject
    {
#if UNITY_EDITOR
        [Multiline]
        public string DeveloperDescription = "";
#endif

        public event Action<Vector2Int> OnValueChanged;
        public Vector2Int Value;
        public void SetValue(Vector2Int value)
        {
            Value = value;
            OnValueChanged?.Invoke(Value);
        }
        public void SetValue(Vector2IntVariable value)
        {
            Value = value.Value;
            OnValueChanged?.Invoke(Value);
        }
        public void ApplyChange(Vector2Int amount)
        {
            Value += amount;
            OnValueChanged?.Invoke(Value);
        }
        public void ApplyChange(Vector2IntVariable amount)
        {
            Value += amount.Value;
            OnValueChanged?.Invoke(Value);
        }
    }
}
