using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Random = UnityEngine.Random;

[CreateAssetMenu(menuName = "ScriptableObject/Character/Player")]
public class Character : BaseScriptableObject
{
    GameManager _gameManager;

    // character scriptable object holds stats & abilities of a character.
    // it passes these values to CharacterStats script where they can be used in game.
    public string CharacterName = "Default";
    public CharacterPortrait Portrait;

    [Header("Stats")]
    public int Level;
    public int Experience;

    public int MaxHealth;
    public int MaxMana;
    public int Power;
    public int Armor;
    public int MovementRange;

    // set by items, for now used only in the shop
    public int PowerBonus;
    public int MaxHealthBonus;
    public int MaxManaBonus;
    public int ArmorBonus;
    public int MovementRangeBonus;

    [Header("Equipment")]
    public Equipment Body;
    public Weapon Weapon;
    public List<Item> Items = new();

    [Header("Abilities")]
    public List<Ability> Abilities = new();

    public CharacterRank Rank { get; private set; }

    [Header("Quest")]
    [HideInInspector] public bool IsAssigned;
    [HideInInspector] public bool IsUnavailable;
    [HideInInspector] public int DayStartedBeingUnavailable;
    [HideInInspector] public int UnavailabilityDuration;

    public event Action OnCharacterLevelUp;
    public event Action<int> OnCharacterExpGain;
    public event Action<CharacterRank> OnRankChanged;

    public event Action<int> OnMaxHealthChanged;
    public event Action<int> OnMaxManaChanged;
    public event Action<int> OnPowerChanged;
    public event Action<int> OnArmorChanged;
    public event Action<int> OnMovementRangeChanged;

    public virtual void Initialize(GameObject obj)
    {
        Transform bodyObj = obj.transform.Find("Body");
        if (Body != null)
            Body.Initialize(bodyObj.gameObject);

        Transform weaponObj = bodyObj.transform.Find("Weapon");
        if (Weapon != null)
            Weapon.Initialize(weaponObj.gameObject);
    }

    public void ChangeStat(string stat, int value)
    {
        if (stat == "MaxHealth")
            MaxHealth += value;
        if (stat == "MaxMana")
            MaxMana += value;
        if (stat == "Power")
            Power += value;
        if (stat == "Armor")
            Armor += value;
        if (stat == "MovementRange")
            MovementRange += value;
    }

    public int GetStatValue(string stat)
    {
        if (stat == "MaxHealth")
            return MaxHealth + MaxHealthBonus;
        if (stat == "MaxMana")
            return MaxMana + MaxManaBonus;
        if (stat == "Power")
            return Power + PowerBonus;
        if (stat == "Armor")
            return Armor + ArmorBonus;
        if (stat == "MovementRange")
            return MovementRange + MovementRangeBonus;

        return 0;
    }

    public void GetExp(int gain) { BaseExpGain(gain); }

    protected virtual void BaseExpGain(int gain)
    {
        Experience += gain;
        OnCharacterExpGain?.Invoke(gain);

        if (Experience < 100)
            return;

        LevelUp();
    }

    public void LevelUp()
    {
        Experience = 0;
        OnCharacterExpGain?.Invoke(0);

        Level++;
        Power += Random.Range(0, 2);
        AudioManager.Instance.PlaySFX("LevelUp", Vector3.one);

        OnCharacterLevelUp?.Invoke();
        UpdateRank();
    }

