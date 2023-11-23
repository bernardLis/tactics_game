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
        base.CreateStats();
        Power = Instantiate(Power);
        Pull = Instantiate(Pull);
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
        MaxHealth = CreateInstance<Stat>();
        MaxHealth.StatType = StatType.Health;
        MaxHealth.Initialize();

        Armor = CreateInstance<Stat>();
        Armor.StatType = StatType.Armor;
        Armor.Initialize();

        Speed = CreateInstance<Stat>();
        Speed.StatType = StatType.Speed;
        Speed.Initialize();

        Pull = CreateInstance<Stat>();
        Pull.StatType = StatType.Pull;
        Pull.Initialize();

        Power = CreateInstance<Stat>();
        Power.StatType = StatType.Power;
        Power.Initialize();

        Level.SetValue(1);

        GlobalUpgradeBoard globalUpgradeBoard = _gameManager.GlobalUpgradeBoard;

        int maxHealth = 100 + globalUpgradeBoard.HeroHealth.GetValue();
        MaxHealth.SetBaseValue(maxHealth);

        int armor = 0 + globalUpgradeBoard.HeroArmor.GetValue();
        Armor.SetBaseValue(armor);

        int speed = 7 + globalUpgradeBoard.HeroSpeed.GetValue();
        Speed.SetBaseValue(speed);

        int pull = 7 + globalUpgradeBoard.HeroPull.GetValue();
        Pull.SetBaseValue(pull);

        int power = 0 + globalUpgradeBoard.HeroPower.GetValue();
        Power.SetBaseValue(power);
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
