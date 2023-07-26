using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Random = UnityEngine.Random;

[CreateAssetMenu(menuName = "ScriptableObject/Hero/Player")]
public class Hero : BaseScriptableObject
{
    static Vector2Int MaxManaGainPerLevelRange = new(5, 15);

    GameManager _gameManager;

    public string HeroName = "Default";
    public HeroPortrait Portrait;

    [Header("Stats")]
    public IntVariable Level;
    public IntVariable Experience;
    public IntVariable ExpForNextLevel;

    bool _levelUpReady;
    public int LeftoverExp;

    public Element Element;

    public IntVariable BaseMana;
    public IntVariable BasePower;
    public IntVariable BaseArmor;
    public IntVariable BaseSpeed;

    public Stat Mana { get; private set; }
    public Stat Power { get; private set; }
    public Stat Armor { get; private set; }
    public Stat Speed { get; private set; }

    public HeroRank Rank { get; private set; }

    [Header("Items")]
    public List<Item> Items = new();

    [Header("Abilities")]
    public List<Ability> Abilities = new();

    public List<Creature> CreatureArmy = new();
    public List<Minion> MinionArmy = new();

    public event Action OnLevelUpReady;
    public event Action OnLevelUp;
    public event Action<Ability> OnAbilityAdded;
    public event Action<Ability> OnAbilityRemoved;
    public event Action<Item> OnItemAdded;
    public event Action<HeroRank> OnRankChanged;
    public event Action<Creature> OnCreatureAdded;
    public event Action<Creature> OnCreatureRemoved;

    public IntVariable CurrentMana;
    public void BattleInitialize()
    {
        CurrentMana = ScriptableObject.CreateInstance<IntVariable>();
        CurrentMana.SetValue(BaseMana.Value);
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

    public int GetExpForNextLevel()
    {
        // TODO: math
        float exponent = 3f;
        float multiplier = 0.8f;
        int baseExp = 100;

        int result = Mathf.FloorToInt((multiplier * Mathf.Pow(Level.Value, exponent)));
        result = Mathf.RoundToInt(result * 0.1f) * 10; // rounding to tens
        return result + baseExp;
    }

    public virtual void AddExp(int gain)
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

    public void LevelUp()
    {
        _levelUpReady = false;
        Experience.SetValue(0);
        ExpForNextLevel.SetValue(GetExpForNextLevel());

        Level.ApplyChange(1);
        BaseMana.ApplyChange(Random.Range(MaxManaGainPerLevelRange.x, MaxManaGainPerLevelRange.y));
        OnLevelUp?.Invoke();
        UpdateRank();
    }

    public void AddPower()
    {
        BasePower.ApplyChange(1);
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
        UpdateStat(item.InfluencedStat, item.Value);
        UpdateRank();
        OnItemAdded?.Invoke(item);
    }

    public void RemoveItem(Item item)
    {
        Items.Remove(item);
        UpdateStat(item.InfluencedStat, -item.Value);
        UpdateRank();
    }

    void UpdateStat(StatType type, int value)
    {
        // TODO: this if statement sucks.
        if (type == StatType.Mana)
            Mana.ApplyBonusValueChange(value);
        if (type == StatType.Power)
            Power.ApplyBonusValueChange(value);
        if (type == StatType.Armor)
            Armor.ApplyBonusValueChange(value);
        if (type == StatType.Speed)
            Speed.ApplyBonusValueChange(value);
    }

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
        Level = ScriptableObject.CreateInstance<IntVariable>();
        Experience = ScriptableObject.CreateInstance<IntVariable>();
        ExpForNextLevel = ScriptableObject.CreateInstance<IntVariable>();
        BaseMana = ScriptableObject.CreateInstance<IntVariable>();
        BasePower = ScriptableObject.CreateInstance<IntVariable>();
        BaseArmor = ScriptableObject.CreateInstance<IntVariable>();
        BaseSpeed = ScriptableObject.CreateInstance<IntVariable>();
    }

