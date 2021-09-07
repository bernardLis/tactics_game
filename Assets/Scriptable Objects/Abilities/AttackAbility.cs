using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Attack Ability")]
public class AttackAbility : Ability
{
	private Transform tr;
	private Highlighter highlighter;
	private AttackTriggerable attackTriggerable;
	private CharacterStats myStats;

	public override void Initialize(GameObject obj)
	{
		tr = obj.transform;
		highlighter = GameManager.instance.GetComponent<Highlighter>();
		attackTriggerable = obj.GetComponent<AttackTriggerable>();
		myStats = obj.GetComponent<CharacterStats>();
	}

	public override void HighlightTargetable()
	{
		highlighter.HighlightTiles(tr.position, range, highlightColor, canTargetDiagonally, canTargetSelf);
	}

	public override void TriggerAbility(GameObject target)
	{
 		attackTriggerable.Attack(target);
	}


}
