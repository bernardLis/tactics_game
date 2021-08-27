using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EnemyCharSelection : CharacterSelection
{

	public override void Awake()
	{
		base.Awake();

		// subscribe to enemy death
		transform.GetComponent<CharacterStats>().characterDeathEvent += OnEnemyDeath;

	}
	void OnEnemyDeath()
	{
		highlighter.ClearHighlightedTiles();
	}

	public override void HiglightMovementRange()
	{
		base.HiglightMovementRange();
		highlighter.HiglightEnemyMovementRange(transform.position, range, new Color(0.53f, 0.52f, 1f, 1f));
	}
}
