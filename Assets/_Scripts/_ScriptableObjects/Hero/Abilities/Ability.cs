using System;
using UnityEngine;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine.UIElements;

[CreateAssetMenu(menuName = "ScriptableObject/Hero/Ability")]
public class Ability : BaseScriptableObject
{
#if UNITY_EDITOR
    [Multiline]
    public string DeveloperComment = "";
#endif

    [Header("Base Characteristics")]
    public string Description = "New Description";
    public Sprite Icon;
    public int Level;

    public List<AbilityLevel> Levels;

    // battle modifiers
    float _battleDamageMultiplier = 1f;
    float _battleCooldownMultiplier = 1f;
    float _battleScaleMultiplier = 1f;

    public Element Element;
    [HideInInspector] public int KillCount;

    [Header("Battle GameObjects")]
    public GameObject AbilityManagerPrefab;

    public event Action OnCooldownStarted;

    public void InitializeBattle()
    {
        Battle b = GameManager.Instance.CurrentBattle;

        _battleDamageMultiplier = b.AbilityDamage;
        _battleCooldownMultiplier = b.AbilityCooldown;
        _battleScaleMultiplier = b.AbilityScale;
    }

    public void StartCooldown()
    {
        OnCooldownStarted?.Invoke();
    }

    public int GetPower()
    {
        return Mathf.FloorToInt(Levels[Level].Power * _battleDamageMultiplier);
    }

    public int GetCooldown()
    {
        return Mathf.FloorToInt(Levels[Level].Cooldown * _battleCooldownMultiplier);
    }

    public float GetScale()
    {
        return Levels[Level].Scale * _battleScaleMultiplier;
    }

    public bool HasMoreUpgrades()
    {
        if (Level == 0) return true;
        
        return Level < Levels.Count - 1;
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
