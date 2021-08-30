using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


//https://medium.com/@allencoded/unity-tilemaps-and-storing-individual-tile-data-8b95d87e9f32
public class FMGameTiles : MonoBehaviour
{
	public static FMGameTiles instance;
	public Tilemap Tilemap;

	public Dictionary<Vector3, WorldTile> tiles;

	void Awake()
	{
		if (instance == null)
		{
			instance = this;
		}
		else if (instance != this)
		{
			Destroy(gameObject);
		}

		GetWorldTiles();
		CreateObstacleColliders();
	}
	void Start()
	{
	}

	// Use this for initialization
	void GetWorldTiles()
	{
		tiles = new Dictionary<Vector3, WorldTile>();
		foreach (Vector3Int pos in Tilemap.cellBounds.allPositionsWithin)
		{
			Vector3Int localPlace = new Vector3Int(pos.x, pos.y, pos.z);
			bool _isObstacle = FMMapManager.instance.IsObstacle(localPlace);
			int _tileDamage = 0;

			if (!Tilemap.HasTile(localPlace)) continue;

			var tile = new WorldTile
			{
				LocalPlace = localPlace,
				WorldLocation = Tilemap.CellToWorld(localPlace),
				TileBase = Tilemap.GetTile(localPlace),
				TilemapMember = Tilemap,
				Name = localPlace.x + "," + localPlace.y,
				Cost = 1, // TODO: Change this with the proper cost from ruletile
				IsObstacle = _isObstacle,
				TileDamage = _tileDamage,
				WithinRange = false,
			};

			tiles.Add(tile.WorldLocation, tile);
		}
	}

	void CreateObstacleColliders()
	{
		GameObject ObsctacleColliders = new GameObject();
		ObsctacleColliders.name = "ObsctacleColliders";

		foreach (KeyValuePair<Vector3, WorldTile> PosTile in tiles)
		{
			if (PosTile.Value.IsObstacle)
			{
				// create gameobject with collider at it's position
				GameObject col = new GameObject();
				col.name = "col";
				col.layer = 3; // unpassable layer

				Rigidbody2D rb = col.AddComponent<Rigidbody2D>();
				rb.bodyType = RigidbodyType2D.Static;

				BoxCollider2D bc = col.AddComponent(typeof(BoxCollider2D)) as BoxCollider2D;
				bc.size = Vector2.one;
				bc.offset = new Vector2(PosTile.Key.x + 0.5f, PosTile.Key.y + 0.5f);

				// parent the object
				col.transform.parent = ObsctacleColliders.transform;
			}
		}
	}

}
