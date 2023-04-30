using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Battle/Army Entity")]
public class ArmyEntity : BaseScriptableObject
{
    public string Name;
    public Sprite Icon;
    public int Price;
    public Element Element;

    public float Health;
    public float Armor;

    public float Power;
    public float AttackRange;
    public float AttackCooldown;

    public float Speed;
    
    public GameObject Prefab;
    public GameObject Projectile;
    public GameObject HitPrefab;

    public Hero Hero;
    public void HeroInfluence(Hero hero)
    {
        Hero = hero;
    }

    public float CalculateDamage(BattleEntity attacker)
    {
        float damage = attacker.Stats.Power;
        if (attacker.Stats.Hero != null)
            damage += attacker.Stats.Hero.Power.GetValue();

        if (Element.StrongAgainst == attacker.Stats.Element)
            damage *= 0.5f;
        if (Element.WeakAgainst == attacker.Stats.Element)
            damage *= 1.5f;

        damage = Mathf.Round(damage);

        float armor = Armor;
        if (Hero != null)
            armor += Hero.Armor.GetValue();

        damage -= Armor;
        if (damage < 0)
            damage = 0;

        return damage;
    }


    public float CalculateDamage(Ability ability)
    {
        float damage = ability.GetPower();
        if (Element.StrongAgainst == ability.Element)
            damage *= 0.5f;
        if (Element.WeakAgainst == ability.Element)
            damage *= 1.5f;

        damage = Mathf.Round(damage);

        // abilities ignore armor

        return damage;
    }

}