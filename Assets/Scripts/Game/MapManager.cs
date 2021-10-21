using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapManager : MonoBehaviour
{
	Tilemap tileMap;

	[SerializeField]
	List<MyTileData> myTileDatas;

	Dictionary<TileBase, MyTileData> dataFromTiles;

	public static MapManager instance;

	void Awake()
	{
		tileMap = TileMapInstance.instance.GetComponent<Tilemap>();

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
		TileBase tile = tileMap.GetTile(pos);
		if (tile == null)
			return false;
		if (!dataFromTiles.ContainsKey(tile))
			return false;

		return dataFromTiles[tile].obstacle;
	}

	public int GetTileDamage(Vector3Int pos)
	{
		TileBase tile = tileMap.GetTile(pos);
		if (tile == null)
			return 0;

		return dataFromTiles[tile].tileDamage;
	}
}
