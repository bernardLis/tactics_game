using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WorldTile
{
	public Vector3Int LocalPlace { get; set; }

	public Vector3 WorldLocation { get; set; }

	public TileBase TileBase { get; set; }

	public Tilemap TilemapMember { get; set; }

	public string Name { get; set; }

	// Below is needed for Breadth First Searching
	public bool IsExplored { get; set; }

	public WorldTile ExploredFrom { get; set; }

	public int Cost { get; set; }
	public bool IsObstacle { get; set; }
	public int TileDamage { get; set; }

	public bool WithinRange { get; set; }



	public void Highlight(Color col)
	{
		TilemapMember.SetTileFlags(LocalPlace, TileFlags.None);
		TilemapMember.SetColor(LocalPlace, col);
	}


	public void ClearHighlightAndTags()
	{
		TilemapMember.SetTileFlags(LocalPlace, TileFlags.None);
		TilemapMember.SetColor(LocalPlace, Color.white);
		WithinRange = false;

	}

}
