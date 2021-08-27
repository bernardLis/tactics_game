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
	public int maxHealth;
	public int armor;
	public int strength;
	public int intelligence;
	public int maxMana;
	public int movementRange;
	
	public Ability[] characterAbilities;
}
