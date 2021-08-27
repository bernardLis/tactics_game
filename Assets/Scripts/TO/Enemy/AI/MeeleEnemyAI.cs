using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Pathfinding;

public class MeeleEnemyAI : EnemyAI
{
	// corutine vars
	bool playerReached;
	bool exit;

	protected override void Awake()
	{
		base.Awake();
	}

	public override IEnumerator RunAI()
	{
		yield return StartCoroutine(base.RunAI());

		// TODO: this is a repetition from the base coroutine,
		// idk how to make base coroutine exit both coroutines.
		// exit if battle is over
		if (TurnManager.instance.state.ToString() == "WON" || TurnManager.instance.state.ToString() == "LOST")
		{
			yield break;
		}
	
		// interact with target 
		if (playerReached && !enemyCharMovementController.trappedOnTheWay)
		{
			enemyInteractionController.Attack(targetCharacter);
			// clearing the highlight and finishing the turn
			yield return new WaitForSeconds(1f);
			highlighter.ClearHighlightedTiles();
		}
		// or just face its direction
		else
		{
			enemyInteractionController.FaceAndFinishInteraction(targetCharacter);
		}

		TurnManager.instance.EnemyCharacterTurnFinished();
		yield return true;
	}

	// * Player character we are getting should already have a valid tile around them
	protected override GameObject GetTargetCharacter()
	{
		playerCharacters = GameObject.FindGameObjectsWithTag("ControlledByPlayer");
		Dictionary<float, GameObject> distToPlayerCharacters = new Dictionary<float, GameObject>();
		// check distance between self and each player character,
		foreach (var playerCharacter in playerCharacters)
		{
			//https://arongranberg.com/astar/documentation/dev_4_1_6_17dee0ac/calling-pathfinding.php
			Path p = seeker.StartPath(transform.position, playerCharacter.transform.position);
			p.BlockUntilCalculated();
			// The path is calculated now
			// distance is the path length 
			// https://arongranberg.com/astar/docs_dev/class_pathfinding_1_1_path.php#a1076ed6812e2b4f98dca64b74dabae5d
			float distance = p.GetTotalLength();
			distToPlayerCharacters.Add(distance, playerCharacter);
		}

		// https://www.dotnetperls.com/sort-dictionary
		var list = distToPlayerCharacters.Keys.ToList();
		list.Sort();

		// going through sorted dict and looking for free tiles around characters
		foreach (float dist in list)
		{
			// we are returning the closest character that has a free tile around him
			if (CheckTilesAroundCharacter(distToPlayerCharacters[dist]))
			{
				targetCharacter = distToPlayerCharacters[dist];
				return targetCharacter;
			}
		}

		// there is no free tile around any character, returning the first character in the dict
		// TODO: this could be smarter. 
		targetCharacter = distToPlayerCharacters.First().Value;
		return targetCharacter;
	}

	bool CheckTilesAroundCharacter(GameObject character)
	{
		// get players tile and then get the tile up, left, right and left from him
		Vector3Int tilePos = tilemap.WorldToCell(character.transform.position);

		// check tiles around target player 
		Vector3Int[] tilesAroundPlayer = {
			new Vector3Int(tilePos.x+1, tilePos.y, tilePos.z),
			new Vector3Int(tilePos.x-1, tilePos.y, tilePos.z),
			new Vector3Int(tilePos.x, tilePos.y+1, tilePos.z),
			new Vector3Int(tilePos.x, tilePos.y-1, tilePos.z)
		};

		// for each point check if there is a within reach tile
		foreach (Vector3Int point in tilesAroundPlayer)
		{
			if (tiles.TryGetValue(point, out _tile))
			{
				if (highlighter.EnemyIsTileWalkable(point) && highlighter.EnemyCanIStopOnTheTile(point))
				{
					return true;
				}
			}
		}
		// reached the end, there are no free tiles around player char
		return false;
	}

	// * Deciding where to move enemy

