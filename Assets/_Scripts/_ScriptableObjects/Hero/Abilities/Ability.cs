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

    [SerializeField] int BaseManaCost;
    [SerializeField] int BasePower;
    [SerializeField] int BaseCooldown;
    [SerializeField] int BaseScale;

    [SerializeField] float ManaCostLevelMultiplier;
    [SerializeField] float PowerLevelMultiplier;
    [SerializeField] float CooldownLevelMultiplier;
    [SerializeField] float ScaleLevelMultiplier;

    public Element Element;
    [HideInInspector] public int KillCount;

    [Header("Battle GameObjects")]
    public GameObject AbilityExecutorPrefab;

    public event Action OnCooldownStarted;
    public void StartCooldown()
    {
        OnCooldownStarted?.Invoke();
    }

    public void IncreaseKillCount()
    {
        KillCount++;
    }

    public int GetManaCost()
    {
        return Mathf.RoundToInt(BaseManaCost * Level * ManaCostLevelMultiplier);
    }

    public int GetPower()
    {
        return Mathf.RoundToInt(BasePower * Level * PowerLevelMultiplier);
    }

    public int GetCooldown()
    {
        return Mathf.RoundToInt(BaseCooldown * Level * CooldownLevelMultiplier);
    }

    public float GetScale()
    {
        return BaseScale * Level * ScaleLevelMultiplier;
    }

    public void LevelUp()
    {
        Level++;
    }

    public void LevelDown()
    {
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
