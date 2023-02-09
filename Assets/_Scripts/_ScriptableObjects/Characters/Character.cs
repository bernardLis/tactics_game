using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Random = UnityEngine.Random;



[CreateAssetMenu(menuName = "ScriptableObject/Character/Player")]
public class Character : BaseScriptableObject
{
    public static int MaxCharacterAbilities = 1;
    public static int MaxCharacterItems = 2;
    static Vector2Int MaxHealthGainPerLevelRange = new(10, 21);
    static Vector2Int MaxManaGainPerLevelRange = new(5, 11);

    GameManager _gameManager;

    public string CharacterName = "Default";
    public CharacterPortrait Portrait;

    [Header("Stats")]
    public IntVariable Level;
    public IntVariable Experience;

    public IntVariable BaseHealth;
    public IntVariable BaseMana;
    public IntVariable BasePower;
    public IntVariable BaseArmor;
    public IntVariable BaseSpeed;

    public Stat Health { get; private set; }
    public Stat Mana { get; private set; }
    public Stat Power { get; private set; }
    public Stat Armor { get; private set; }
    public Stat Speed { get; private set; }

    public Element Element;
    public CharacterRank Rank { get; private set; }

    [Header("Items")]
    public List<Item> Items = new();

    [Header("Abilities")]
    public List<Ability> Abilities = new();

    [Header("Quest")]
    [HideInInspector] public bool IsAssigned;
    [HideInInspector] public bool IsUnavailable;
    [HideInInspector] public int DayStartedBeingUnavailable;
    [HideInInspector] public int UnavailabilityDuration;

    public int DayAddedToTroops { get; private set; }
    public Vector2 DeskPosition { get; private set; }
    public int WeeklyWage { get; private set; }
    public int NewWage { get; private set; }
    public bool Negotiated { get; private set; }

    public event Action<CharacterRank> OnRankChanged;
    public event Action<Element> OnElementChanged;
    public event Action<int> OnWageChanged;

    public void GetExp(int gain) { BaseExpGain(gain); }

    protected virtual void BaseExpGain(int gain)
    {
        Experience.ApplyChange(gain);
        if (Experience.Value < 100)
            return;

        LevelUp();
    }

    public void LevelUp()
    {
        Experience.SetValue(0);
        Level.ApplyChange(1);

        BaseHealth.ApplyChange(Random.Range(MaxHealthGainPerLevelRange.x, MaxHealthGainPerLevelRange.y));
        BaseMana.ApplyChange(Random.Range(MaxManaGainPerLevelRange.x, MaxManaGainPerLevelRange.y));

        AudioManager.Instance.PlaySFX("LevelUp", Vector3.one);

        UpdateRank();
    }

    public void AddPower() { BasePower.ApplyChange(1); }

    public void AddArmor() { BaseArmor.ApplyChange(1); }

    public void AddRange() { BaseSpeed.ApplyChange(1); }

    public void AddAbility(Ability ability)
    {
        Abilities.Add(ability);
        UpdateRank();
        UpdateElement(ability.Element);
    }

    public void RemoveAbility(Ability ability)
    {
        Abilities.Remove(ability);
        UpdateRank();
        UpdateElement(_gameManager.GameDatabase.GetElementByName(ElementName.Earth));
    }

    void UpdateElement(Element element)
    {
        if (element == Element)
            return;
        Element = element;
        OnElementChanged?.Invoke(element);
    }

    public bool CanTakeAnotherItem() { return Items.Count < MaxCharacterItems; }

    public bool CanTakeAnotherAbility() { return Abilities.Count < MaxCharacterAbilities; }

    public void AddItem(Item item)
    {
        Items.Add(item);
        UpdateStat(item.InfluencedStat, item.Value);
        UpdateRank();
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
        if (type == StatType.Health)
            Health.ApplyBonusValueChange(value);
        if (type == StatType.Mana)
            Mana.ApplyBonusValueChange(value);
        if (type == StatType.Power)
            Power.ApplyBonusValueChange(value);
        if (type == StatType.Armor)
            Armor.ApplyBonusValueChange(value);
        if (type == StatType.Speed)
            Speed.ApplyBonusValueChange(value);
    }

