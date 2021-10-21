using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Character")]
public class Character : ScriptableObject
{
    // character scriptable object holds stats & abilities of a character.
    // it passes these values to CharacterStats script where they can be used in game.
    public string characterName = "Default";
    public Sprite portrait;

    [Header("Stats")]
    public int maxHealth;
    public int armor;
    public int strength;
    public int intelligence;
    public int maxMana;
    public int movementRange;

    [Header("Equipment")]
    public Equipment body;
    public Equipment feet;
    public Equipment hair;
    public Equipment hands;
    public Equipment helmet;
    public Equipment legs;
    public Equipment torso;
    public Equipment shield;
    public Equipment weapon;

    [Header("Abilities")]
    public Ability[] characterAbilities;
}
