using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Random = UnityEngine.Random;

[CreateAssetMenu(menuName = "ScriptableObject/Hero/Player")]
public class Hero : EntityMovement
{
    GameManager _gameManager;

    public HeroPortrait Portrait;

    public Stat Pull;

    public override void InitializeBattle(int team)
    {
        base.InitializeBattle(team);
    }

    protected override void CreateStats()
    {
        base.CreateStats();
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


    [Header("Army")]
    public List<Creature> CreatureArmy = new();
    public event Action<Creature> OnCreatureAdded;
    public event Action<Creature> OnCreatureRemoved;

    public void AddCreature(Creature creature, bool noDelegate = false)
    {
        Debug.Log($"Hero {name} adds army {creature}");
        CreatureArmy.Add(creature);

        if (noDelegate) return;
        OnCreatureAdded?.Invoke(creature);
    }

    public void RemoveCreature(Creature creature)
    {
        Debug.Log($"Hero {name} removes {creature}");
        CreatureArmy.Remove(creature);
        OnCreatureRemoved?.Invoke(creature);
    }

    [Header("Abilities")]
    public List<Ability> Abilities = new();
    public event Action<Ability> OnAbilityAdded;
    public event Action<Ability> OnAbilityRemoved;

    public void AddAbility(Ability ability)
    {
        Ability instance = Instantiate(ability);
        Abilities.Add(instance);
        UpdateRank();
        OnAbilityAdded?.Invoke(instance);
    }

    public void RemoveAbility(Ability ability)
    {
        Abilities.Remove(ability);
        UpdateRank();
        OnAbilityRemoved?.Invoke(ability);
    }

    /* HERO RANK */
    public HeroRank Rank { get; private set; }
    public event Action<HeroRank> OnRankChanged;

    public void UpdateRank()
    {
        int points = CountRankPoints();
        HeroRank newRank = _gameManager.EntityDatabase.GetRankByPoints(points);
        if (newRank == Rank)
            return;

        Rank = newRank;
        OnRankChanged?.Invoke(Rank);
    }

    public int CountRankPoints()
    {
        int total = Level.Value;
        return total;
    }

    /* HERO CREATION */

    public void CreateFromHeroCreation(string heroName, HeroPortrait portrait, Element element)
    {
        _gameManager = GameManager.Instance;

        Id = Guid.NewGuid().ToString();
        name = heroName;

        EntityName = heroName;
        Portrait = portrait;
        Element = element;

        CreateBaseStats();

        Abilities = new();

        UpdateRank();

        CreatureArmy = new();
        Creature c = Instantiate(_gameManager.EntityDatabase.GetStartingArmy(element).Creatures[0]);
        CreatureArmy.Add(c);
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

        Level.SetValue(1);
        MaxHealth.SetBaseValue(100);
        Armor.SetBaseValue(0);
        Speed.SetBaseValue(3);
        Pull.SetBaseValue(7);
    }

    /* SERIALIZATION */
    new public HeroData SerializeSelf()
    {
        HeroData data = new()
        {
            EntityMovementData = base.SerializeSelf(),

            Portrait = Portrait.Id,
        };

        List<AbilityData> abilityData = new();
        foreach (Ability a in Abilities)
            abilityData.Add(a.SerializeSelf());
        data.AbilityData = abilityData;

        data.CreatureDatas = new();
        foreach (Creature c in CreatureArmy)
            data.CreatureDatas.Add(c.SerializeSelf());

        return data;
    }

    public void LoadFromData(HeroData data)
    {
        _gameManager = GameManager.Instance;
        EntityDatabase heroDatabase = _gameManager.EntityDatabase;

        Id = data.Id;
        Portrait = heroDatabase.GetPortraitById(data.Portrait);

        CreateBaseStats();
        LoadFromData(data.EntityMovementData);

        CreateStats();

        foreach (AbilityData abilityData in data.AbilityData)
        {
            Ability a = Instantiate(heroDatabase.GetAbilityById(abilityData.TemplateId));
            a.LoadFromData(abilityData);
            Abilities.Add(a);
        }

        CreatureArmy = new();
        foreach (CreatureData d in data.CreatureDatas)
        {
            Creature baseCreature = heroDatabase.GetCreatureById(d.CreatureId);
            Creature c = Instantiate(baseCreature);
            c.LoadFromData(d);
            CreatureArmy.Add(c);
        }

        UpdateRank();
    }

}

[Serializable]
public struct HeroData
{
    public EntityMovementData EntityMovementData;
    public string Id;
    public string Portrait;

    public List<AbilityData> AbilityData;

    public List<CreatureData> CreatureDatas;
}
