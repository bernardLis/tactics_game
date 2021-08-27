using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public class CharacterSelection : MonoBehaviour
{
	public Highlighter highlighter;

	// https://medium.com/@allencoded/unity-tilemaps-and-storing-individual-tile-data-8b95d87e9f32
	public Tilemap tilemap;
	public WorldTile _tile;
	public Dictionary<Vector3, WorldTile> tiles;

	public CharacterStats myStats;
	public int range;

	public virtual void Awake()
	{
		highlighter = GameManager.instance.GetComponent<Highlighter>();

		tilemap = TileMapInstance.instance.GetComponent<Tilemap>();
		myStats = GetComponent<CharacterStats>();
		tiles = GameTiles.instance.tiles; // This is our Dictionary of tiles
	}

	public virtual void HiglightMovementRange()
	{
		range = myStats.movementRange.GetValue();
	}
}
