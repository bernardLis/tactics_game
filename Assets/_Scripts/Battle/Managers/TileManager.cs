using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

//https://medium.com/@allencoded/unity-tilemaps-and-storing-individual-tile-data-8b95d87e9f32
public class TileManager : MonoBehaviour
{
    public Tilemap Tilemap;
    [SerializeField] List<MyTileData> _myTileDatas;
    Dictionary<TileBase, MyTileData> _dataFromTiles;

    public static Dictionary<Vector3, WorldTile> Tiles;

    void Awake()
    {
        _dataFromTiles = new Dictionary<TileBase, MyTileData>();
        foreach (var tileData in _myTileDatas)
            foreach (var tile in tileData.Tiles)
                _dataFromTiles.Add(tile, tileData);
    }

    public void SetUp()
    {
        CreateWorldTiles();
        CreateBoundsColliders();
    }

    void CreateWorldTiles()
    {
        Tiles = new Dictionary<Vector3, WorldTile>();
        foreach (Vector3Int pos in Tilemap.cellBounds.allPositionsWithin)
        {
            Vector3Int localPlace = new Vector3Int(pos.x, pos.y, pos.z);
            bool _isObstacle = IsObstacle(localPlace);

            if (!Tilemap.HasTile(localPlace)) continue;

            var tile = new WorldTile
            {
                LocalPlace = localPlace,
                WorldLocation = Tilemap.CellToWorld(localPlace),
                TileBase = Tilemap.GetTile(localPlace),
                TilemapMember = Tilemap,
                Name = localPlace.x + "," + localPlace.y,
                Cost = 1, // TODO: Could be used for tiles with walk penalties
                IsObstacle = _isObstacle,
                WithinRange = false,
            };

            Tiles.Add(tile.WorldLocation, tile);
        }
    }

    void CreateBoundsColliders()
    {
        GameObject BoundColliders = new("BoundColliders");
        foreach (KeyValuePair<Vector3, WorldTile> PosTile in Tiles)
        {
            if (PosTile.Value.IsObstacle)
            {
                // create gameobject with collider at it's position
                GameObject col = new("col");
                col.tag = Tags.BoundCollider;
                col.layer = 3; // unpassable layer
                col.transform.position = new Vector3(PosTile.Key.x + 0.5f, PosTile.Key.y + 0.5f, 0f);

                BoxCollider2D bc = col.AddComponent(typeof(BoxCollider2D)) as BoxCollider2D;
                bc.size = Vector2.one;
                col.transform.parent = BoundColliders.transform;
            }
        }
    }

    public bool IsObstacle(Vector3Int pos)
    {
        TileBase tile = Tilemap.GetTile(pos);
        if (tile == null)
            return false;
        if (!_dataFromTiles.ContainsKey(tile))
            return false;

        return _dataFromTiles[tile].isObstacle;
    }
}
