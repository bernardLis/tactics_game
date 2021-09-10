using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

using Random = UnityEngine.Random;

public enum RabbitState { IDLE, FLEEING, CORNERED, CAPTURED }
public class WildRabbit : Rabbit
{
	Transform player;
	float detectionRadius = 4f;
	float sneakDetectionRadius = 1f;

	FMPlayerMovementController playerController;
	FMPlayerInteractionController playerInteractionController;

	RabbitState state;
	int retryCounter;
	Vector3 startPosition;

	[SerializeField]
	GameObject poofEffect;
	[SerializeField]
	GameObject starEffect;
	[SerializeField]
	Item rabbitItem;


	//	public event Action rabbitHides;

	protected override void Awake()
	{
		base.Awake();


		state = RabbitState.IDLE;
	}

	void Start()
	{
		startPosition = transform.position;
		player = GameObject.FindGameObjectWithTag("Player").transform;

		if (player != null)
			playerController = player.GetComponent<FMPlayerMovementController>();
		playerInteractionController = player.GetComponent<FMPlayerInteractionController>();
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
			if (hitCollider.CompareTag("PlayerCollider") && !playerController.isSneaking && state != RabbitState.FLEEING)
				Flee();
		}

		Collider2D[] sneakColliders = Physics2D.OverlapCircleAll(transform.position, sneakDetectionRadius);
		foreach (var hitCollider in sneakColliders)
		{
			if (hitCollider.CompareTag("PlayerCollider") && state != RabbitState.FLEEING)
				Flee();
		}
	}

	void OnCollisionEnter2D(Collision2D col)
	{
		// if you hit unpassable layer, you are cornered
		// TODO: something a bit smarter.
		if (col.gameObject.layer == 3)
			Cornered();
		if (col.gameObject.CompareTag("Player"))
			Captured();

		//Destroy(Instantiate(poofEffect, transform.position, Quaternion.identity) as GameObject, 1f);
	}

	void Captured()
	{
		print("player hit, you are captured");
		state = RabbitState.CAPTURED;
		Destroy(Instantiate(starEffect, transform.position, Quaternion.identity) as GameObject, 1f);

		// TODO: inventory sys
		rabbitItem.PickUp();
		playerInteractionController.rabbitsCaught++;

		TargetReached();
		Destroy(gameObject);
	}



	void Flee()
	{
		if (state != RabbitState.FLEEING && state != RabbitState.CORNERED && state != RabbitState.CAPTURED)
		{
			retryCounter++;
			if (retryCounter >= 10)
			{
				Cornered();
				return;
			}

			state = RabbitState.FLEEING;

			if (tempObject != null)
				Destroy(tempObject);

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
				if (_tile.IsObstacle)
				{
					// TODO: I would like to start over, is that alright? 
					state = RabbitState.IDLE;
					Flee();
					return;
				}
			}
			else
			{
				// probably reached the end of the map
				Cornered();
				return;
			}

			// we have managed to get through all the checks now we can really flee
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

	void Cornered()
	{
		if (state != RabbitState.CORNERED)
		{
			state = RabbitState.CORNERED;
			TargetReached();

			if (tempObject != null)
				Destroy(tempObject);

			Destroy(Instantiate(poofEffect, transform.position, Quaternion.identity) as GameObject, 1f);
			/*
			// Action
			if (rabbitHides != null)
			{
				rabbitHides();
			}
			*/
			// TODO: dunno if correct
			rabbitSpawner.SpawnRabbit();


			Destroy(gameObject);
		}
		/*
		// TODO: make capture circle smaller
		tempObject = new GameObject("rabbit start pos");
		tempObject.transform.position = startPosition;
		targetReached = false;

		moveSpeed = 5f;
		animator.SetFloat("speed", 1f);
		SetDirection(tempObject.transform);
		*/
	}

	protected override void TargetReached()
	{
		base.TargetReached();
		state = RabbitState.IDLE;
	}

}
