using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using UnityEngine.Tilemaps;

public enum RabbitState { IDLE, FLEEING, CAPTURED }
public class RabbitScript : MonoBehaviour
{
	// pathfinding
	AIDestinationSetter destinationSetter;
	AILerp AILerp;

	// tilemap
	Tilemap tilemap;
	Dictionary<Vector3, WorldTile> tiles;
	WorldTile _tile;

	// animation TODO: a separate script?
	Animator animator;

	public Transform player;
	float detectionRadius = 4f;
	FMPlayerMovementController playerController;
	GameObject tempObject;
	bool targetReached;

	RabbitState state;
	float moveSpeed = 1f;
	float nextRandomMove = 0f;
	float randomMoveDelay = 5f;

	void Awake()
	{
		destinationSetter = GetComponent<AIDestinationSetter>();
		AILerp = GetComponent<AILerp>();

		tilemap = TileMapInstance.instance.GetComponent<Tilemap>();
		tiles = GameTiles.instance.tiles; // This is our Dictionary of tiles

		animator = GetComponentInChildren<Animator>();

		playerController = player.GetComponent<FMPlayerMovementController>();

		state = RabbitState.IDLE;
	}

	void Start()
	{
		RandomMove();
	}

	void Update()
	{
		if (tempObject != null)
		{
			transform.position = Vector3.MoveTowards(transform.position,
							tempObject.transform.position, moveSpeed * Time.deltaTime);
		}

		// Rabbit moves randomly from time to time
		// TODO: move randomly;
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

		// target reached;
		if (tempObject != null && !targetReached)
		{
			if (Vector3.Distance(transform.position, tempObject.transform.position) < 0.1f)
			{
				animator.SetFloat("speed", 0f);
				Destroy(tempObject);
				targetReached = true;
				state = RabbitState.IDLE;
			}
		}
	}

	void RandomMove()
	{
		nextRandomMove += randomMoveDelay;


		// TODO: need to check if this is reachable
		if (tempObject != null)
		{
			Destroy(tempObject);
		}
		float x = Random.Range(-1f, 1f);
		float y = Random.Range(-1f, 1f);
		Vector3 newRandomPosition = transform.position + new Vector3(x, y, 0f);
		newRandomPosition.z = 0f;

		tempObject = new GameObject("rabbit random pos");
		tempObject.transform.position = newRandomPosition;
		targetReached = false;
		moveSpeed = 1f;
		/*
		destinationSetter.target = tempObject.transform;
		AILerp.speed = 1f;
		*/
		animator.SetFloat("speed", 1f);

		SetDirection(tempObject.transform);
	}

	void Flee()
	{
		if (state != RabbitState.FLEEING)
		{
			state = RabbitState.FLEEING;

			// get a tile that is further from player
			Vector3 direction = (transform.position - player.transform.position).normalized;

			// TODO: I could add some random number to it
			Vector3 fleePosition = transform.position + (direction * 3f);
			fleePosition.z = 0f;

			if (tempObject != null)
			{
				Destroy(tempObject);
			}

			tempObject = new GameObject("rabbit flee pos");
			// TODO: need to check if this is reachable
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

	// animation
	void SetDirection(Transform target)
	{
		animator.SetBool("north", false);
		animator.SetBool("south", false);
		animator.SetBool("east", false);
		animator.SetBool("west", false);

		Vector2 dir = target.transform.position - transform.position;
		int dirToInt = DirectionToIndex(dir, 4);
		if (dirToInt == 0)
		{
			animator.SetBool("north", true);
		}
		else if (dirToInt == 1)
		{

			animator.SetBool("west", true);
		}
		else if (dirToInt == 2)
		{
			animator.SetBool("south", true);
		}
		else
		{
			animator.SetBool("east", true);
		}
	}


	public static int DirectionToIndex(Vector2 dir, int sliceCount)
	{
		//get the normalized direction
		Vector2 normDir = dir.normalized;
		//calculate how many degrees one slice is
		float step = 360f / sliceCount;
		//calculate how many degress half a slice is.
		//we need this to offset the pie, so that the North (UP) slice is aligned in the center
		float halfstep = step / 2;
		//get the angle from -180 to 180 of the direction vector relative to the Up vector.
		//this will return the angle between dir and North.
		float angle = Vector2.SignedAngle(Vector2.up, normDir);

		//add the halfslice offset
		angle += halfstep;
		//if angle is negative, then let's make it positive by adding 360 to wrap it around.
		if (angle < 0)
		{
			angle += 360;
		}
		//calculate the amount of steps required to reach this angle
		float stepCount = angle / step;
		//round it, and we have the answer!
		return Mathf.FloorToInt(stepCount);
	}

}
