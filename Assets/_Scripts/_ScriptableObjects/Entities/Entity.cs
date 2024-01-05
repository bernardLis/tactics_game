using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Entity : BaseScriptableObject
{
    [Header("Entity")]
    public string EntityName;
    public Sprite Icon;
    public Sprite[] IconAnimation;
    public int Price;
    public Element Element;
    public GameObject Prefab;

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

        CurrentHealth = CreateInstance<IntVariable>();
        CurrentHealth.SetValue(MaxHealth.GetValue());
    }

    public void AddDmgTaken(int dmg) { TotalDamageTaken += dmg; }

    [Header("Stats")]
    public Stat MaxHealth;
    public IntVariable CurrentHealth;
    public Stat Armor;

    protected virtual void CreateStats()
    {
        MaxHealth = Instantiate(MaxHealth);
        Armor = Instantiate(Armor);

        MaxHealth.Initialize();
        Armor.Initialize();

        OnLevelUp += MaxHealth.LevelUp;
        OnLevelUp += Armor.LevelUp;
    }

    /* LEVEL */
    [Header("Level")]
    public IntVariable Level;
    [HideInInspector] public IntVariable Experience;
    [HideInInspector] public IntVariable ExpForNextLevel;
    [HideInInspector] public int LeftoverExp;
    public event Action OnLevelUp;

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

        CurrentHealth.SetValue(MaxHealth.GetValue());

        // HERE: entity rework - probably need to scale stats with level - change base values
        // but that would be different for creature and for hero
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

    /* SERIALIZATION */
    public EntityData SerializeSelf()
    {
        // TODO: to be implemented

        return new EntityData();
    }

    public void LoadFromData(EntityData data)
    {
    }
}

[Serializable]
public struct EntityData
{
    public string Name;

    public int Health;
    public int Armor;

    public int Level;
    public int Experience;
}
