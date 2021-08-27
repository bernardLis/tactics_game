using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Ability : ScriptableObject
{
	public string aName = "New Ability";
	public string aDescription = "New Description";

	public string aType;

	public Sprite aIcon;
	public AudioClip aSound;
	public int value;
	public int range;
	public int manaCost;
	public bool canTargetSelf;
	public bool canTargetDiagonally;
	public Color highlightColor;

	public abstract void Initialize(GameObject obj);
	public abstract void HighlightTargetable();
	public abstract void TriggerAbility(GameObject target);
}
