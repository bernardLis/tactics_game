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

    public Element Element;
    [HideInInspector] public int KillCount;

    [Header("Battle GameObjects")]
    public GameObject AbilityManagerPrefab;

    public event Action OnCooldownStarted;
    public event Action OnLevelUp;

    float _cooldownMultiplier = 1f;
    float _scaleMultiplier = 1f;

    BattleManager _battleManager;
    public void InitializeBattle()
    {
        _battleManager = BattleManager.Instance;

        UpgradeBoard globalUpgradeBoard = GameManager.Instance.UpgradeBoard;
        float cooldownUpgrade = globalUpgradeBoard.GetUpgradeByName("Ability Cooldown").GetValue();
        float scaleUpgrade = globalUpgradeBoard.GetUpgradeByName("Ability Scale").GetValue();

        _cooldownMultiplier = 1 - cooldownUpgrade * 0.01f;
        _scaleMultiplier = 1 + scaleUpgrade * 0.01f;
    }

    public void StartCooldown()
    {
        OnCooldownStarted?.Invoke();
    }

    public int GetPower()
    {
        return Mathf.FloorToInt(Levels[Level].Power *
                                (1 + _battleManager.BattleHero.Hero.Power.GetValue() * 0.1f));
    }

    public float GetCooldown()
    {
        return Levels[Level].Cooldown * _cooldownMultiplier;
    }

    public float GetScale()
    {
        return Levels[Level].Scale * _scaleMultiplier;
    }

    public bool HasMoreUpgrades()
    {
        if (Level == 0) return true;

        return Level < Levels.Count - 1;
    }

    public void AddKill() { KillCount++; }

    public void LevelUp()
    {
        if (Level < Levels.Count - 1)
            Level++;

        OnLevelUp?.Invoke();
    }

    public void LevelDown()
    {
        if (Level > 0)
            Level--;
    }

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
