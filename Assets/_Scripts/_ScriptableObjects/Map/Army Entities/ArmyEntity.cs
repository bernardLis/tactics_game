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

    public GameObject Prefab;
    public float Health;
    public float Armor;

    public float Power;
    public float AttackRange;
    public float AttackCooldown;

    public float Speed;

    public GameObject Projectile;


    public float CalculateDamage(BattleEntity attacker)
    {
        float damage = attacker.Stats.Power; //- armor //* attacker.Stats.Element.DamageMultiplier[Element];
        return damage;
    }
}