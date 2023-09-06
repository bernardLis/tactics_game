using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Random = UnityEngine.Random;

[CreateAssetMenu(menuName = "ScriptableObject/Hero/Player")]
public class Hero : EntityMovement
{
    static Vector2Int MaxManaGainPerLevelRange = new(2, 5);

    GameManager _gameManager;

    public HeroPortrait Portrait;

    bool _levelUpReady;

    public IntVariable BaseTotalMana;
    public Stat TotalMana { get; protected set; }

    public HeroRank Rank { get; private set; }

    [Header("Items")]
    public List<Item> Items = new();

    [Header("Abilities")]
    public List<Ability> Abilities = new();

    public List<Creature> CreatureArmy = new();
    public List<Minion> MinionArmy = new();

    public event Action OnLevelUpReady;
    public event Action<Ability> OnAbilityAdded;
    public event Action<Ability> OnAbilityRemoved;
    public event Action<Item> OnItemAdded;
    public event Action<HeroRank> OnRankChanged;
    public event Action<Creature> OnCreatureAdded;
    public event Action<Creature> OnCreatureRemoved;

    bool _tutorialCompleted;
    public IntVariable CurrentMana;
    public void BattleInitialize()
    {
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

    // returns leftover
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

    public override int GetExpForNextLevel()
    {
        // TODO: math
        float exponent = 2.5f;
        float multiplier = 0.7f;
        int baseExp = 100;

        int result = Mathf.FloorToInt(multiplier * Mathf.Pow(Level.Value, exponent));
        result = Mathf.RoundToInt(result * 0.1f) * 10; // rounding to tens
        int expRequired = result + baseExp;

        if (!_tutorialCompleted)
        {
            _tutorialCompleted = true;
            expRequired = 20; // HERE: tutorial
        }
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

    public override void LevelUp()
    {
        base.LevelUp();
        _levelUpReady = false;

        BaseTotalMana.ApplyChange(Random.Range(MaxManaGainPerLevelRange.x, MaxManaGainPerLevelRange.y));
        UpdateRank();
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

    void CreateBaseStats()
    {
        // Level = CreateInstance<IntVariable>();
        // Experience = CreateInstance<IntVariable>();
        // ExpForNextLevel = CreateInstance<IntVariable>();
        BaseTotalHealth = CreateInstance<IntVariable>();
        BaseTotalMana = CreateInstance<IntVariable>();
        // BasePower = CreateInstance<IntVariable>();
        BaseArmor = CreateInstance<IntVariable>();
        BaseSpeed = CreateInstance<IntVariable>();
        Level = CreateInstance<IntVariable>(); 
        InitializeBattle();

    }


    public void CreateFromHeroCreation(string heroName, HeroPortrait portrait, Element element)
    {
        _gameManager = GameManager.Instance;

        name = heroName;
        Id = Guid.NewGuid().ToString();

        EntityName = heroName;
        Portrait = portrait;
        Element = element;

        CreateBaseStats();

        Level.SetValue(1);
        Experience.SetValue(0);
        ExpForNextLevel.SetValue(GetExpForNextLevel());

        BaseTotalHealth.SetValue(100);
        BaseTotalMana.SetValue(30);
        // BasePower.SetValue(5);
        BaseArmor.SetValue(0);
        BaseSpeed.SetValue(3);

        CreateStats();

        Items = new();

        Abilities = new();

        UpdateRank();

        CreatureArmy = new();
        // HERE: hero creation
        Creature c = Instantiate(_gameManager.HeroDatabase.GetStartingArmy(element).Creatures[0]);
        CreatureArmy.Add(c);
    }

    public void CreateRandom(int level)
    {
        _gameManager = GameManager.Instance;
        HeroDatabase heroDatabase = _gameManager.HeroDatabase;

        bool isMale = Random.value > 0.5f;

        Id = Guid.NewGuid().ToString();

        EntityName = isMale ? heroDatabase.GetRandomNameMale() : heroDatabase.GetRandomNameFemale();
        name = EntityName;
        Portrait = isMale ? heroDatabase.GetRandomPortraitMale() : heroDatabase.GetRandomPortraitFemale();

        Element = heroDatabase.GetRandomElement();

        CreateBaseStats();

        Level.SetValue(level);
        Experience.SetValue(0);
        ExpForNextLevel.SetValue(GetExpForNextLevel());

        BaseTotalMana.SetValue(30 + Random.Range(MaxManaGainPerLevelRange.x, MaxManaGainPerLevelRange.y) * level);

        int totalPointsLeft = level;
        int powerLevelBonus = Random.Range(0, totalPointsLeft + 1);
        totalPointsLeft -= powerLevelBonus;
        int armorLevelBonus = Random.Range(0, totalPointsLeft + 1);
        totalPointsLeft -= armorLevelBonus;
        // BasePower.SetValue(5 + powerLevelBonus);
        BaseArmor.SetValue(0 + armorLevelBonus);
        BaseSpeed.SetValue(3 + totalPointsLeft);

        CreateStats();

        Items = new();
        Abilities = new();

        // TODO: something smarter maybe the higher level the better army too?        
        CreatureArmy = new();
        int armyCount = Random.Range(Level.Value + 1, Level.Value + 4);
        for (int i = 0; i < armyCount; i++)
        {
            Creature instance = Instantiate(_gameManager.HeroDatabase.GetRandomCreature());
            CreatureArmy.Add(instance);
        }

        UpdateRank();
    }

    public void LoadFromData(HeroData data)
    {
        _gameManager = GameManager.Instance;

        HeroDatabase heroDatabase = _gameManager.HeroDatabase;

        name = data.EntityName;

        Id = data.Id;
        EntityName = data.EntityName;
        Portrait = heroDatabase.GetPortraitById(data.Portrait);

        Element = heroDatabase.GetElementByName(Enum.Parse<ElementName>(data.Element));

        CreateBaseStats();

        Level.SetValue(data.Level);
        Experience.SetValue(data.Experience);
        ExpForNextLevel.SetValue(GetExpForNextLevel());

        BaseTotalMana.SetValue(data.BaseMana);
        // BasePower.SetValue(data.BasePower);
        BaseArmor.SetValue(data.BaseArmor);
        BaseSpeed.SetValue(data.BaseSpeed);

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

    new public HeroData SerializeSelf()
    {
        HeroData data = new()
        {
            Id = Id,
            EntityName = EntityName,
            Portrait = Portrait.Id,

            Element = Element.ElementName.ToString(),

            Level = Level.Value,
            Experience = Experience.Value,

            BaseMana = BaseTotalMana.Value,
            // BasePower = BasePower.Value,
            BaseArmor = BaseArmor.Value,
            BaseSpeed = BaseSpeed.Value
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
}

[Serializable]
public struct HeroData
{
    public string Id;
    public string EntityName;
    public string Portrait;
    public string Element;

    public int Level;
    public int Experience;

    public int BaseMana;
    public int BasePower;
    public int BaseArmor;
    public int BaseSpeed;

    public List<AbilityData> AbilityData;
    public List<string> ItemIds;

    public List<CreatureData> CreatureDatas;
}
