/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

[RequireComponent(typeof(AILerp))]
[RequireComponent(typeof(AIDestinationSetter))]
public class CharacterMovementControllerAlt : MonoBehaviour
{
	// movepoint controller will yield controll to this script when character is selected
	public GameObject gameManager;
	public GameObject movePoint;
	MovePointController movePointController;
	AIDestinationSetter destinationSetter;
	PlayerCharSelection characterSelection;
	CharacterInteractionController characterInteractionController;
	TurnManager turnManager;
	AILerp AILerp;
	IsometricCharacterRenderer _isoRenderer;
	

	WorldTile _tile;


	public bool reachedDestinationThisTurn = false;


	void Awake()
	{
		turnManager = (TurnManager)gameManager.GetComponent<TurnManager>();
		

		movePointController = (MovePointController)movePoint.GetComponent<MovePointController>();

		characterSelection = (PlayerCharSelection)transform.GetComponent<PlayerCharSelection>();
		destinationSetter = (AIDestinationSetter)transform.GetComponent<AIDestinationSetter>();
		characterInteractionController = (CharacterInteractionController) transform.GetComponent<CharacterInteractionController>();

		AILerp = (AILerp)transform.GetComponent<AILerp>();
		_isoRenderer = GetComponentInChildren<IsometricCharacterRenderer>();
	}

	void Update(){
		if(AILerp.reachedDestination && !reachedDestinationThisTurn){
			Debug.Log("AILerp.reachedDestination" + AILerp.reachedDestination);
			OnTargetReached();
		}
	}
	public void OnTargetReached()
	{
		//destinationSetter.target = null;

		AILerp.canSearch = false;
		AILerp.canMove = false;

		Vector2 inputVector = new Vector2(0f, 0f);
		_isoRenderer.SetDirection(inputVector);

		// do stuff
		if(!reachedDestinationThisTurn){
			movePointController.blockMovePoint = false;
			characterSelection.movedThisTurn = true;
			reachedDestinationThisTurn = true;

			// yield control to interaction controller	
			turnManager.PlayerCharacterTurnFinished();
		}
	}

	public void Move(Transform t)
	{
		//Vector3 currentPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z);
		if (destinationSetter != null)
		{

			var tiles = GameTiles.instance.tiles; // This is our Dictionary of tiles

			// check if selected tile is within range
			int posX = (int)Mathf.Floor(t.position.x);
			int posY = (int)Mathf.Floor(t.position.y);

			var worldPoint = new Vector3Int(posX, posY, 0);

			if (tiles.TryGetValue(worldPoint, out _tile))
			{
				if (_tile.WithinRange)
				{
					// move
					destinationSetter.target = t;
					AILerp.canSearch = true;
					AILerp.canMove = true;

					// disable movepoint
					movePointController.blockMovePoint = true;

					// clear highlight
					characterSelection.ClearHighlight();
				}
				else
				{
					Debug.Log("not within range");
				}
			}
		}
	}

}
*/