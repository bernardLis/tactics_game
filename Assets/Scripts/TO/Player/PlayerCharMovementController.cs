using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using UnityEngine.Tilemaps;

public class PlayerCharMovementController : CharacterMovementController
{

	PlayerCharSelection characterSelection;
	PlayerCharInteractionController playerCharInteractionController;

	public InputMaster controls;

	WorldTile _tile;
	WorldTile currentTile;
	WorldTile destinationTile;

	Vector3 lastPosition;
	public bool reachedDestinationThisTurn = false;
	GameObject tempObject;

	protected override void Awake()
	{
		base.Awake();
		FindObjectOfType<TurnManager>().enemyTurnEndEvent += OnTurnEnd;
		GetComponent<PlayerStats>().characterDeathEvent += OnPlayerDeath;

		characterSelection = GetComponent<PlayerCharSelection>();
		playerCharInteractionController = GetComponent<PlayerCharInteractionController>();
		//characterInteractionController = (CharacterInteractionController)transform.GetComponent<CharacterInteractionController>();

		// https://www.youtube.com/watch?v=Pzd8NhcRzVo
		// registering input callbacks (keyboard)
		controls = new InputMaster();
		controls.Player.Back.performed += ctx => BackClickInput();

		// mark && remember the current tile as player pos
		Vector3Int tilePos = tilemap.WorldToCell(transform.position);
		if (tiles.TryGetValue(tilePos, out _tile))
		{
			currentTile = _tile;
		}
	}

	public void BackClickInput()
	{
		if (reachedDestinationThisTurn)
		{
			// block player input when character is moving back;
			controls.Disable();

			// GO BACK WITH AILERP quickly.
			tempObject = new GameObject("Player Destination");
			tempObject.transform.position = lastPosition;

			// move
			destinationSetter.target = tempObject.transform;

			AILerp.canSearch = true;
			AILerp.canMove = true;
			AILerp.speed = 25f;
		}
		else
		{
			MovePointController.instance.UnselectSelected();
		}
	}

	protected override void OnEnable()
	{
		base.OnEnable();
		controls.Enable();
	}
	protected override void OnDisable()
	{
		base.OnDisable();
		controls.Disable();
	}

	public void Move(Transform t)
	{
		// not allowing player input when character is moving
		controls.Disable();

		// remember your last position && tile
		lastPosition = transform.position;

		Vector3Int tilePos = tilemap.WorldToCell(transform.position);

		if (tiles.TryGetValue(tilePos, out _tile))
		{
			currentTile = _tile;
		}

		// check if selected tile is within range
		tilePos = tilemap.WorldToCell(t.position);

		if (tiles.TryGetValue(tilePos, out _tile))
		{
			// remember the destination tile
			destinationTile = _tile;

			// don't move if it's the same tile
			if (currentTile == _tile)
			{
				OnTargetReached();
			}
			else
			{
				// move
				destinationSetter.target = t;
				AILerp.canSearch = true;
				AILerp.canMove = true;

				// disable movepoint
				MovePointController.instance.blockMovePoint = true;
			}
			// clear highlight
			highlighter.ClearHighlightedTiles();
		}
	}
	public override void OnTargetReached()
	{
		base.OnTargetReached();

		if (reachedDestinationThisTurn)
		{
			destinationTile = null;
			reachedDestinationThisTurn = false;

			// resetting ailerp speed
			AILerp.speed = 3f;

			characterSelection.movedThisTurn = false;
			characterSelection.HiglightMovementRange();

			MovePointController.instance.blockMovePoint = false;

			// destroy the temp object
			if (tempObject != null)
			{
				Destroy(tempObject);
			}

			// allow player input
			controls.Enable();
		}
		else
		{
			characterSelection.movedThisTurn = true;
			reachedDestinationThisTurn = true;

			// yield control to interaction controller
			playerCharInteractionController.enabled = true;

			// disable controls
			controls.Disable();

			// disable self
			this.enabled = false;
		}
	}

	public void OnTurnEnd()
	{
		reachedDestinationThisTurn = false;
	}

	void OnPlayerDeath()
	{
	}
}
