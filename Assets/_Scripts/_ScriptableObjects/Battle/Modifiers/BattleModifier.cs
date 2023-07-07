using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Battle/Battle Modifier")]
public class BattleModifier : BaseScriptableObject
{
    public BattleModifierType BattleModifierType;
    public Sprite Icon;
    public float Multiplier;
    public string Description;
    public int Cost;
}

public enum BattleModifierType
{
    Obstacle,
    CreatureSpeed,
    ElementDamage,
    CreatureAbilityCooldown,
    AbilityDamage,
    AbilityAOE,
    AbilityManaCost,
    AbilityCooldown
}