    public void SetUnavailable(int days)
    {
        _gameManager = GameManager.Instance;

        IsUnavailable = true;
        DayStartedBeingUnavailable = _gameManager.Day;
        UnavailabilityDuration = days;
    }

    public void OnDayPassed(int day)
    {
        if (!IsUnavailable)
            return;

        UnavailabilityDuration--;
        if (UnavailabilityDuration <= 0)
            IsUnavailable = false;
    }

    public void UpdateRank()
    {
        int points = CountRankPoints();
        CharacterRank newRank = _gameManager.GameDatabase.CharacterDatabase.GetRankByPoints(points);
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

        foreach (Ability a in Abilities)
            total += a.StarRank;

        return total;
    }

    public void SetWeeklyWage(int wage)
    {
        WeeklyWage = wage;
        Negotiated = false;
        OnWageChanged?.Invoke(wage);
    }

    public void RaiseCheck()
    {
        if (!IsAskingForRaise())
            return;

        _gameManager.RemoveCharacterFromTroops(this);
        Report r = ScriptableObject.CreateInstance<Report>();
        r.Initialize(ReportType.RaiseRequest, null, null, null, null, null, null, this);
        _gameManager.AddNewReport(r);
    }

    public bool IsAskingForRaise()
    {
        if (WeeklyWage / Level.Value >= 150)
            return false;
        if (Random.value > (_gameManager.Day - DayAddedToTroops) * 0.1f)
            return false;
        if (Random.value > 0.5f)
            return false;
        NewWage = GetRequestedWage();
        if (NewWage < WeeklyWage)
            return false;
        return true;
    }

    public void SetNegotiated(bool has) { Negotiated = has; }

    public void SetNewWage(int newWage) { NewWage = newWage; }

    public int GetRequestedWage() { return Random.Range(100, 200) * Level.Value; }

    public void InitializeStarterTroops()
    {
        Debug.Log($"Character {name} (starter troop) is initialized.");
        _gameManager = GameManager.Instance;
        DayAddedToTroops = 0;
        CreateStats();
        UpdateRank();
    }

    public void SetDayAddedToTroops(int day) { DayAddedToTroops = day; }

    public void UpdateDeskPosition(Vector2 newPos)
    {
        DeskPosition = newPos;
        _gameManager.SaveJsonData();
    }

    void CreateBaseStats()
    {
        Level = ScriptableObject.CreateInstance<IntVariable>();
        Experience = ScriptableObject.CreateInstance<IntVariable>();
        BaseHealth = ScriptableObject.CreateInstance<IntVariable>();
        BaseMana = ScriptableObject.CreateInstance<IntVariable>();
        BasePower = ScriptableObject.CreateInstance<IntVariable>();
        BaseArmor = ScriptableObject.CreateInstance<IntVariable>();
        BaseSpeed = ScriptableObject.CreateInstance<IntVariable>();
    }

    void CreateStats()
    {
        Health = ScriptableObject.CreateInstance<Stat>();
        Health.StatType = StatType.Health;
        Health.SetBaseValue(BaseHealth.Value);
        BaseHealth.OnValueChanged += Health.ApplyBaseValueChange;

        Mana = ScriptableObject.CreateInstance<Stat>();
        Mana.StatType = StatType.Mana;
        Mana.SetBaseValue(BaseMana.Value);
        BaseMana.OnValueChanged += Mana.ApplyBaseValueChange;

        Power = ScriptableObject.CreateInstance<Stat>();
        Power.StatType = StatType.Power;
        Power.SetBaseValue(BasePower.Value);
        BasePower.OnValueChanged += Power.ApplyBaseValueChange;

        Armor = ScriptableObject.CreateInstance<Stat>();
        Armor.StatType = StatType.Armor;
        Armor.SetBaseValue(BaseArmor.Value);
        BaseArmor.OnValueChanged += Armor.ApplyBaseValueChange;

        Speed = ScriptableObject.CreateInstance<Stat>();
        Speed.StatType = StatType.Speed;
        Speed.SetBaseValue(BaseSpeed.Value);
        BaseSpeed.OnValueChanged += Speed.ApplyBaseValueChange;
    }

