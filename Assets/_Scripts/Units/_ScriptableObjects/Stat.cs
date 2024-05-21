using System;
using Lis.Core;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Lis.Units
{
    [CreateAssetMenu]
    public class Stat : BaseScriptableObject
    {
        [Header("Stat Info")]
        public StatType StatType;

        public Sprite Icon;
        public string Description;

        [Header("Stat Values")]
        public float BaseValue;

        [HideInInspector] public float BonusValue;
        public bool IsDecreasingPerLevel;

        [Tooltip("Min: inclusive, Max: inclusive")]
        public Vector2 GrowthPerLevelRange;

        public Vector2 MinMaxValue = new(0, 999);

        public event Action<float> OnValueChanged;

        public void Initialize()
        {
            BonusValue = 0;
            if (Icon == null)
                Icon = GameManager.Instance.UnitDatabase.GetStatIconByType(StatType);
            if (string.IsNullOrEmpty(Description))
                Description = GameManager.Instance.UnitDatabase.GetStatDescriptionByType(StatType);
        }

        public void LevelUp()
        {
            float growth = Random.Range(GrowthPerLevelRange.x, GrowthPerLevelRange.y);

            if (IsDecreasingPerLevel) growth *= -1;

            BaseValue += growth;
            BaseValue = Mathf.Clamp(BaseValue, MinMaxValue.x, MinMaxValue.y);

            OnValueChanged?.Invoke(GetValue());
        }

        public float GetValue()
        {
            float totalValue = BaseValue + BonusValue;
            return Mathf.Clamp(totalValue, MinMaxValue.x, MinMaxValue.y);
        }

        public void SetBaseValue(float value)
        {
            BaseValue = value;
            OnValueChanged?.Invoke(GetValue());
        }

        public void SetBonusValue(float value)
        {
            BonusValue = value;
            OnValueChanged?.Invoke(GetValue());
        }

        public void ApplyBaseValueChange(float value)
        {
            BaseValue += value;
            OnValueChanged?.Invoke(GetValue());
        }

        public void ApplyBonusValueChange(float value)
        {
            BonusValue += value;
            OnValueChanged?.Invoke(GetValue());
        }
    }
}