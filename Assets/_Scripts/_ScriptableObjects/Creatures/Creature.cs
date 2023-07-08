using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(menuName = "ScriptableObject/Battle/Creature")]
public class Creature : BaseScriptableObject
{
    public string Name;
    public int UpgradeTier;
    public Sprite[] IconAnimation;
    public int Level;
    public int Price;
    public Element Element;

    public float BaseHealth;
    public float Armor;

    public float BasePower;
    public float AttackRange; // stopping distance of agent
    public float AttackCooldown;
    public float Speed;

    public CreatureAbility CreatureAbility;

    public GameObject Prefab;
    public GameObject Projectile;
    public GameObject HitPrefab;

    [Header("Upgrade")]
    public Creature EvolvedCreature;

    // battle
    [HideInInspector] public int OldKillCount;
    [HideInInspector] public int TotalKillCount;

    [HideInInspector] public int OldDamageDealt;
    [HideInInspector] public int TotalDamageDealt;

    [HideInInspector] public int OldDamageTaken;
    [HideInInspector] public int TotalDamageTaken;

    [HideInInspector] public Hero Hero;

    float ElementalDamageMultiplier = 1f;

    public event Action OnLevelUp;

    public void InitializeBattle(Hero hero)
    {
        OldKillCount = TotalKillCount;
        OldDamageDealt = TotalDamageDealt;
        OldDamageTaken = TotalDamageTaken;

        if (hero != null) Hero = hero;

        // modifiers
        Battle b = GameManager.Instance.SelectedBattle;

        Speed *= b.CreatureSpeedMultiplier;
        ElementalDamageMultiplier = b.ElementalDamageMultiplier;
        if (CreatureAbility != null)
        {
            CreatureAbility = Instantiate(CreatureAbility);
            Debug.Log($"b.CreatureAbilityCooldown {b.CreatureAbilityCooldown}");
            CreatureAbility.Cooldown = Mathf.FloorToInt(CreatureAbility.Cooldown * b.CreatureAbilityCooldown);
        }
    }

    public void InitializeMinion(int level)
    {
        Level = level;
        if (level >= 5) Speed = 2;
    }

    // TODO: math
    public int GetHealth() { return Mathf.RoundToInt(BaseHealth + 0.2f * BaseHealth * (Level - 1)); }

    public int GetPower() { return Mathf.RoundToInt(BasePower + 0.1f * BasePower * (Level - 1)); }

    public void AddKill(int ignored) { TotalKillCount++; }
    public void AddDmgDealt(int dmg) { TotalDamageDealt += dmg; }
    public void AddDmgTaken(int dmg) { TotalDamageTaken += dmg; }

    public int CalculateDamage(BattleEntity attacker)
    {
        float damage = attacker.Creature.GetPower();
        if (attacker.Creature.Hero != null)
            damage += attacker.Creature.Hero.Power.GetValue();

        float elementalDamageBonus = 0f;
        if (Element.StrongAgainst == attacker.Creature.Element)
            elementalDamageBonus = -damage * 0.5f;
        if (Element.WeakAgainst == attacker.Creature.Element)
            elementalDamageBonus = damage * 0.5f;

        elementalDamageBonus *= ElementalDamageMultiplier;
        damage += elementalDamageBonus;

        damage = Mathf.Round(damage);

        float armor = Armor;
        if (Hero != null)
            armor += Hero.Armor.GetValue();

        damage -= Armor;
        if (damage < 0)
            damage = 0;

        return Mathf.RoundToInt(damage);
    }

    public int CalculateDamage(Ability ability)
    {
        float damage = ability.GetPower();
        if (Element.StrongAgainst == ability.Element)
            damage *= 0.5f;
        if (Element.WeakAgainst == ability.Element)
            damage *= 1.5f;

        damage = Mathf.Round(damage);

        // abilities ignore armor
        return Mathf.RoundToInt(damage);
    }

    public int NextLevelSpiceRequired()
    {
        // TODO: math
        return 10 * Level;
    }

    public void LevelUp()
    {
        Level++;
        OnLevelUp?.Invoke();

    }

    public bool ShouldEvolve()
    {
        return Random.value < ChanceToEvolve(Level);
    }

    public float ChanceToEvolve(int level)
    {
        if (EvolvedCreature == null) return 0;
        // starting from level 5 there is an increasing chance to evolve, 
        // which caps at 100% at level 10
        // starting from level 5 there is an increasing chance to evolve, 
        // which caps at 100% at level 10 
        // TODO: math, and also tier 1 +10 levels
        float chance = 0.1f * ((level - 4) * 1.5f);
        if (chance < 0) return 0;
        return chance;
        // level 5 -> 0.15
        // level 6 -> 0.3
        // level 7 -> 0.45
        // level 8 -> 0.6
        // level 9 -> 0.75
        // level 10 -> 0.9
        // level 11 -> 1.05

    }

    public CreatureData SerializeSelf()
    {
        CreatureData data = new();
        data.CreatureId = Id;

        data.Name = Name;
        data.Level = Level;

        data.KillCount = TotalKillCount;
        data.DamageDealt = TotalDamageDealt;
        data.DamageTaken = TotalDamageTaken;

        return data;
    }

    public void LoadFromData(CreatureData data)
    {
        Name = data.Name;
        Level = data.Level;

        TotalKillCount = data.KillCount;
        TotalDamageDealt = data.DamageDealt;
        TotalDamageTaken = data.DamageTaken;
    }
}

[System.Serializable]
public struct CreatureData
{
    public string Name;
    public int Level;
    public string CreatureId;

    public int KillCount;
    public int DamageDealt;
    public int DamageTaken;
}