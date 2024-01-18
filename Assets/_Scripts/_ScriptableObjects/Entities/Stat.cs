using System;

using UnityEngine;
using Random = UnityEngine.Random;

namespace Lis
{
    [CreateAssetMenu]
    public class Stat : BaseScriptableObject
    {
        [Header("Stat Info")]
        public StatType StatType;
        public Sprite Icon;
        public string Description;

        [Header("Stat Values")]
        public int BaseValue;
        [HideInInspector] public int BonusValue;
        public bool IsDecreasingPerLevel;
        [Tooltip("Min: inclusive, Max: inclusive")]
        public Vector2Int GrowthPerLevelRange;
        public Vector2Int MinMaxValue = new Vector2Int(0, 999);

        public event Action<int> OnValueChanged;

        public void Initialize()
        {
            if (Icon == null)
                Icon = GameManager.Instance.EntityDatabase.GetStatIconByType(StatType);
            if (Description == null || Description.Length == 0)
                Description = GameManager.Instance.EntityDatabase.GetStatDescriptionByType(StatType);
        }

        public void LevelUp()
        {
            int growth = Random.Range(GrowthPerLevelRange.x, GrowthPerLevelRange.y + 1);

            if (IsDecreasingPerLevel) growth *= -1;

            BaseValue += growth;
            BaseValue = Mathf.Clamp(BaseValue, MinMaxValue.x, MinMaxValue.y);

            OnValueChanged?.Invoke(GetValue());
        }

        public int GetValue()
        {
            int totalValue = BaseValue + BonusValue;
            return Mathf.Clamp(totalValue, MinMaxValue.x, MinMaxValue.y);
        }

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
}