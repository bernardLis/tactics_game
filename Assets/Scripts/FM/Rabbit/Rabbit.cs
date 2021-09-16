using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Rabbit : MonoBehaviour
{
	// tilemap
	protected Tilemap tilemap;
	protected Dictionary<Vector3, WorldTile> tiles;
	protected WorldTile _tile;

	// animation TODO: a separate script?
	protected Animator animator;

	protected GameObject tempObject;
	protected bool targetReached;

	public float moveSpeed = 1f;
	public float randomMoveRange = 1f;
	protected float nextRandomMove = 0f;
	public float randomMoveDelayMin = 1f;
	public float randomMoveDelayMax = 5f;

	public RabbitSpawner rabbitSpawner;

	protected virtual void Awake()
	{
		tilemap = TileMapInstance.instance.GetComponent<Tilemap>();
		tiles = GameTiles.instance.tiles; // This is our Dictionary of tiles

		animator = GetComponentInChildren<Animator>();
	}

	void Start()
	{
		RandomMove();
	}

	protected virtual void Update()
	{
		if (tempObject != null)
			transform.position = Vector3.MoveTowards(transform.position,
							tempObject.transform.position, moveSpeed * Time.deltaTime);

		// target reached;
		if (tempObject != null && !targetReached && Vector3.Distance(transform.position, tempObject.transform.position) < 0.01f)
			TargetReached();
	}

	protected virtual void TargetReached()
	{
		animator.SetFloat("speed", 0f);
		Destroy(tempObject);
		targetReached = true;
	}

	protected void RandomMove()
	{
		if (tempObject != null)
			Destroy(tempObject);

		float x = Random.Range(-randomMoveRange, randomMoveRange);
		float y = Random.Range(-randomMoveRange, randomMoveRange);
		Vector3 newRandomPosition = transform.position + new Vector3(x, y, 0f);
		newRandomPosition.z = 0f;

		// check if the tile is not an obstacle
		Vector3Int tilePos = tilemap.WorldToCell(newRandomPosition);
		if (tiles.TryGetValue(tilePos, out _tile))
		{
			if (_tile.IsObstacle)
			{
				// TODO: I would like to start over, is that code alright? 
				RandomMove();
				return;
			}
		}

		tempObject = new GameObject("rabbit random pos");
		tempObject.transform.position = newRandomPosition;
		targetReached = false;
		moveSpeed = 1f;

		animator.SetFloat("speed", 1f);

		nextRandomMove += Random.Range(randomMoveDelayMin, randomMoveDelayMax);

		SetDirection(tempObject.transform);
	}


	// animation
	protected void SetDirection(Transform target)
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


	int DirectionToIndex(Vector2 dir, int sliceCount)
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
