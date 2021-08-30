using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class FMMapManager : MonoBehaviour
{
	[SerializeField]
	Tilemap map;
	[SerializeField]
	List<MyTileData> myTileDatas;
	Dictionary<TileBase, MyTileData> dataFromTiles;
	[SerializeField]

	public static FMMapManager instance;

	void Awake()
	{
		// singleton
		if (instance == null)
		{
			instance = this;
		}
		else if (instance != this)
		{
			Destroy(gameObject);
		}

		dataFromTiles = new Dictionary<TileBase, MyTileData>();
		foreach (var tileData in myTileDatas)
		{
			foreach (var tile in tileData.tiles)
			{
				dataFromTiles.Add(tile, tileData);
			}
		}
	}

	public bool IsObstacle(Vector3Int pos)
	{
		TileBase tile = map.GetTile(pos);
		if (tile == null)
			return false;
		if (!dataFromTiles.ContainsKey(tile))
			return false;

		return dataFromTiles[tile].obstacle;
	}

	public int GetTileDamage(Vector3Int pos)
	{
		TileBase tile = map.GetTile(pos);
		if (tile == null)
			return 0;

		return dataFromTiles[tile].tileDamage;
	}
}