    public int GetNumberOfAbilitySlots() { return 2; }

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
        ResolveItems();
        InformSubscribers(item);
        UpdateRank();
    }

    public void RemoveItem(Item item)
    {
        Items.Remove(item);
        ResolveItems();
        InformSubscribers(item);
        UpdateRank();
    }

    void InformSubscribers(Item item)
    {
        if (item.InfluencedStat == StatType.MaxHealth)
            OnMaxHealthChanged?.Invoke(GetStatValue("MaxHealth"));
        if (item.InfluencedStat == StatType.MaxMana)
            OnMaxManaChanged?.Invoke(GetStatValue("MaxMana"));
        if (item.InfluencedStat == StatType.Power)
            OnPowerChanged?.Invoke(GetStatValue("Power"));
        if (item.InfluencedStat == StatType.Armor)
            OnArmorChanged?.Invoke(GetStatValue("Armor"));
        if (item.InfluencedStat == StatType.MovementRange)
            OnMovementRangeChanged?.Invoke(GetStatValue("MovementRange"));
    }

    public void ResolveItems()
    {
        MaxHealthBonus = 0;
        MaxManaBonus = 0;
        PowerBonus = 0;
        ArmorBonus = 0;
        MovementRangeBonus = 0;

        foreach (Item item in Items)
        {
            if (item.InfluencedStat == StatType.MaxHealth)
                MaxHealthBonus += item.Value;
            if (item.InfluencedStat == StatType.MaxMana)
                MaxManaBonus += item.Value;
            if (item.InfluencedStat == StatType.Power)
                PowerBonus += item.Value;
            if (item.InfluencedStat == StatType.Armor)
                ArmorBonus += item.Value;
            if (item.InfluencedStat == StatType.MovementRange)
                MovementRangeBonus += item.Value;
        }

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
        Debug.Log($"points {points} for {name}");
        CharacterRank newRank = _gameManager.GameDatabase.CharacterDatabase.GetRankByPoints(points);
        if (newRank == Rank)
            return;

        Rank = newRank;
        OnRankChanged?.Invoke(Rank);
    }

    public int CountRankPoints()
    {
        int total = Level;

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
            total += a.Rank;

        return total;
    }

    public virtual void CreateRandom()
    {
        _gameManager = GameManager.Instance;

        GameDatabase gameDatabase = _gameManager.GameDatabase;
        CharacterDatabase characterDatabase = gameDatabase.CharacterDatabase;
        bool isMale = Random.value > 0.5f;

        Id = Guid.NewGuid().ToString();

        CharacterName = isMale ? characterDatabase.GetRandomNameMale() : characterDatabase.GetRandomNameFemale();
        name = CharacterName;
        Portrait = isMale ? characterDatabase.GetRandomPortraitMale() : characterDatabase.GetRandomPortraitFemale();

        Level = 1;
        Experience = 0;

        MaxHealth = 100;
        MaxMana = 30;
        Power = 5;
        Armor = 0;
        MovementRange = 3;

        Body = gameDatabase.GetBodyByName("OnePunchMan");
        Weapon = gameDatabase.GetRandomWeapon();
        List<Item> Items = new();

        Abilities = new();

        UpdateRank();
    }

    // creates character at runtime from saved data
    public virtual void CreateFromData(CharacterData data)
    {
        _gameManager = GameManager.Instance;

        GameDatabase gameDatabase = _gameManager.GameDatabase;
        _gameManager.OnDayPassed += OnDayPassed;
        name = data.CharacterName;

        Id = data.Id;
        CharacterName = data.CharacterName;
        Portrait = gameDatabase.CharacterDatabase.GetPortraitById(data.Portrait);

        Level = data.Level;
        Experience = data.Experience;
        Power = data.Power;
        MaxHealth = data.MaxHealth;
        MaxMana = data.MaxMana;
        Armor = data.Armor;
        MovementRange = data.MovementRange;

        Body = gameDatabase.GetBodyByName(data.Body);
        Weapon = gameDatabase.GetWeaponByName(data.Weapon);

        foreach (AbilityData abilityData in data.AbilityData)
        {
            Ability a = Instantiate(gameDatabase.GetAbilityById(abilityData.TemplateId));
            a.name = abilityData.Name;
            Abilities.Add(a);
        }

        foreach (string id in data.ItemReferenceIds)
            Items.Add(gameDatabase.GetItemByReferenceId(id));

        IsAssigned = data.IsAssigned;
        IsUnavailable = data.IsOnUnavailable;
        DayStartedBeingUnavailable = data.DayStartedBeingUnavailable;
        UnavailabilityDuration = data.UnavailabilityDuration;

        UpdateRank();
    }

    public CharacterData SerializeSelf()
    {
        CharacterData data = new();
        data.Id = Id;
        data.CharacterName = CharacterName;
        data.Portrait = Portrait.Id;
        data.Level = Level;
        data.Experience = Experience;
        data.Power = Power;
        data.MaxHealth = MaxHealth;
        data.MaxMana = MaxMana;
        data.Armor = Armor;
        data.MovementRange = MovementRange;
        data.Body = Body.name;
        data.Weapon = Weapon.name;

        List<AbilityData> abilityData = new();
        foreach (Ability a in Abilities)
            abilityData.Add(a.SerializeSelf());
        data.AbilityData = abilityData;

        List<string> itemReferenceIds = new();
        foreach (Item i in Items)
            itemReferenceIds.Add(i.ReferenceID);
        data.ItemReferenceIds = new(itemReferenceIds);

        data.IsAssigned = IsAssigned;
        data.IsOnUnavailable = IsUnavailable;
        data.DayStartedBeingUnavailable = DayStartedBeingUnavailable;
        data.UnavailabilityDuration = UnavailabilityDuration;

        return data;
    }

}

[System.Serializable]
public struct CharacterData
{
    public string Id;
    public string ReferenceID;
    public string CharacterName;
    public string Portrait;
    public int Level;
    public int Experience;
    public int Power;
    public int MaxHealth;
    public int MaxMana;
    public int Armor;
    public int MovementRange;
    public string Body;
    public string Weapon;
    public List<AbilityData> AbilityData;
    public List<string> ItemReferenceIds;

    public bool IsAssigned;
    public bool IsOnUnavailable;
    public int DayStartedBeingUnavailable;
    public int UnavailabilityDuration;
}
