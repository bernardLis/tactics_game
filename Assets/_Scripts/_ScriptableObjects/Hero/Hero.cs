using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Random = UnityEngine.Random;

[CreateAssetMenu(menuName = "ScriptableObject/Hero/Player")]
public class Hero : EntityMovement
{
    GameManager _gameManager;

    public Stat Power;
    public Stat Pull;

    public override void InitializeBattle(int team)
    {
        base.InitializeBattle(team);
    }

    protected override void CreateStats()
    {
    }

    /* LEVELING */
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


    [Header("Abilities")]
    public List<Ability> Abilities = new();
    public event Action<Ability> OnAbilityAdded;
    public event Action<Ability> OnAbilityRemoved;

    public void AddAbility(Ability ability)
    {
        Ability instance = Instantiate(ability);
        instance.InitializeBattle();
        Abilities.Add(instance);
        OnAbilityAdded?.Invoke(instance);
    }

    public void RemoveAbility(Ability ability)
    {
        Abilities.Remove(ability);
        OnAbilityRemoved?.Invoke(ability);
    }

    public void CreateHero(string heroName, Element element)
    {
        _gameManager = GameManager.Instance;

        Id = Guid.NewGuid().ToString();
        name = heroName;

        EntityName = heroName;
        Element = element;

        CreateBaseStats();

        Abilities = new();
        // HERE: testing
        //  AddAbility(_gameManager.EntityDatabase.GetRandomAbility());
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

        GlobalUpgradeBoard globalUpgradeBoard = _gameManager.GlobalUpgradeBoard;
        MaxHealth.ApplyBaseValueChange(globalUpgradeBoard.GetUpgradeByName("Hero Health").GetValue());
        Armor.ApplyBaseValueChange(globalUpgradeBoard.GetUpgradeByName("Hero Armor").GetValue());
        Speed.ApplyBaseValueChange(globalUpgradeBoard.GetUpgradeByName("Hero Speed").GetValue());
        Pull.ApplyBaseValueChange(globalUpgradeBoard.GetUpgradeByName("Hero Pull").GetValue());
        Power.ApplyBaseValueChange(globalUpgradeBoard.GetUpgradeByName("Hero Power").GetValue());
    }

    public List<Stat> GetAllStats()
    {
        List<Stat> stats = new()
        {
            MaxHealth,
            Armor,
            Speed,
            Pull,
            Power
        };
        return stats;
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
