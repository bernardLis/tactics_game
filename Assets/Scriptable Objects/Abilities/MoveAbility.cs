using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Move Ability")]
public class MoveAbility : Ability
{
	private Transform tr;
	private Highlighter highlighter;
	private PushTriggerable pushTriggerable;
	private CharacterStats myStats;

	public override void Initialize(GameObject obj)
	{
		tr = obj.transform;
		highlighter = GameManager.instance.GetComponent<Highlighter>();
		pushTriggerable = obj.GetComponent<PushTriggerable>();
		myStats = obj.GetComponent<CharacterStats>();
	}
	public override void HighlightTargetable()
	{
		highlighter.HighlightTiles(tr.position, range, highlightColor, canTargetDiagonally, canTargetSelf);
	}

	// maybe this should be triggered from the move point
	public override void TriggerAbility(GameObject target)
	{
		pushTriggerable.Push(target); // 
	}
}
