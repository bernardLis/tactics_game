using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerCharSelection : CharacterSelection
{
	public GameObject selectionCircle;
	SpriteRenderer selectionCircleRenderer;

	public bool movedThisTurn = false;

	public override void Awake()
	{
		base.Awake();
		FindObjectOfType<TurnManager>().enemyTurnEndEvent += OnTurnEnd;
		selectionCircleRenderer = selectionCircle.GetComponent<SpriteRenderer>();

		// subscribe to player death
		transform.GetComponent<CharacterStats>().characterDeathEvent += OnPlayerCharDeath;
	}
	void OnPlayerCharDeath()
	{
		UnselectCharacter();
		highlighter.ClearHighlightedTiles();
	}

	public void SelectCharacter()
	{
		// selectionCircle.SetActive(true);
		if (!movedThisTurn)
		{
			selectionCircle.SetActive(false);
			HiglightMovementRange();
		}
	}
	public void UnselectCharacter()
	{
		//selectionCircle.SetActive(false);
		selectionCircle.SetActive(true);
		if (!movedThisTurn)
		{
			selectionCircleRenderer.color = new Color(0.53f, 0.52f, 1f, 1f);
		}
		else
		{
			selectionCircleRenderer.color = new Color(1f, 1f, 1f, 1f);
		}

		highlighter.ClearHighlightedTiles();
	}

	public override void HiglightMovementRange()
	{
		base.HiglightMovementRange();
		highlighter.HiglightPlayerMovementRange(transform.position, range, new Color(0.53f, 0.52f, 1f, 1f));
	}

	public void OnTurnEnd()
	{
		selectionCircleRenderer.color = new Color(0.53f, 0.52f, 1f, 1f);
		movedThisTurn = false;
	}
}
