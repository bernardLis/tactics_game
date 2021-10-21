using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EnemyStats : CharacterStats
{
	bool firstTurn = true;

	protected override void Awake()
	{
		base.Awake();
		FindObjectOfType<TurnManager>().playerTurnEndEvent += OnPlayerTurnEnd;
	}

	public override void Die()
	{
		base.Die();
	}

	void OnPlayerTurnEnd()
	{
		// TODO: this is a bad way to block enemies from getting mana on their first turn
		if (!firstTurn)
		{
			GainMana(10);
		}
		else
		{
			firstTurn = false;
		}
	}
}
