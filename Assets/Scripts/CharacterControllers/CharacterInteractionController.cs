using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;


public class CharacterInteractionController : MonoBehaviour
{

	protected Highlighter highlighter;
	protected CharacterRenderer characterRenderer;

	// tiles
	protected Dictionary<Vector3, WorldTile> tiles;
	protected Tilemap tilemap;
	protected WorldTile _tile;

	protected CharacterStats myStats;
	protected CharacterStats targetStats;
	public Ability selectedAbility;

	protected virtual void Awake()
	{
		highlighter = GameManager.instance.GetComponent<Highlighter>();

		characterRenderer = GetComponentInChildren<CharacterRenderer>();

		tilemap = TileMapInstance.instance.GetComponent<Tilemap>();
		tiles = GameTiles.instance.tiles; // This is our Dictionary of tiles

		myStats = (CharacterStats)transform.GetComponent<CharacterStats>();
	}

	public virtual void Face(Vector2 direction)
	{
		// small vector to stand still
		characterRenderer.SetDirection(direction * 0.01f);
	}
	public virtual void FinishCharacterTurn()
	{
		// it's meant to be overwritten
	}

}
