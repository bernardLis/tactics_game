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


    public override void InitializeBattle(int team)
    {
        base.InitializeBattle(team);

        CurrentMana = CreateInstance<IntVariable>();
        CurrentMana.SetValue(TotalMana.GetValue());
    }

    protected override void CreateStats()
    {
        base.CreateStats();

        TotalMana = CreateInstance<Stat>();
        TotalMana.StatType = StatType.Mana;
        TotalMana.SetBaseValue(BaseTotalMana.Value);
        TotalMana.OnValueChanged += TotalMana.SetBaseValue;
    }

    /* LEVELING */
    bool _levelUpReady;
    public event Action OnLevelUpReady;
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

    public override void AddExp(int gain)
    {
        if (_levelUpReady)
        {
            LeftoverExp += gain;
            return;
        }
        if (Experience.Value + gain >= ExpForNextLevel.Value)
        {
            LeftoverExp = Experience.Value + gain - ExpForNextLevel.Value;
            Experience.SetValue(ExpForNextLevel.Value);
            LevelUpReady();
            return;
        }

        Experience.ApplyChange(gain);
    }

    public void LevelUpReady()
    {
        _levelUpReady = true;
        AudioManager.Instance.PlayUI("Level Up");

        OnLevelUpReady?.Invoke();
    }

    public void AddPower()
    {
        // BasePower.ApplyChange(1);
    }

    public void AddArmor()
    {
        BaseArmor.ApplyChange(1);
    }

    public void AddSpeed()
    {
        BaseSpeed.ApplyChange(1);
    }

    public override void LevelUp()
    {
        base.LevelUp();
        _levelUpReady = false;

        BaseTotalMana.ApplyChange(Random.Range(MaxManaGainPerLevelRange.x, MaxManaGainPerLevelRange.y));
        UpdateRank();
    }


    [Header("Mana")]
    public IntVariable BaseTotalMana;
    public Stat TotalMana { get; protected set; }
    Vector2Int MaxManaGainPerLevelRange = new(2, 5);
    public IntVariable CurrentMana;

    public float RestoreMana(int amount)
    {
        int manaMissing = TotalMana.GetValue() - CurrentMana.Value;
        if (manaMissing <= 0)
            return amount;

        if (manaMissing >= amount)
        {
            CurrentMana.ApplyChange(amount);
            return 0;
        }

        CurrentMana.ApplyChange(manaMissing);
        return amount - manaMissing;
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

    [Header("Items")]
    public List<Item> Items = new();
    public event Action<Item> OnItemAdded;
    public event Action<Item> OnItemRemoved;

    public void AddItem(Item item)
    {
        Items.Add(item);
        // UpdateStat(item.InfluencedStat, item.Value);
        UpdateRank();
        OnItemAdded?.Invoke(item);
    }

    public void RemoveItem(Item item)
    {
        Items.Remove(item);
        // UpdateStat(item.InfluencedStat, -item.Value);
        UpdateRank();
        OnItemRemoved?.Invoke(item);
    }

    // void UpdateStat(StatType type, int value)
    // {
    //     // TODO: this if statement sucks.
    //     if (type == StatType.Mana)
    //         BaseTotalMana.ApplyChange(value);
    //     if (type == StatType.Power)
    //         Power.ApplyBonusValueChange(value);
    //     if (type == StatType.Armor)
    //         Armor.ApplyBonusValueChange(value);
    //     if (type == StatType.Speed)
    //         Speed.ApplyBonusValueChange(value);
    // }

    /* HERO RANK */
    public HeroRank Rank { get; private set; }
    public event Action<HeroRank> OnRankChanged;

    public void UpdateRank()
    {
        int points = CountRankPoints();
        HeroRank newRank = _gameManager.HeroDatabase.GetRankByPoints(points);
        if (newRank == Rank)
            return;

        Rank = newRank;
        OnRankChanged?.Invoke(Rank);
    }

    public int CountRankPoints()
    {
        int total = Level.Value;

        foreach (Item i in Items)
        {
            if (i.Rarity == ItemRarity.Common)
                total += 1;
            if (i.Rarity == ItemRarity.Uncommon)
                total += 2;
            if (i.Rarity == ItemRarity.Rare)
                total += 4;
            if (i.Rarity == ItemRarity.Epic)
                total += 8;
        }

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

        Items = new();
        Abilities = new();

        UpdateRank();

        CreatureArmy = new();
        Creature c = Instantiate(_gameManager.HeroDatabase.GetStartingArmy(element).Creatures[0]);
        CreatureArmy.Add(c);
    }

    void CreateBaseStats()
    {
        Level = CreateInstance<IntVariable>();
        BaseTotalHealth = CreateInstance<IntVariable>();
        BaseTotalMana = CreateInstance<IntVariable>();
        BaseArmor = CreateInstance<IntVariable>();
        BaseSpeed = CreateInstance<IntVariable>();

        Level.SetValue(1);
        BaseTotalHealth.SetValue(100);
        BaseTotalMana.SetValue(30);
        BaseArmor.SetValue(0);
        BaseSpeed.SetValue(3);
    }


    /* SERIALIZATION */
    new public HeroData SerializeSelf()
    {
        HeroData data = new()
        {
            EntityMovementData = base.SerializeSelf(),

            Portrait = Portrait.Id,
            BaseMana = BaseTotalMana.Value,
        };

        List<AbilityData> abilityData = new();
        foreach (Ability a in Abilities)
            abilityData.Add(a.SerializeSelf());
        data.AbilityData = abilityData;

        List<string> itemIds = new();
        foreach (Item i in Items)
            itemIds.Add(i.Id);
        data.ItemIds = new(itemIds);

        data.CreatureDatas = new();
        foreach (Creature c in CreatureArmy)
            data.CreatureDatas.Add(c.SerializeSelf());

        return data;
    }


    public void LoadFromData(HeroData data)
    {
        _gameManager = GameManager.Instance;
        HeroDatabase heroDatabase = _gameManager.HeroDatabase;

        Id = data.Id;
        Portrait = heroDatabase.GetPortraitById(data.Portrait);

        CreateBaseStats();
        LoadFromData(data.EntityMovementData);

        BaseTotalMana.SetValue(data.BaseMana);

        CreateStats();

        foreach (AbilityData abilityData in data.AbilityData)
        {
            Ability a = Instantiate(heroDatabase.GetAbilityById(abilityData.TemplateId));
            a.LoadFromData(abilityData);
            Abilities.Add(a);
        }

        foreach (string id in data.ItemIds)
            AddItem(heroDatabase.GetItemById(id));

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


    public int BaseMana;

    public List<AbilityData> AbilityData;
    public List<string> ItemIds;

    public List<CreatureData> CreatureDatas;
}
