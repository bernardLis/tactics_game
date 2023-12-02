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

    BattleManager _battleManager;
    public void InitializeBattle()
    {
        _battleManager = BattleManager.Instance;
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

    public int GetCooldown()
    {
        return Mathf.FloorToInt(Levels[Level].Cooldown);
    }

    public float GetScale()
    {
        return Levels[Level].Scale;
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