	// this method gets the destination and checks whether target player will be reached
	protected override void GetDestination(GameObject targetPlayer)
	{
		// getting tile we are standing on
		Vector3Int tilePos = tilemap.WorldToCell(transform.position);
		if (tiles.TryGetValue(tilePos, out _tile))
		{
			currentTile = _tile;
		}

		// don't move if you can hit the player
		if (StayAtYourPostion())
		{
			playerReached = true;
			enemyCharMovementController.GoToDestination(transform.position);
		}
		// if you do not have movement range stay
		else if (myStats.movementRange.GetValue() <= 0)
		{
			enemyCharMovementController.GoToDestination(transform.position);
		}
		// go to the player if it is within the reach
		else if (PlayerWithinReach())
		{
			playerReached = true;
		}
		// else 
		else
		{
			playerReached = false;
			// find best tile calls seeker to pathfind;
			FindPath();
		}
	}

	/* *** if we are next to the player character => stay *** */
	bool StayAtYourPostion()
	{
		float posX = transform.position.x;
		float posY = transform.position.y;

		Vector2[] checkArray = {
			new Vector2(posX+1, posY),
			new Vector2(posX-1, posY),
			new Vector2(posX, posY+1),
			new Vector2(posX, posY-1)
		};

		// check if there is a player char on 4 tiles next to us
		foreach (Vector2 point in checkArray)
		{
			Collider2D col = Physics2D.OverlapCircle(point, 0.2f);
			if (col != null)
			{
				if (col.transform.CompareTag("PlayerCollider"))
					return true;
			}
		}
		return false;
	}

	/* *** if we can reach the player character => go to the tile that is next to the player *** */
	// this means:
	// check if there is a within reach tile next to target player character
	bool PlayerWithinReach()
	{
		// get player's tile and then get the tile up, left, right and left from him
		Vector3Int tilePos = tilemap.WorldToCell(targetCharacter.transform.position);

		// if there are within reach tiles next to the player go there
		Vector2Int[] withinReachCheckArray = {
			new Vector2Int(tilePos.x+1, tilePos.y),
			new Vector2Int(tilePos.x-1, tilePos.y),
			new Vector2Int(tilePos.x, tilePos.y+1),
			new Vector2Int(tilePos.x, tilePos.y-1)
		};

		// for each point check if there is a within reach tile
		foreach (Vector2Int point in withinReachCheckArray)
		{
			// go through within reach tiles and check if there is one that matches the point 
			// that's where we want to go
			foreach (WorldTile tile in highlighter.highlightedTiles)
			{
				if (point.x == tile.LocalPlace.x && point.y == tile.LocalPlace.y)
				{
					enemyCharMovementController.GoToDestination(new Vector3(point.x, point.y, tile.LocalPlace.z));
					return true;
				}
			}
		}
		return false;
	}

	/* ***if we cannot reach the player character => go to the tile that is nearest the player and within range*** */
	// this mean: 
	// https://arongranberg.com/astar/docs_dev/calling-pathfinding.php
	// find a path to player 
	// choose the tile that is furthest on that path
	// we need to look for a path to the unocuppied tile next to the player, if it exists
	void FindPath()
	{
		Path p = seeker.StartPath(transform.position, targetCharacter.transform.position, GoToBestTile);
	}

	void GoToBestTile(Path p)
	{
		Vector3Int tilePos;

		//We got our path back
		if (!p.error)
		{
			// Yay, now we can get a Vector3 representation of the path
			// from p.vectorPath
			// loop from the target to self
			for (int i = p.vectorPath.Count - 1; i >= 0; i--)
			{
				tilePos = tilemap.WorldToCell(p.vectorPath[i]);
				if (tiles.TryGetValue(tilePos, out _tile))
				{
					// check if it is within reach and is not the tile I am currently standing on
					if (_tile.WithinRange && _tile != currentTile)
					{
						// return it's transform
						enemyCharMovementController.GoToDestination(new Vector3(_tile.LocalPlace.x, _tile.LocalPlace.y, _tile.LocalPlace.z));
						return;
					}
				}
			}

			// no within range tile that is on path
			//selecting a random tile
			// TODO: something smarter
			WorldTile randomTile = highlighter.highlightedTiles[Random.Range(0, highlighter.highlightedTiles.Count)];
			enemyCharMovementController.GoToDestination(new Vector3(randomTile.LocalPlace.x, randomTile.LocalPlace.y, randomTile.LocalPlace.z));
		}
	}
}
