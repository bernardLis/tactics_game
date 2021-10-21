using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(AILerp))]
[RequireComponent(typeof(AIDestinationSetter))]
public class CharacterMovementController : AILerp
{
	public Highlighter highlighter;
	// A*
	public AIDestinationSetter destinationSetter;
	public AILerp AILerp;
	public CharacterRenderer _characterRenderer;

	public Dictionary<Vector3, WorldTile> tiles;
	public Tilemap tilemap;
	protected override void Awake()
	{
		base.Awake();
		highlighter = GameManager.instance.GetComponent<Highlighter>();

		destinationSetter = GetComponent<AIDestinationSetter>();
		AILerp = GetComponent<AILerp>();
		_characterRenderer = GetComponentInChildren<CharacterRenderer>();

		// This is our Dictionary of tiles
		tiles = GameTiles.instance.tiles;
		tilemap = TileMapInstance.instance.GetComponent<Tilemap>();
		
		SnapToGrid();
	}


	public override void OnTargetReached()
	{
		base.OnTargetReached();

		AILerp.canSearch = false;
		AILerp.canMove = false;

		_characterRenderer.SetDirection(Vector2.zero);

		SnapToGrid();
	}

	public void SnapToGrid()
	{
		float x = transform.position.x;
		float y = transform.position.y;
		//https://answers.unity.com/questions/714197/round-to-05-15-25-.html
		float outputX = Mathf.Sign(x) * (Mathf.Abs((int)x) + 0.5f);
		float outputY = Mathf.Sign(y) * (Mathf.Abs((int)y) + 0.5f);

		if (outputX != transform.position.x || outputY != transform.position.y)
		{
			transform.position = new Vector3(outputX, outputY, transform.position.z);
		}
	}

}