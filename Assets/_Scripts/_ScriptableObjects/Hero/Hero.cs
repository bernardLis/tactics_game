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
    public int LevelUpPointsLeft;

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

    [Header("Quest")]
    public List<Injury> Injuries = new();

    public int DayAddedToTroops { get; private set; }

    public List<ArmyGroup> Army = new();


    public event Action<Item> OnItemAdded;
    public event Action<HeroRank> OnRankChanged;
    public event Action<Injury> OnInjuryAdded;
    public event Action<ArmyGroup> OnArmyAdded;

    public IntVariable CurrentMana;
    public void BattleInitialize()
    {
        CurrentMana = ScriptableObject.CreateInstance<IntVariable>();
        CurrentMana.SetValue(BaseMana.Value);
    }

    public void AddNewArmy(ArmyGroup armyGroup)
    {
        Debug.Log($"Hero {name} adds army {armyGroup.Creature} count {armyGroup.NumberOfUnits}");
        // join armies of the same type
        foreach (ArmyGroup ag in Army)
        {
            if (ag.Creature == armyGroup.Creature)
            {
                ag.ChangeCount(armyGroup.NumberOfUnits);
                return;
            }
        }

        Army.Add(armyGroup);
        OnArmyAdded?.Invoke(armyGroup);
    }

    public void TryJoiningArmies()
    {
        List<ArmyGroup> toRemove = new();
        Dictionary<Creature, int> armyDict = new();
        for (int i = 0; i < Army.Count; i++)
        {
            if (armyDict.ContainsKey(Army[i].Creature))
            {
                int index = armyDict[Army[i].Creature];
                Army[index].JoinArmy(Army[i]);
                toRemove.Add(Army[i]);
                continue;
            }
            else
                armyDict.Add(Army[i].Creature, i);
        }

        foreach (ArmyGroup ag in toRemove)
            RemoveArmy(ag);
    }

    public void RemoveArmy(ArmyGroup armyGroup)
    {
        Debug.Log($"Hero {name} removes {armyGroup.Creature} count {armyGroup.NumberOfUnits}");

        Army.Remove(armyGroup);
    }

    public int GetTotalNumberOfArmyEntities()
    {
        int total = 0;
        foreach (ArmyGroup ag in Army)
            total += ag.NumberOfUnits;
        return total;
    }

    public void AddInjury(Injury injury)
    {
        if (InjuryDramaCheck())
        {
            Debug.LogWarning($"Trying to add an injury to hero ({HeroName}) with active injury. It is not supported.");
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

    public int GetExpForNextLevel()
    {
        float exponent = 3f;
        float multiplier = 0.8f;
        int baseExp = 100;

        int result = Mathf.FloorToInt((multiplier * Mathf.Pow(Level.Value, exponent)));
        result = Mathf.RoundToInt(result * 0.1f) * 10; // rounding to tens
        return result + baseExp;
    }

    public virtual void GetExp(int gain)
    {
        Experience.ApplyChange(gain);

        if (Experience.Value < ExpForNextLevel.Value)
            return;
        LevelUp();
    }

    public void LevelUp()
    {
        Level.ApplyChange(1);
        Experience.SetValue(0);
        ExpForNextLevel.SetValue(GetExpForNextLevel());

        BaseMana.ApplyChange(Random.Range(MaxManaGainPerLevelRange.x, MaxManaGainPerLevelRange.y));
        LevelUpPointsLeft += 1;

        AudioManager.Instance.PlayUI("Level Up");

        UpdateRank();
    }

    public void AddPower()
    {
        if (LevelUpPointsLeft <= 0)
            return;

        BasePower.ApplyChange(1);
        LevelUpPointsLeft--;
    }

    public void AddArmor()
    {
        if (LevelUpPointsLeft <= 0)
            return;

        BaseArmor.ApplyChange(1);
        LevelUpPointsLeft--;
    }

    public void AddSpeed()
    {
        if (LevelUpPointsLeft <= 0)
            return;

        BaseSpeed.ApplyChange(1);
        LevelUpPointsLeft--;
    }

    public void AddAbility(Ability ability)
    {
        Abilities.Add(ability);
        UpdateRank();
    }

    public void RemoveAbility(Ability ability)
    {
        Abilities.Remove(ability);
        UpdateRank();
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

    public void CreateFromHeroCreation(string name, HeroPortrait portrait, Element element)
    {
        _gameManager = GameManager.Instance;
        Id = Guid.NewGuid().ToString();
        HeroName = name;
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
        Ability a = Instantiate(_gameManager.HeroDatabase.GetStartingAbility(element));
        Abilities.Add(a);

        UpdateRank();

        Army = new();
        foreach (ArmyGroup ag in _gameManager.HeroDatabase.GetStartingArmy(element).ArmyGroups)
        {
            ArmyGroup instance = Instantiate(ag);
            Army.Add(instance);
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
        Army = new();
        int armyCount = Random.Range(1, _gameManager.HeroDatabase.AllArmyEntities.Count);
        for (int i = 0; i < armyCount; i++)
        {
            Army.Add(ScriptableObject.CreateInstance<ArmyGroup>());
            Army[i].Creature = _gameManager.HeroDatabase.GetRandomArmyEntity();

            // TODO: needs balance
            int count = Mathf.RoundToInt(Random.Range(1, 10) * 0.2f * level);
            count = Mathf.Clamp(count, 1, 100);

            Army[i].NumberOfUnits = count;
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

        Injuries = new();
        foreach (var i in data.InjuryData)
        {
            Injury instance = Instantiate(heroDatabase.GetInjuryById(i.Id));
            instance.name = Helpers.ParseScriptableObjectCloneName(instance.name);
            instance.CreateFromData(i);
            Injuries.Add(instance);
        }
        DayAddedToTroops = data.DayAddedToTroops;

        Army = new();
        foreach (ArmyGroupData d in data.ArmyGroupDatas)
        {
            ArmyGroup ag = CreateInstance<ArmyGroup>();
            ag.LoadFromData(d);
            Army.Add(ag);
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

        data.InjuryData = new();
        foreach (Injury i in Injuries)
            data.InjuryData.Add(i.SerializeSelf());

        data.DayAddedToTroops = DayAddedToTroops;

        data.ArmyGroupDatas = new();
        foreach (ArmyGroup ag in Army)
            data.ArmyGroupDatas.Add(ag.SerializeSelf());

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

    public List<InjuryData> InjuryData;

    public int DayAddedToTroops;

    public List<ArmyGroupData> ArmyGroupDatas;
}