    public virtual void CreateRandom(int level)
    {
        _gameManager = GameManager.Instance;

        GameDatabase gameDatabase = _gameManager.GameDatabase;
        CharacterDatabase characterDatabase = gameDatabase.CharacterDatabase;
        bool isMale = Random.value > 0.5f;

        Id = Guid.NewGuid().ToString();

        CharacterName = isMale ? characterDatabase.GetRandomNameMale() : characterDatabase.GetRandomNameFemale();
        name = CharacterName;
        Portrait = isMale ? characterDatabase.GetRandomPortraitMale() : characterDatabase.GetRandomPortraitFemale();

        CreateBaseStats();

        Level.SetValue(level);
        Experience.SetValue(0);
        Element = _gameManager.GameDatabase.GetElementByName(ElementName.Earth);

        BaseHealth.SetValue(100 + Random.Range(MaxHealthGainPerLevelRange.x, MaxHealthGainPerLevelRange.y) * level);
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

        UpdateRank();

        WeeklyWage = Random.Range(100, 200) * Level.Value;
    }

    public virtual void CreateFromData(CharacterData data)
    {
        _gameManager = GameManager.Instance;

        GameDatabase gameDatabase = _gameManager.GameDatabase;
        _gameManager.OnDayPassed += OnDayPassed;
        name = data.CharacterName;

        Id = data.Id;
        CharacterName = data.CharacterName;
        Portrait = gameDatabase.CharacterDatabase.GetPortraitById(data.Portrait);

        CreateBaseStats();

        Level.SetValue(data.Level);
        Experience.SetValue(data.Experience);
        Element = _gameManager.GameDatabase.GetElementByName((ElementName)System.Enum.Parse(typeof(ElementName), data.Element));

        BaseHealth.SetValue(data.BaseHealth);
        BaseMana.SetValue(data.BaseMana);
        BasePower.SetValue(data.BasePower);
        BaseArmor.SetValue(data.BaseArmor);
        BaseSpeed.SetValue(data.BaseSpeed);
        CreateStats();

        foreach (AbilityData abilityData in data.AbilityData)
        {
            Ability a = Instantiate(gameDatabase.GetAbilityById(abilityData.TemplateId));
            a.name = abilityData.Name;
            Abilities.Add(a);
        }

        foreach (string id in data.ItemIds)
            AddItem(gameDatabase.GetItemById(id));

        IsAssigned = data.IsAssigned;
        IsUnavailable = data.IsOnUnavailable;
        DayStartedBeingUnavailable = data.DayStartedBeingUnavailable;
        UnavailabilityDuration = data.UnavailabilityDuration;

        DayAddedToTroops = data.DayAddedToTroops;
        DeskPosition = data.DeskPosition;
        WeeklyWage = data.WeeklyWage;
        NewWage = data.NewWage;
        Negotiated = data.Negotiated;

        UpdateRank();
        UpdateElement(Element);
    }

    public CharacterData SerializeSelf()
    {
        CharacterData data = new();
        data.Id = Id;
        data.CharacterName = CharacterName;
        data.Portrait = Portrait.Id;
        data.Level = Level.Value;
        data.Experience = Experience.Value;
        data.Element = Element.ElementName.ToString();

        data.BaseHealth = BaseHealth.Value;
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

        data.IsAssigned = IsAssigned;
        data.IsOnUnavailable = IsUnavailable;
        data.DayStartedBeingUnavailable = DayStartedBeingUnavailable;
        data.UnavailabilityDuration = UnavailabilityDuration;

        data.DayAddedToTroops = DayAddedToTroops;
        data.DeskPosition = DeskPosition;
        data.WeeklyWage = WeeklyWage;
        data.NewWage = NewWage;
        data.Negotiated = Negotiated;

        return data;
    }
}

[System.Serializable]
public struct CharacterData
{
    public string Id;
    public string CharacterName;
    public string Portrait;
    public int Level;
    public int Experience;
    public string Element;

    public int BaseHealth;
    public int BaseMana;
    public int BasePower;
    public int BaseArmor;
    public int BaseSpeed;

    public List<AbilityData> AbilityData;
    public List<string> ItemIds;

    public bool IsAssigned;
    public bool IsOnUnavailable;
    public int DayStartedBeingUnavailable;
    public int UnavailabilityDuration;

    public int DayAddedToTroops;
    public Vector2 DeskPosition;
    public int WeeklyWage;
    public int NewWage;
    public bool Negotiated;
}
