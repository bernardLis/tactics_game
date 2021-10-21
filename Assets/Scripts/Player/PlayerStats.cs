using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerStats : CharacterStats
{

	protected override void Awake()
	{
		base.Awake();
		FindObjectOfType<TurnManager>().enemyTurnEndEvent += OnEnemyTurnEnd;
	}

	public override void Die()
	{
		base.Die();
	}

	void OnEnemyTurnEnd()
	{
		GainMana(10);
	}

}
