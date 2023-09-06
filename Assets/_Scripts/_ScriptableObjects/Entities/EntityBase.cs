using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class EntityBase : BaseScriptableObject
{
    [Header("Entity Base")]
    public string EntityName;
    public Sprite Icon;
    public Sprite[] IconAnimation;
    public int Price;
    public Element Element;
    public GameObject Prefab;

    [Header("Level")]
    public IntVariable Level;
    [HideInInspector] public IntVariable Experience;
    [HideInInspector] public IntVariable ExpForNextLevel;
    [HideInInspector] public int LeftoverExp;
    public event Action OnLevelUp;

    [HideInInspector] public int OldDamageTaken;
    [HideInInspector] public int TotalDamageTaken;

    [HideInInspector] public int Team;

    public virtual void InitializeBattle(int team)
    {
        Team = team;

        CreateStats();
        OldDamageTaken = TotalDamageTaken;

        Level = Instantiate(Level);

        Experience = CreateInstance<IntVariable>();
        Experience.SetValue(0);

        ExpForNextLevel = CreateInstance<IntVariable>();
        ExpForNextLevel.SetValue(GetExpForNextLevel());
    }

    public void AddDmgTaken(int dmg) { TotalDamageTaken += dmg; }

    [Header("Stats")]
    public IntVariable BaseTotalHealth;
    public Stat TotalHealth { get; private set; }
    public Vector2Int HealthGrowthPerLevel;

    public IntVariable BaseArmor;
    public Stat Armor { get; private set; }
    public Vector2Int ArmorGrowthPerLevel;

    protected virtual void CreateStats()
    {
        BaseTotalHealth = Instantiate(BaseTotalHealth);
        TotalHealth = CreateInstance<Stat>();
        TotalHealth.StatType = StatType.Health;
        TotalHealth.SetBaseValue(BaseTotalHealth.Value);
        BaseTotalHealth.OnValueChanged += TotalHealth.SetBaseValue;

        BaseArmor = Instantiate(BaseArmor);
        Armor = CreateInstance<Stat>();
        Armor.StatType = StatType.Armor;
        Armor.SetBaseValue(BaseArmor.Value);
        BaseArmor.OnValueChanged += Armor.SetBaseValue;
    }

    /* LEVEL */
    public virtual int GetExpForNextLevel()
    {
        // meant to be overwritten
        return 100;
    }

    public virtual void AddExp(int gain)
    {
        LeftoverExp = gain;
        if (Experience.Value + gain >= ExpForNextLevel.Value)
        {
            LeftoverExp = Experience.Value + gain - ExpForNextLevel.Value;
            LevelUp();
        }

        Experience.ApplyChange(LeftoverExp);
        LeftoverExp = 0;
    }

    public virtual void LevelUp()
    {
        Level.ApplyChange(1);
        Experience.SetValue(0);
        ExpForNextLevel.SetValue(GetExpForNextLevel());
        OnLevelUp?.Invoke();

        // HERE: entity rework - probably need to scale stats with level - change base values
        // but that would be different for creature and for 
    }

    /* DAMAGE */
    public virtual int CalculateDamage(EntityFight attacker)
    {
        float damage = attacker.Power.GetValue();

        damage *= GetElementalDamageMultiplier(attacker.Element);

        damage -= Armor.GetValue();
        if (damage < 0)
            damage = 0;

        return Mathf.RoundToInt(damage);
    }

    public int CalculateDamage(Ability ability)
    {
        float damage = ability.GetPower();

        damage *= GetElementalDamageMultiplier(ability.Element);

        // abilities ignore armor
        return Mathf.RoundToInt(damage);
    }


    float GetElementalDamageMultiplier(Element attackerElement)
    {
        float elementalDamageBonus = 1f;
        if (Element.StrongAgainst == attackerElement)
            elementalDamageBonus = 0.5f;
        if (Element.WeakAgainst == attackerElement)
            elementalDamageBonus = 1.5f;

        return elementalDamageBonus;
    }

    /* LOOT */
    [Header("Loot")]
    [SerializeField] List<Loot> Loot = new();
    public Loot GetLoot()
    {
        int v = Random.Range(0, 101);
        List<Loot> possibleLoot = new();
        foreach (Loot l in Loot)
        {
            if (v <= l.LootChance)
                possibleLoot.Add(l);
        }

        // return the loot with the lowest chance
        if (possibleLoot.Count > 0)
        {
            Loot lowestChanceLoot = possibleLoot[0];
            foreach (Loot l in possibleLoot)
            {
                if (l.LootChance < lowestChanceLoot.LootChance)
                    lowestChanceLoot = l;
            }

            Loot instance = Instantiate(lowestChanceLoot);
            instance.Initialize();
            return instance;
        }
        return null;
    }

    /* SERIALIZATION */
    public EntityBaseData SerializeSelf()
    {
        // TODO: to be implemented

        return new EntityBaseData();
    }

    public void LoadFromData(EntityBaseData data)
    {
    }
}

[Serializable]
public struct EntityBaseData
{
    public string Name;

    public int Health;
    public int Armor;

    public int Level;
    public int Experience;
}
