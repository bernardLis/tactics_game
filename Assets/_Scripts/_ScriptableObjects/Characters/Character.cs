using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Random = UnityEngine.Random;

[CreateAssetMenu(menuName = "ScriptableObject/Character/Player")]
public class Character : BaseScriptableObject
{
    // character scriptable object holds stats & abilities of a character.
    // it passes these values to CharacterStats script where they can be used in game.
    public string ReferenceID;
    public string CharacterName = "Default";
    public Sprite Portrait;

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
    [Tooltip("For now just defend, basic attack is from the weapon")]
    public List<Ability> BasicAbilities = new();
    public List<Ability> Abilities = new();

    [Header("Quest")]
    [HideInInspector] public bool IsOnQuest;

    public event Action OnCharacterLevelUp;
    public event Action<int> OnCharacterExpGain;

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

    public void GetExp(Character opponent, bool isKill = false)
    {
        int opponentLevel = opponent.Level;
        int exp = CalculateExpGain(opponentLevel, isKill);
        BaseExpGain(exp);
    }

    protected virtual void BaseExpGain(int gain)
    {
        Experience += gain;
        OnCharacterExpGain?.Invoke(gain);

        if (Experience < 100)
            return;

        LevelUp();
    }

    int CalculateExpGain(int opponentLevel, bool isKill)
    {
        int expGain = 10;
        if (isKill)
            expGain += 20;

        int levelExpGain = (opponentLevel - Level) * 6;
        levelExpGain = Mathf.Clamp(levelExpGain, 0, 100);
        expGain += levelExpGain;

        return expGain;
    }

    public void LevelUp()
    {
        Experience = 0;

        Level++;
        Power += Random.Range(0, 2); ;

        OnCharacterLevelUp?.Invoke();
    }

    public int GetNumberOfAbilitySlots() { return 2; }

    public void AddAbility(Ability ability) { Abilities.Add(ability); }

    public void RemoveAbility(Ability ability) { Abilities.Remove(ability); }

    public void AddItem(Item item)
    {
        Items.Add(item);
        ResolveItems();
        InformSubscribers(item);
    }

    public void RemoveItem(Item item)
    {
        Items.Remove(item);
        ResolveItems();
        InformSubscribers(item);
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

    // creates character at runtime from saved data
    public virtual void CreateFromData(CharacterData data)
    {
        GameDatabase gameDatabase = GameManager.Instance.GameDatabase;
        Id = data.Id;
        ReferenceID = data.ReferenceID;
        CharacterName = data.CharacterName;
        Portrait = gameDatabase.GetPortraitById(data.Portrait);

        Level = data.Level;
        Experience = data.Experience;
        Power = data.Power;
        MaxHealth = data.MaxHealth;
        MaxMana = data.MaxMana;
        Armor = data.Armor;
        MovementRange = data.MovementRange;

        Body = gameDatabase.GetBodyByName(data.Body);
        Weapon = gameDatabase.GetWeaponByName(data.Weapon);

        BasicAbilities.Add(gameDatabase.GetAbilityById("5f7d8c47-7ec1-4abf-b8ec-74ea82be327f")); // basic defend id

        foreach (string id in data.AbilityReferenceIds)
            Abilities.Add(gameDatabase.GetAbilityByReferenceId(id));

        foreach (string id in data.ItemReferenceIds)
            Items.Add(gameDatabase.GetItemByReferenceId(id));

        IsOnQuest = data.IsOnQuest;
    }


    public CharacterData SerializeSelf()
    {
        CharacterData data = new();
        data.Id = Id;
        data.ReferenceID = ReferenceID;
        data.CharacterName = CharacterName;
        data.Portrait = Portrait.name;
        data.Level = Level;
        data.Experience = Experience;
        data.Power = Power;
        data.MaxHealth = MaxHealth;
        data.MaxMana = MaxMana;
        data.Armor = Armor;
        data.MovementRange = MovementRange;
        data.Body = Body.name;
        data.Weapon = Weapon.name;

        List<string> abilityReferenceIds = new();
        foreach (Ability a in Abilities)
            abilityReferenceIds.Add(a.ReferenceID);
        data.AbilityReferenceIds = new(abilityReferenceIds);

        List<string> itemReferenceIds = new();
        foreach (Item i in Items)
            itemReferenceIds.Add(i.ReferenceID);
        data.ItemReferenceIds = new(itemReferenceIds);

        data.IsOnQuest = IsOnQuest;

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
    public List<string> AbilityReferenceIds;
    public List<string> ItemReferenceIds;

    public bool IsOnQuest;
}
