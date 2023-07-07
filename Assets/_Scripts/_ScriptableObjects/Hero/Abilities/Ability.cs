using System;
using UnityEngine;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine.UIElements;

[CreateAssetMenu(menuName = "ScriptableObject/Hero/Ability")]
public class Ability : BaseScriptableObject
{
    [Header("Base Characteristics")]
    public string Description = "New Description";
    public Sprite Icon;
    public int Level;
    public bool IsStartingAbility;

    public Sound AbilityNameSound;

    [SerializeField] int _baseManaCost;
    [SerializeField] int _basePower;
    [SerializeField] int _baseCooldown;
    [SerializeField] int _baseScale;

    [SerializeField] float _manaCostLevelMultiplier;
    [SerializeField] float _powerLevelMultiplier;
    [SerializeField] float _cooldownLevelMultiplier;
    [SerializeField] float _scaleLevelMultiplier;

    // battle modifiers
    float _battleDamageMultiplier = 1f;
    float _battleCooldownMultiplier = 1f;
    float _battleManaCostMultiplier = 1f;
    float _battleScaleMultiplier = 1f;

    public Element Element;
    [HideInInspector] public int KillCount;

    [Header("Battle GameObjects")]
    public GameObject AbilityExecutorPrefab;

    public event Action OnCooldownStarted;

    public void InitializeBattle()
    {
        Battle b = GameManager.Instance.SelectedBattle;

        _battleDamageMultiplier = b.AbilityDamage;
        _battleCooldownMultiplier = b.AbilityCooldown;
        _battleManaCostMultiplier = b.AbilityManaCost;
        _battleScaleMultiplier = b.AbilityScale;
    }

    public void StartCooldown()
    {
        OnCooldownStarted?.Invoke();
    }

    public int GetManaCost()
    {
        int manaCost = Mathf.RoundToInt(_baseManaCost + ((Level - 1) * _manaCostLevelMultiplier));
        return Mathf.RoundToInt(manaCost * _battleManaCostMultiplier);
    }

    public int GetPower()
    {
        int power = Mathf.RoundToInt(_basePower + ((Level - 1) * _powerLevelMultiplier));
        return Mathf.RoundToInt(power * _battleDamageMultiplier);
    }

    public int GetCooldown()
    {
        int cooldown = Mathf.RoundToInt(_baseCooldown + ((Level - 1) * _cooldownLevelMultiplier));
        return Mathf.RoundToInt(cooldown * _battleCooldownMultiplier);
    }

    public float GetScale()
    {
        int scale = Mathf.RoundToInt(_baseScale + ((Level - 1) * _scaleLevelMultiplier));
        return scale * _battleScaleMultiplier;
    }

    public void IncreaseKillCount() { KillCount++; }

    public void LevelUp() { Level++; }

    public void LevelDown() { Level--; }

    public void LoadFromData(AbilityData data)
    {
        Level = data.Level;
        KillCount = data.KillCount;
    }

    public AbilityData SerializeSelf()
    {
        AbilityData data = new();
        if (this == null)
            return data;

        data.TemplateId = Id;
        data.Level = Level;
        data.KillCount = KillCount;
        return data;
    }
}

[Serializable]
public struct AbilityData
{
    public string TemplateId;
    public string Name;
    public int Level;
    public int KillCount;
}
