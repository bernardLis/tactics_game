using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Battle/Battle Modifier")]
public class BattleModifier : BaseScriptableObject
{
    public BattleModifierType BattleModifierType;
    public Sprite Icon;
    public string Description;
    public int Cost;
}

public enum BattleModifierType
{
    Obstacle,
    DoubleCreatureSpeed,
    NoElementDamageMultiplier,
    DoubleElementDamageMultiplier,
    HalfCreatureAbilityCooldown,
    DoubleCreatureAbilityCooldown,
    DoubleAbilityDamage,
    DoubleAbilityAOE,
    HalfAbilityManaCost,
    NoAbilityCooldown
}

