using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class HealerEnemyAI : EnemyAI
{
	GameObject[] enemyCharacters;
	int lowestHealth;

	protected override void Awake()
	{
		base.Awake();

		// giving lowest health a value
		lowestHealth = int.MaxValue;
	}

	public override IEnumerator RunAI()
	{
		yield return StartCoroutine(base.RunAI());

		// TODO: this is a repetition from the base coroutine,
		// idk how to make base coroutine exit both coroutines.
		// exit if battle is over
		if (TurnManager.battleState == BattleState.WON || TurnManager.battleState == BattleState.LOST)
		{
			yield break;
		}

		// interact with target 
		if (!enemyCharMovementController.trappedOnTheWay)
		{
			// manhattan distance to see whether we are in range
			int manDistance = Mathf.RoundToInt(Mathf.Abs(transform.position.x - targetCharacter.transform.position.x)
												+ Mathf.Abs(transform.position.y - targetCharacter.transform.position.y));

			// TODO: don't hardcore abilities
			if (manDistance <= myStats.abilities[1].range)
			{
				CharacterStats targetStats = targetCharacter.GetComponent<CharacterStats>();
				if (targetStats.currentHealth < targetStats.maxHealth.GetValue())
				{
					myStats.abilities[1].HighlightTargetable();
					yield return new WaitForSeconds(0.5f);
					enemyInteractionController.Heal(targetCharacter);
				}
				// TODO: so many elses that do the same, can I change that?
				else
				{
					// TODO: face closest player
					enemyInteractionController.FaceAndFinishInteraction(targetCharacter);
				}
			}
			else
			{
				// TODO: face closest player
				enemyInteractionController.FaceAndFinishInteraction(targetCharacter);
			}
			// clearing the highlight and finishing the turn
			yield return new WaitForSeconds(1f);
			highlighter.ClearHighlightedTiles();
		}
		// or just face its direction
		else
		{
			// TODO: face closest player
			enemyInteractionController.FaceAndFinishInteraction(targetCharacter);
		}

		TurnManager.instance.EnemyCharacterTurnFinished();
		yield return true;
	}

	protected override GameObject GetTargetCharacter()
	{
		// TODO: you can check reachability by calculating walking range & ability range 
		// manhatann distance between target and self
		// get list of allied characters
		enemyCharacters = GameObject.FindGameObjectsWithTag("Enemy");

		// see which has lowest health 
		foreach (GameObject enemy in enemyCharacters)
		{
			// check if enemy's health is lower than his max health
			CharacterStats stats = enemy.GetComponent<CharacterStats>();
			if (stats.currentHealth < stats.maxHealth.GetValue())
			{
				if (stats.currentHealth <= lowestHealth)
				{
					lowestHealth = stats.currentHealth;
					targetCharacter = enemy;
				}
			}
		}
		// TODO: what if we end up with no targetCharacter
		if (targetCharacter == null)
		{
			targetCharacter = enemyCharacters[0];
			// prioritize hanging around your teammates
			// except if everyone but you is dead : enemyCharacters.Length > 1
			if (targetCharacter == gameObject && enemyCharacters.Length > 1)
			{
				targetCharacter = enemyCharacters[1];
			}
		}
		// that's the target
		return targetCharacter;
	}

	protected override void GetDestination(GameObject target)
	{
		// if we are the target character
		// run away from the closest player char
		if (target == gameObject)
		{
			// get player characters
			playerCharacters = GameObject.FindGameObjectsWithTag("Player");

			GameObject closestPlayer = playerCharacters[0];
			int distanceToClosestPlayer = Mathf.RoundToInt(Mathf.Abs(transform.position.x - playerCharacters[0].transform.position.x)
																		+ Mathf.Abs(transform.position.y - playerCharacters[0].transform.position.y));
			foreach (GameObject player in playerCharacters)
			{
				int dist = Mathf.RoundToInt(Mathf.Abs(transform.position.x - player.transform.position.x)
																		+ Mathf.Abs(transform.position.y - player.transform.position.y));
				if (dist < distanceToClosestPlayer)
				{
					closestPlayer = player;
					distanceToClosestPlayer = dist;
				}
			}

			// now we have closest player, we gotta choose a tile to run away from him
			targetTile = null;

			float maxDistanceFromTarget = distanceToClosestPlayer;
			// maybe from tiles in range you can find a tile that furthest from the player 
			foreach (WorldTile tile in highlighter.highlightedTiles)
			{
				// I want to maximize manDist from target 
				int distance = Mathf.RoundToInt(Mathf.Abs(tile.LocalPlace.x - closestPlayer.transform.position.x)
											 + Mathf.Abs(tile.LocalPlace.y - closestPlayer.transform.position.y));

				// but I also need to stay in attack range
				if (distance > maxDistanceFromTarget)
				{
					maxDistanceFromTarget = distance;
					targetTile = tile;
				}
			}
			// if we finish without a better tile stay;
			if (targetTile == null)
			{
				Vector3Int tilePos = tilemap.WorldToCell(transform.position);
				if (tiles.TryGetValue(tilePos, out _tile))
				{
					targetTile = _tile;
				}
			}

			enemyCharMovementController.GoToDestination(new Vector3(targetTile.LocalPlace.x, targetTile.LocalPlace.y, targetTile.LocalPlace.z));
			return;
		}

		// TODO: make it smarter - ex. keep your distance from player characters
		// check how far is the target
		int distanceFromTarget = Mathf.RoundToInt(Mathf.Abs(transform.position.x - target.transform.position.x)
															+ Mathf.Abs(transform.position.y - target.transform.position.y));
		abilityRange = myStats.abilities[1].range;

		// if they are exactly in range
		if (distanceFromTarget == abilityRange)
		{
			targetInRange = true;
			enemyCharMovementController.GoToDestination(transform.position);
		}
		// you can move away from target
		else if (distanceFromTarget < abilityRange)
		{
			targetTile = null;
			targetInRange = true;

			float maxDistanceFromTarget = distanceFromTarget;
			// maybe from tiles in range you can find a tile that furthest from the player 
			// but you could still attack from it  
			foreach (WorldTile tile in highlighter.highlightedTiles)
			{
				// I want to maximize manDist from target 
				int distance = Mathf.RoundToInt(Mathf.Abs(tile.LocalPlace.x - target.transform.position.x)
											 + Mathf.Abs(tile.LocalPlace.y - target.transform.position.y));

				// but I also need to stay in attack range
				if (distance > maxDistanceFromTarget && distance < abilityRange)
				{
					maxDistanceFromTarget = distance;
					targetTile = tile;
				}
			}

			// if we finish without a better tile stay;
			if (targetTile == null)
			{
				Vector3Int tilePos = tilemap.WorldToCell(transform.position);
				if (tiles.TryGetValue(tilePos, out _tile))
				{
					targetTile = _tile;
				}
			}

			enemyCharMovementController.GoToDestination(new Vector3(targetTile.LocalPlace.x, targetTile.LocalPlace.y, targetTile.LocalPlace.z));
		}
		// you need to come closer to the target
		else
		{
			// find best tile calls seeker to pathfind;
			FindPath();
		}
	}

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
			// selecting a random tile
			// TODO: something smarter
			WorldTile randomTile = highlighter.highlightedTiles[Random.Range(0, highlighter.highlightedTiles.Count)];
			enemyCharMovementController.GoToDestination(new Vector3(randomTile.LocalPlace.x, randomTile.LocalPlace.y, randomTile.LocalPlace.z));
		}
	}
}
