using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

//https://medium.com/@allencoded/unity-tilemaps-and-storing-individual-tile-data-8b95d87e9f32
public class TileManager : MonoBehaviour
{
    public Tilemap tilemap;
    [SerializeField]
    List<MyTileData> myTileDatas;
    Dictionary<TileBase, MyTileData> dataFromTiles;

    public static Dictionary<Vector3, WorldTile> tiles;

    public static TileManager instance;

    void Awake()
    {
        // singleton
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        dataFromTiles = new Dictionary<TileBase, MyTileData>();
        foreach (var tileData in myTileDatas)
            foreach (var tile in tileData.tiles)
                dataFromTiles.Add(tile, tileData);
    }

    public void SetUp()
    {
        CreateWorldTiles();
        CreateObstacleColliders();
    }

    void CreateWorldTiles()
    {
        tiles = new Dictionary<Vector3, WorldTile>();
        foreach (Vector3Int pos in tilemap.cellBounds.allPositionsWithin)
        {
            Vector3Int localPlace = new Vector3Int(pos.x, pos.y, pos.z);
            bool _isObstacle = IsObstacle(localPlace);

            if (!tilemap.HasTile(localPlace)) continue;

            var tile = new WorldTile
            {
                LocalPlace = localPlace,
                WorldLocation = tilemap.CellToWorld(localPlace),
                TileBase = tilemap.GetTile(localPlace),
                TilemapMember = tilemap,
                Name = localPlace.x + "," + localPlace.y,
                Cost = 1, // TODO: Could be used for tiles with walk penalties
                IsObstacle = _isObstacle,
                WithinRange = false,
            };

            tiles.Add(tile.WorldLocation, tile);
        }
    }

    void CreateObstacleColliders()
    {
        GameObject ObsctacleColliders = new();
        ObsctacleColliders.name = "ObsctacleColliders";

        // TODO: does not work
        foreach (KeyValuePair<Vector3, WorldTile> PosTile in tiles)
        {
            if (PosTile.Value.IsObstacle)
            {
                // create gameobject with collider at it's position
                GameObject col = new();
                col.name = "col";
                col.layer = 3; // unpassable layer
                               // positioning the game object;
                col.transform.position = new Vector3(PosTile.Key.x + 0.5f, PosTile.Key.y + 0.5f, 0f);

                BoxCollider2D bc = col.AddComponent(typeof(BoxCollider2D)) as BoxCollider2D;
                bc.size = Vector2.one;
                //bc.offset = new Vector2(PosTile.Key.x + 0.5f, PosTile.Key.y + 0.5f);

                // parent the object
                col.transform.parent = ObsctacleColliders.transform;
            }
        }
    }

    public bool IsObstacle(Vector3Int pos)
    {
        TileBase tile = tilemap.GetTile(pos);
        if (tile == null)
            return false;
        if (!dataFromTiles.ContainsKey(tile))
            return false;

        return dataFromTiles[tile].obstacle;
    }
}
