using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : BaseScriptableObject
{
    public string Name;
    public Sprite[] IconAnimation;
    public int Level;
    public int Price;
    public Element Element;

    public float BaseHealth;
    public float Armor;

    public float Speed;

    public GameObject Prefab;

    // BATTLE
    [HideInInspector] public Hero Hero;
    protected float _elementalDamageMultiplier = 0f;

    public virtual void InitializeBattle(Hero hero)
    {
        if (hero != null) Hero = hero;
    }

    // TODO: math
    public int GetHealth() { return Mathf.RoundToInt(BaseHealth + 0.2f * BaseHealth * (Level - 1)); }

    public virtual int CalculateDamage(BattleCreature attacker)
    {
        float damage = attacker.Creature.GetPower();
        if (attacker.Entity.Hero != null)
            damage += attacker.Entity.Hero.Power.GetValue();

        damage *= GetElementalDamageMultiplier(attacker.Entity.Element);

        damage -= Armor;
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

    public int CalculateDamage(BattleTurret bt)
    {
        float damage = bt.Turret.GetCurrentUpgrade().Power;

        damage *= GetElementalDamageMultiplier(bt.Turret.Element);
        damage -= Armor;
        if (damage < 0)
            damage = 0;

        return Mathf.RoundToInt(damage);
    }

    float GetElementalDamageMultiplier(Element attackerElement)
    {
        float elementalDamageBonus = 1f;
        if (Element.StrongAgainst == attackerElement)
            elementalDamageBonus = 0.5f;
        if (Element.WeakAgainst == attackerElement)
            elementalDamageBonus = 1.5f;

        elementalDamageBonus += _elementalDamageMultiplier;
        return elementalDamageBonus;
    }
}
