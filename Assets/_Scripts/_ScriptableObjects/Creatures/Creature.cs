using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Battle/Creature")]
public class Creature : BaseScriptableObject
{
    public string Name;
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
    public Creature UpgradedCreature;
    public int KillsToUpgrade;

    // battle
    [HideInInspector] public int OldKillCount;
    [HideInInspector] public int TotalKillCount;

    [HideInInspector] public int OldDamageDealt;
    [HideInInspector] public int TotalDamageDealt;

    [HideInInspector] public int OldDamageTaken;
    [HideInInspector] public int TotalDamageTaken;

    [HideInInspector] public Hero Hero;

    public void InitializeBattle(Hero hero)
    {
        OldKillCount = TotalKillCount;
        OldDamageDealt = TotalDamageDealt;
        OldDamageTaken = TotalDamageTaken;

        if (hero != null) Hero = hero;
    }

    public int GetHealth() { return Mathf.RoundToInt(BaseHealth + 0.2f * BaseHealth * Level); }

    public int GetPower() { return Mathf.RoundToInt(BasePower + 0.1f * BasePower * Level); }

    public void AddKill(int ignored) { TotalKillCount++; }
    public void AddDmgDealt(int dmg) { TotalDamageDealt += dmg; }
    public void AddDmgTaken(int dmg) { TotalDamageTaken += dmg; }

    public int CalculateDamage(BattleEntity attacker)
    {
        float damage = attacker.Creature.GetPower();
        if (attacker.Creature.Hero != null)
            damage += attacker.Creature.Hero.Power.GetValue();

        if (Element.StrongAgainst == attacker.Creature.Element)
            damage *= 0.5f;
        if (Element.WeakAgainst == attacker.Creature.Element)
            damage *= 1.5f;

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

    /*
            public bool ShouldEvolve()
            {
                if (Creature.UpgradedCreature == null) return false;

                return TotalKillCount >= NumberOfKillsToEvolve();
            }

            public void Evolve()
            {
                PreviousCreature = Creature;
                Creature = Creature.UpgradedCreature;
                OnEvolved?.Invoke(this);
            }
        */

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