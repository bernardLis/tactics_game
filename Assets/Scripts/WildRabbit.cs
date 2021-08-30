using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RabbitState { IDLE, FLEEING, CORNERED, CAPTURED }
public class WildRabbit : Rabbit
{
	Transform player;
	float detectionRadius = 4f;
	FMPlayerMovementController playerController;

	RabbitState state;
	int retryCounter;

	protected override void Awake()
	{
		base.Awake();


		state = RabbitState.IDLE;
	}

	void Start()
	{
		player = GameObject.FindGameObjectWithTag("Player").transform;

		if (player != null)
			playerController = player.GetComponent<FMPlayerMovementController>();
	}

	protected override void Update()
	{
		base.Update();
		// Rabbit moves randomly from time to time
		if (state == RabbitState.IDLE && Time.time > nextRandomMove)
		{
			RandomMove();
		}

		// TODO: is this a correct implementation of what I want to do?
		// Rabbit flees from the player 
		Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, detectionRadius);
		foreach (var hitCollider in colliders)
		{
			if (hitCollider.CompareTag("PlayerCollider"))
			{
				if (!playerController.isSneaking && state != RabbitState.FLEEING)
				{
					Flee();
				}
			}
		}
	}


	void Flee()
	{
		if (state != RabbitState.FLEEING)
		{
			retryCounter++;
			if (retryCounter >= 10)
			{
				print("rabbit flee retry counter more than 10");
				state = RabbitState.CORNERED;
				// TODO: maybe also check if we are moving towards but not making progress and create a function that runs to somewhere
				// and makes the capture circle small so player not always wins when they corner a rabbit

			}

			state = RabbitState.FLEEING;
			if (tempObject != null)
			{
				Destroy(tempObject);
			}

			// go further from player
			Vector3 direction = (transform.position - player.transform.position).normalized;
			float rng = 0.5f;
			direction = direction + new Vector3(Random.Range(-rng, rng), Random.Range(-rng, rng), 0f);

			Vector3 fleePosition = transform.position + (direction * Random.Range(2f, 4f));
			fleePosition.z = 0f;

			// check if the tile is not an obstacle
			Vector3Int tilePos = tilemap.WorldToCell(fleePosition);
			if (tiles.TryGetValue(tilePos, out _tile))
			{
				print("if out tile");
				if (_tile.IsObstacle)
				{
					print("tile is obstacle");

					// TODO: I would like to start over, is that alright? 
					state = RabbitState.IDLE;
					Flee();
					return;
				}
			}
			else
			{
				print("I have probably reached end of the map coz I am choosing destination without tile.");
				state = RabbitState.IDLE;
				Flee();
				return;
			}

			retryCounter = 0;

			tempObject = new GameObject("rabbit flee pos");
			tempObject.transform.position = fleePosition;
			targetReached = false;

			moveSpeed = 5f;
			animator.SetFloat("speed", 1f);
			SetDirection(tempObject.transform);


			// I could use manhatann distance and check whether the tile we are picking is not an obstacle
			// and check like movement range of 7 for example;
			// if we are cornered we should disappear 
		}
	}

	protected override void TargetReached()
	{
		base.TargetReached();
		state = RabbitState.IDLE;
	}

}
