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
    public static int MaxCharacterArmySlots = 5;

    static Vector2Int MaxHealthGainPerLevelRange = new(100, 210);
    static Vector2Int MaxManaGainPerLevelRange = new(50, 110);

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
    public List<Injury> Injuries = new();

    public int DayAddedToTroops { get; private set; }
    public Vector2 DeskPosition { get; private set; }
    [field: SerializeField] public IntVariable WeeklyWage { get; private set; }
    public IntVariable NewWage { get; private set; }
    public bool Negotiated { get; private set; }

    public Vector2 MapPosition;

    public List<ArmyGroup> Army = new();

    public event Action<CharacterRank> OnRankChanged;
    public event Action<Element> OnElementChanged;
    public event Action<Injury> OnInjuryAdded;

    public void InitializeSpecialCharacter()
    {
        _gameManager = GameManager.Instance;
        CreateStats();
        UpdateRank();
    }

    public void UpdateDeskPosition(Vector2 newPos)
    {
        DeskPosition = newPos;
        _gameManager.SaveJsonData();
    }

    public void AddArmy(ArmyGroup armyGroup)
    {
        Debug.Log($"Character {name} adds army {armyGroup.ArmyEntity} count {armyGroup.EntityCount}");

        Army.Add(armyGroup);
        _gameManager.SaveJsonData();
    }
    public void RemoveArmy(ArmyGroup armyGroup)
    {
        Debug.Log($"Character {name} removes {armyGroup.ArmyEntity} count {armyGroup.EntityCount}");

        Army.Remove(armyGroup);
        _gameManager.SaveJsonData();
    }

    public int GetTotalNumberOfArmyEntities()
    {
        int total = 0;
        foreach (ArmyGroup ag in Army)
            total += ag.EntityCount;
        return total;
    }

    public void AddInjury(Injury injury)
    {
        if (InjuryDramaCheck())
        {
            Debug.LogWarning($"Trying to add an injury to character ({CharacterName}) with active injury. It is not supported.");
            return;
        }
        injury.DateTimeStarted = _gameManager.GetCurrentDateTime();

        Injuries.Add(injury);
        OnInjuryAdded?.Invoke(injury);
    }

    public bool IsUnavailable()
    {
        foreach (Injury i in Injuries)
            if (i.IsHealed == false)
                return true;
        return false;
    }

    //TODO: I am currently not handling multiple active injuries
    public bool InjuryDramaCheck()
    {
        int numberOfUnhealedInjuries = 0;

        foreach (Injury i in Injuries)
            if (i.IsHealed == false)
                numberOfUnhealedInjuries++;

        if (numberOfUnhealedInjuries > 0)
        {
            Debug.Log($"Number of active injuries: {numberOfUnhealedInjuries}");
            return true;
        }

        return false;
    }

    public Injury GetActiveInjury()
    {
        foreach (Injury i in Injuries)
            if (i.IsHealed == false)
                return i;
        return null;
    }

    public void SetDayAddedToTroops(int day) { DayAddedToTroops = day; }

    public virtual void GetExp(int gain)
    {
        int g = Mathf.Clamp(gain, 0, 100);
        Experience.ApplyChange(g);
        if (Experience.Value < 100)
            return;
    }

    public void LevelUp()
    {
        Level.ApplyChange(1);
        Experience.SetValue(0);

        BaseHealth.ApplyChange(Random.Range(MaxHealthGainPerLevelRange.x, MaxHealthGainPerLevelRange.y));
        BaseMana.ApplyChange(Random.Range(MaxManaGainPerLevelRange.x, MaxManaGainPerLevelRange.y));

        AudioManager.Instance.PlaySFX("LevelUp", Vector3.one);

        UpdateRank();
    }

    public void AddPower() { BasePower.ApplyChange(100); }

    public void AddArmor() { BaseArmor.ApplyChange(100); }

    public void AddRange() { BaseSpeed.ApplyChange(100); }

    public bool CanTakeAnotherAbility() { return Abilities.Count < MaxCharacterAbilities; }

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
        WeeklyWage.SetValue(wage);
        Negotiated = false;
    }

    public void RaiseCheck()
    {
        if (!IsAskingForRaise())
            return;

        _gameManager.RemoveCharacterFromTroops(this);

        DateTime expiry = ScriptableObject.CreateInstance<DateTime>();
        expiry.Day = _gameManager.Day + 1;

        Report r = ScriptableObject.CreateInstance<Report>();
        r.Initialize(ReportType.RaiseRequest, character: this, expiryDateTime: expiry);
        _gameManager.AddNewReport(r);
    }

    public bool IsAskingForRaise()
    {
        if (WeeklyWage.Value / Level.Value >= 150)
            return false;
        if (Random.value > (_gameManager.Day - DayAddedToTroops) * 0.1f)
            return false;
        if (Random.value > 0.5f)
            return false;
        NewWage.SetValue(GetRequestedWage());
        if (NewWage.Value < WeeklyWage.Value)
            return false;
        return true;
    }

    public void SetNegotiated(bool has) { Negotiated = has; }

    public void SetNewWage(int newWage) { NewWage.SetValue(newWage); }

    public int GetRequestedWage() { return Random.Range(100, 200) * Level.Value; }

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
        BaseHealth.OnValueChanged += Health.SetBaseValue;

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

        WeeklyWage = ScriptableObject.CreateInstance<IntVariable>();
        NewWage = ScriptableObject.CreateInstance<IntVariable>();
    }

    public void CreateFromCharacterCreation(string name, CharacterPortrait portrait, Vector2 mapPosition)
    {
        _gameManager = GameManager.Instance;
        Id = Guid.NewGuid().ToString();
        CharacterName = name;
        Portrait = portrait;

        CreateBaseStats();

        Level.SetValue(1);
        Experience.SetValue(0);
        Element = _gameManager.GameDatabase.GetElementByName(ElementName.Earth);

        BaseHealth.SetValue(1000);
        BaseMana.SetValue(300);

        BasePower.SetValue(500);
        BaseArmor.SetValue(0);
        BaseSpeed.SetValue(300);

        CreateStats();

        Items = new();
        Abilities = new();

        UpdateRank();
        WeeklyWage.SetValue(0);

        MapPosition = mapPosition;
        Army = new();
        foreach (ArmyGroup ag in _gameManager.GameDatabase.BasicArmy)
        {
            ArmyGroup instance = Instantiate(ag);
            Army.Add(instance);
        }
    }

    public void CreateRandom(int level)
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
        BasePower.SetValue(500 + powerLevelBonus * 100);
        BaseArmor.SetValue(0 + armorLevelBonus * 100);
        BaseSpeed.SetValue(300 + totalPointsLeft * 100);

        CreateStats();

        List<Item> Items = new();
        Abilities = new();

        UpdateRank();

        WeeklyWage.SetValue(Random.Range(100, 200) * Level.Value);
    }

    public void CreateFromData(CharacterData data)
    {
        _gameManager = GameManager.Instance;

        GameDatabase gameDatabase = _gameManager.GameDatabase;
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
        Injuries = new();
        foreach (var i in data.InjuryData)
        {
            Injury instance = Instantiate(gameDatabase.GetInjuryById(i.Id));
            instance.name = Helpers.ParseScriptableObjectCloneName(instance.name);
            instance.CreateFromData(i);
            Injuries.Add(instance);
        }
        DayAddedToTroops = data.DayAddedToTroops;
        DeskPosition = data.DeskPosition;

        WeeklyWage.SetValue(data.WeeklyWage);
        NewWage.SetValue(data.NewWage);
        Negotiated = data.Negotiated;
        MapPosition = data.MapPosition;
        Army = new();
        foreach (ArmyGroupData d in data.ArmyGroupDatas)
        {
            ArmyGroup ag = CreateInstance<ArmyGroup>();
            ag.LoadFromData(d);
            Army.Add(ag);
        }

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
        data.InjuryData = new();
        foreach (Injury i in Injuries)
            data.InjuryData.Add(i.SerializeSelf());

        data.DayAddedToTroops = DayAddedToTroops;
        data.DeskPosition = DeskPosition;
        data.WeeklyWage = WeeklyWage.Value;
        data.NewWage = NewWage.Value;
        data.Negotiated = Negotiated;
        data.MapPosition = MapPosition;
        data.ArmyGroupDatas = new();
        foreach (ArmyGroup ag in Army)
            data.ArmyGroupDatas.Add(ag.SerializeSelf());

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
    public List<InjuryData> InjuryData;

    public int DayAddedToTroops;
    public Vector2 DeskPosition;
    public int WeeklyWage;
    public int NewWage;
    public bool Negotiated;

    public Vector2 MapPosition;

    public List<ArmyGroupData> ArmyGroupDatas;
}
