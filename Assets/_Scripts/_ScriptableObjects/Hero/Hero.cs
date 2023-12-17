using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Random = UnityEngine.Random;

[CreateAssetMenu(menuName = "ScriptableObject/Hero/Player")]
public class Hero : EntityMovement
{
    GameManager _gameManager;

    [Header("Stats")]
    public Stat Power;
    public Stat Pull;
    public Stat BonusExp;

    public override void InitializeBattle(int team)
    {
        base.InitializeBattle(team);

    }

    protected override void CreateStats()
    {
        // Hero handles it through CreateBaseStats() instead 
    }

    /* LEVELING */
    public int GetExpValue(int gain)
    {
        return Mathf.CeilToInt(gain + gain * BonusExp.GetValue() * 0.01f);
    }

    public override void AddExp(int gain)
    {
        base.AddExp(GetExpValue(gain));
    }

    public override int GetExpForNextLevel()
    {
        // TODO: math
        float exponent = 2.5f;
        float multiplier = 0.7f;
        int baseExp = 100;

        int result = Mathf.FloorToInt(multiplier * Mathf.Pow(Level.Value, exponent));
        result = Mathf.RoundToInt(result * 0.1f) * 10; // rounding to tens
        int expRequired = result + baseExp;

        return expRequired;
    }

    [Header("Tablets")]
    public List<Tablet> Tablets = new();
    public TabletAdvanced AdvancedTablet;
    public event Action<TabletAdvanced> OnTabletAdvancedAdded;
    public Dictionary<Element, Tablet> TabletsByElement = new();
    public void CreateTablets()
    {
        foreach (Tablet original in _gameManager.EntityDatabase.HeroTablets)
        {
            Tablet instance = Instantiate(original);
            Tablets.Add(instance);
            instance.Initialize(this);
            instance.OnLevelUp += CheckAdvancedTablets;
        }
    }

    public Tablet GetTabletByElement(Element element)
    {
        if (TabletsByElement.Count == 0)
            foreach (Tablet t in Tablets)
                TabletsByElement.Add(t.Element, t);

        return TabletsByElement[element];
    }

    void CheckAdvancedTablets()
    {
        Debug.Log($"CheckAdvancedTablets");
        if (AdvancedTablet != null) return; // only one advanced tablet

        ElementName firstElement = ElementName.None;
        ElementName secondElement = ElementName.None;

        foreach (Tablet t in Tablets)
        {
            if (!t.IsMaxLevel()) continue;
            if (firstElement == ElementName.None)
            {
                Debug.Log($"firstElement  {firstElement}");

                firstElement = t.Element.ElementName;
                continue;
            }
            if (secondElement == ElementName.None)
            {
                Debug.Log($"secondElement  {secondElement}");

                secondElement = t.Element.ElementName;
                AddAdvancedTablet(firstElement, secondElement);
                break;
            }
        }
    }

    void AddAdvancedTablet(ElementName firstElement, ElementName secondElement)
    {
        Debug.Log($"AddAdvancedTablet {firstElement} {secondElement}");

        TabletAdvanced original = _gameManager.EntityDatabase.GetAdvancedTabletByElementNames(firstElement, secondElement);
        if (original == null) return;
        Debug.Log($"adding advanced tablet {original.name}");
        AdvancedTablet = Instantiate(original);
        AdvancedTablet.Initialize(this);
        OnTabletAdvancedAdded?.Invoke(AdvancedTablet);
    }

    [Header("Abilities")]
    public List<Ability> Abilities = new();
    public Ability AbilityAdvanced;
    public event Action<Ability> OnAbilityAdded;

    public void AddAbility(Ability ability)
    {
        Ability instance = Instantiate(ability);
        instance.InitializeBattle(this);
        Abilities.Add(instance);
        OnAbilityAdded?.Invoke(instance);
    }

    public void CreateHero(string heroName, Element element)
    {
        _gameManager = GameManager.Instance;

        Id = Guid.NewGuid().ToString();
        name = heroName;

        EntityName = heroName;
        Element = element;

        CreateBaseStats();
        CreateTablets();
        foreach (Tablet t in Tablets)
            t.Initialize(this);

        Abilities = new();
    }

    void CreateBaseStats()
    {
        Level = CreateInstance<IntVariable>();
        Level.SetValue(1);

        EntityDatabase entityDatabase = _gameManager.EntityDatabase;

        MaxHealth = Instantiate(entityDatabase.GetHeroStatByType(StatType.Health));
        MaxHealth.Initialize();

        Armor = Instantiate(entityDatabase.GetHeroStatByType(StatType.Armor));
        Armor.Initialize();

        Speed = Instantiate(entityDatabase.GetHeroStatByType(StatType.Speed));
        Speed.Initialize();

        Pull = Instantiate(entityDatabase.GetHeroStatByType(StatType.Pull));
        Pull.Initialize();

        Power = Instantiate(entityDatabase.GetHeroStatByType(StatType.Power));
        Power.Initialize();

        BonusExp = Instantiate(entityDatabase.GetHeroStatByType(StatType.ExpBonus));
        BonusExp.Initialize();

        UpgradeBoard globalUpgradeBoard = _gameManager.UpgradeBoard;
        MaxHealth.ApplyBaseValueChange(globalUpgradeBoard.GetUpgradeByName("Hero Health").GetValue());
        Armor.ApplyBaseValueChange(globalUpgradeBoard.GetUpgradeByName("Hero Armor").GetValue());
        Speed.ApplyBaseValueChange(globalUpgradeBoard.GetUpgradeByName("Hero Speed").GetValue());
        Pull.ApplyBaseValueChange(globalUpgradeBoard.GetUpgradeByName("Hero Pull").GetValue());
        Power.ApplyBaseValueChange(globalUpgradeBoard.GetUpgradeByName("Hero Power").GetValue());
        BonusExp.ApplyBaseValueChange(globalUpgradeBoard.GetUpgradeByName("Hero Exp Bonus").GetValue());
    }

    public List<Stat> GetAllStats()
    {
        List<Stat> stats = new()
        {
            MaxHealth,
            Armor,
            Speed,
            Pull,
            Power,
            BonusExp
        };
        return stats;
    }

    public Stat GetStatByType(StatType type)
    {
        switch (type)
        {
            case StatType.Health:
                return MaxHealth;
            case StatType.Armor:
                return Armor;
            case StatType.Speed:
                return Speed;
            case StatType.Pull:
                return Pull;
            case StatType.Power:
                return Power;
            case StatType.ExpBonus:
                return BonusExp;
            default:
                return null;
        }
    }

    /* SERIALIZATION */
    new public HeroData SerializeSelf()
    {
        HeroData data = new()
        {
            EntityMovementData = base.SerializeSelf(),
        };

        List<AbilityData> abilityData = new();
        foreach (Ability a in Abilities)
            abilityData.Add(a.SerializeSelf());
        data.AbilityData = abilityData;

        return data;
    }

    public void LoadFromData(HeroData data)
    {
        _gameManager = GameManager.Instance;
        EntityDatabase heroDatabase = _gameManager.EntityDatabase;

        Id = data.Id;

        CreateBaseStats();
        LoadFromData(data.EntityMovementData);

        CreateStats();

        foreach (AbilityData abilityData in data.AbilityData)
        {
            Ability a = Instantiate(heroDatabase.GetAbilityById(abilityData.TemplateId));
            a.LoadFromData(abilityData);
            Abilities.Add(a);
        }
    }
}

[Serializable]
public struct HeroData
{
    public EntityMovementData EntityMovementData;
    public string Id;

    public List<AbilityData> AbilityData;
}
