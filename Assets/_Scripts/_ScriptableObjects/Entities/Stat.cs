using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

[CreateAssetMenu]
public class Stat : BaseScriptableObject
{
    public StatType StatType;
    public Sprite Icon;
    public string Description;

    public int BaseValue;
    [HideInInspector] public int BonusValue;
    public bool IsDecreasingPerLevel;
    public Vector2Int GrowthPerLevelRange;
    public Vector2Int MaxMinValue;

    public event Action<int> OnValueChanged;

    public void Initialize()
    {
        if (Icon == null)
            Icon = GameManager.Instance.EntityDatabase.GetStatIconByType(StatType);
        if (Description.Length == 0)
            Description = GameManager.Instance.EntityDatabase.GetStatDescriptionByType(StatType);
    }

    public void LevelUp()
    {
        int growth = Random.Range(GrowthPerLevelRange.x, GrowthPerLevelRange.y + 1);
        growth = Mathf.Clamp(growth, MaxMinValue.x, MaxMinValue.y);

        if (IsDecreasingPerLevel)
            growth *= -1;

        BaseValue += growth;
        OnValueChanged?.Invoke(GetValue());
    }

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