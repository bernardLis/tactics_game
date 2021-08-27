using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/HealAbility")]
public class HealAbility : Ability
{
	private Transform tr;
	private Highlighter highlighter;
	private HealTriggerable healTriggerable;
	private CharacterStats myStats;

	public override void Initialize(GameObject obj)
	{
		tr = obj.transform;
		highlighter = GameManager.instance.GetComponent<Highlighter>();
		healTriggerable = obj.GetComponent<HealTriggerable>();
		myStats = obj.GetComponent<CharacterStats>();
	}

	public override void HighlightTargetable()
	{
		highlighter.HighlightTiles(tr.position, range, highlightColor, canTargetDiagonally, canTargetSelf);
	}

	// maybe this should be triggered from the move point
	public override void TriggerAbility(GameObject target)
	{
		healTriggerable.Heal(target); // 
	}

}