    void CreateStats()
    {
        Mana = ScriptableObject.CreateInstance<Stat>();
        Mana.StatType = StatType.Mana;
        Mana.SetBaseValue(BaseMana.Value);
        BaseMana.OnValueChanged += Mana.SetBaseValue;

        Power = ScriptableObject.CreateInstance<Stat>();
        Power.StatType = StatType.Power;
        Power.SetBaseValue(BasePower.Value);
        BasePower.OnValueChanged += Power.SetBaseValue;

        Armor = ScriptableObject.CreateInstance<Stat>();
        Armor.StatType = StatType.Armor;
        Armor.SetBaseValue(BaseArmor.Value);
        BaseArmor.OnValueChanged += Armor.SetBaseValue;

        Speed = ScriptableObject.CreateInstance<Stat>();
        Speed.StatType = StatType.Speed;
        Speed.SetBaseValue(BaseSpeed.Value);
        BaseSpeed.OnValueChanged += Speed.SetBaseValue;
    }

    public void CreateFromHeroCreation(string heroName, HeroPortrait portrait, Element element)
    {
        _gameManager = GameManager.Instance;

        name = heroName;
        Id = Guid.NewGuid().ToString();

        HeroName = heroName;
        Portrait = portrait;
        Element = element;

        CreateBaseStats();

        Level.SetValue(1);
        Experience.SetValue(0);
        ExpForNextLevel.SetValue(GetExpForNextLevel());

        BaseMana.SetValue(30);

        BasePower.SetValue(5);
        BaseArmor.SetValue(0);
        BaseSpeed.SetValue(3);

        CreateStats();

        Items = new();

        Abilities = new();
        AddAbility(_gameManager.HeroDatabase.GetStartingAbility(element));

        UpdateRank();

        CreatureArmy = new();
        foreach (Creature c in _gameManager.HeroDatabase.GetStartingArmy(element).Creatures)
        {
            Creature instance = Instantiate(c);
            CreatureArmy.Add(instance);
        }
    }

    public void CreateRandom(int level)
    {
        _gameManager = GameManager.Instance;

        GameDatabase gameDatabase = _gameManager.GameDatabase;
        HeroDatabase heroDatabase = _gameManager.HeroDatabase;
        bool isMale = Random.value > 0.5f;

        Id = Guid.NewGuid().ToString();

        HeroName = isMale ? heroDatabase.GetRandomNameMale() : heroDatabase.GetRandomNameFemale();
        name = HeroName;
        Portrait = isMale ? heroDatabase.GetRandomPortraitMale() : heroDatabase.GetRandomPortraitFemale();

        Element = heroDatabase.GetRandomElement();

        CreateBaseStats();

        Level.SetValue(level);
        Experience.SetValue(0);
        ExpForNextLevel.SetValue(GetExpForNextLevel());

        BaseMana.SetValue(30 + Random.Range(MaxManaGainPerLevelRange.x, MaxManaGainPerLevelRange.y) * level);

        int totalPointsLeft = level;
        int powerLevelBonus = Random.Range(0, totalPointsLeft + 1);
        totalPointsLeft -= powerLevelBonus;
        int armorLevelBonus = Random.Range(0, totalPointsLeft + 1);
        totalPointsLeft -= armorLevelBonus;
        BasePower.SetValue(5 + powerLevelBonus);
        BaseArmor.SetValue(0 + armorLevelBonus);
        BaseSpeed.SetValue(3 + totalPointsLeft);

        CreateStats();

        List<Item> Items = new();
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

        GameDatabase gameDatabase = _gameManager.GameDatabase;
        HeroDatabase heroDatabase = _gameManager.HeroDatabase;

        name = data.HeroName;

        Id = data.Id;
        HeroName = data.HeroName;
        Portrait = heroDatabase.GetPortraitById(data.Portrait);

        Element = heroDatabase.GetElementByName(Enum.Parse<ElementName>(data.Element));

        CreateBaseStats();

        Level.SetValue(data.Level);
        Experience.SetValue(data.Experience);
        ExpForNextLevel.SetValue(GetExpForNextLevel());

        BaseMana.SetValue(data.BaseMana);
        BasePower.SetValue(data.BasePower);
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

    public HeroData SerializeSelf()
    {
        HeroData data = new();
        data.Id = Id;
        data.HeroName = HeroName;
        data.Portrait = Portrait.Id;

        data.Element = Element.ElementName.ToString();

        data.Level = Level.Value;
        data.Experience = Experience.Value;

        data.BaseMana = BaseMana.Value;
        data.BasePower = BasePower.Value;
        data.BaseArmor = BaseArmor.Value;
        data.BaseSpeed = BaseSpeed.Value;

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

[System.Serializable]
public struct HeroData
{
    public string Id;
    public string HeroName;
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
