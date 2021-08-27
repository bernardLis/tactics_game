using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/AttackAbility")]
public class AttackAbility : Ability
{
	private Transform tr;
	private Highlighter highlighter;
	private AttackTriggerable attackTriggerable;
	private CharacterStats myStats;

	public override void Initialize(GameObject obj)
	{
		// TODO: it seems like this does not work if there are many characters that use that ability.
		// it seems like all those variables are overwritten to only one user. But why? 
		tr = obj.transform;
		highlighter = GameManager.instance.GetComponent<Highlighter>();
		attackTriggerable = obj.GetComponent<AttackTriggerable>();
		myStats = obj.GetComponent<CharacterStats>();
	}

	public override void HighlightTargetable()
	{
		highlighter.HighlightTiles(tr.position, range, highlightColor, canTargetDiagonally, canTargetSelf);
	}

	// maybe this should be triggered from the move point
	public override void TriggerAbility(GameObject target)
	{
 		attackTriggerable.Attack(target);
	}


}
