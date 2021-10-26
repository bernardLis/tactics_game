using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Profiling;
public class Highlighter : MonoBehaviour
{
    public static Highlighter instance;

    [SerializeField]
    GameObject flasherHolder;
    [SerializeField]
    GameObject flasherPrefab;

    //TODO: this script could be better.
    Tilemap tilemap;
    Dictionary<Vector3, WorldTile> tiles;

    WorldTile _tile;
    WorldTile charTile;

    public List<WorldTile> highlightedTiles = new List<WorldTile>();

    //flashers
    List<GameObject> flashers = new List<GameObject>();

    [Header("Flasher")]
    public float flasherXOffset = 0.4f;
    public float flasherYOffset = 0.6f;



    protected virtual void Awake()
    {
        // singleton
        if (instance != null)
        {
            Debug.LogWarning("More than one instance of Highlighter found");
            return;
        }
        instance = this;

        tilemap = TileMapInstance.instance.GetComponent<Tilemap>();
        tiles = GameTiles.instance.tiles; // This is our Dictionary of tiles
    }

    public WorldTile HighlightSingle(Vector3 position, Color col)
    {
        // making sure there is only one highlight at the time
        ClearHighlightedTiles();

        Vector3Int tilePos = tilemap.WorldToCell(position);
        if (tiles.TryGetValue(tilePos, out _tile))
        {
            HighlightTile(_tile, col);
            return _tile;
        }
        return null;
    }

    // TODO: this
    public void HighlightRectangle()
    {

    }

    // TODO: this is a mess...
    public void HighlightTiles(Vector3 position, int range, Color col, bool diagonal, bool self)
    {
        // making sure there is only one highlight at the time
        ClearHighlightedTiles();

        var markedTiles = new List<WorldTile>();

        // get the tile character is currently standing on
        Vector3Int tilePos = tilemap.WorldToCell(position);
        Vector3Int worldPoint;

        if (tiles.TryGetValue(tilePos, out _tile))
        {
            markedTiles.Add(_tile);
            charTile = _tile;
        }

        for (int i = 0; i < range; i++)
        {
            // for each tile in marked tiles
            var newMarkedTiles = new List<WorldTile>();

            foreach (WorldTile markedTile in markedTiles)
            {
                // +/-  x
                for (int x = -1; x <= 1; x++)
                {
                    // excluding diagonals
                    if (!diagonal)
                    {
                        worldPoint = new Vector3Int(markedTile.LocalPlace.x + x, charTile.LocalPlace.y, 0);
                    }
                    else
                    {
                        worldPoint = new Vector3Int(markedTile.LocalPlace.x + x, markedTile.LocalPlace.y, 0);
                    }

                    if (tiles.TryGetValue(worldPoint, out _tile))
                    {
                        // excluding self
                        if (!self)
                        {
                            if (!_tile.IsObstacle && _tile != charTile)
                            {
                                if (!highlightedTiles.Contains(_tile))
                                    HighlightTile(_tile, col);
                                if (!newMarkedTiles.Contains(_tile))
                                    newMarkedTiles.Add(_tile);
                            }
                        }
                        else
                        {
                            if (!_tile.IsObstacle)
                            {
                                if (!highlightedTiles.Contains(_tile))
                                    HighlightTile(_tile, col);
                                if (!newMarkedTiles.Contains(_tile))
                                    newMarkedTiles.Add(_tile);
                            }
                        }
                    }
                }
                // +/- y
                for (int y = -1; y <= 1; y++)
                {
                    // excluding diagonals
                    if (!diagonal)
                    {
                        worldPoint = new Vector3Int(charTile.LocalPlace.x, markedTile.LocalPlace.y + y, 0);
                    }
                    else
                    {
                        worldPoint = new Vector3Int(markedTile.LocalPlace.x, markedTile.LocalPlace.y + y, 0);
                    }
                    if (tiles.TryGetValue(worldPoint, out _tile))
                    {
                        // excluding self
                        if (!self)
                        {
                            if (!_tile.IsObstacle && _tile != charTile)
                            {
                                if (!highlightedTiles.Contains(_tile))
                                    HighlightTile(_tile, col);
                                if (!newMarkedTiles.Contains(_tile))
                                    newMarkedTiles.Add(_tile);
                            }
                        }
                        else
                        {
                            if (!_tile.IsObstacle)
                            {
                                if (!highlightedTiles.Contains(_tile))
                                    HighlightTile(_tile, col);
                                if (!newMarkedTiles.Contains(_tile))
                                    newMarkedTiles.Add(_tile);
                            }
                        }
                    }
                }
            }
            markedTiles = newMarkedTiles;
        }
    }

    /* Player movement highlighting */
    public List<WorldTile> HiglightPlayerMovementRange(Vector3 position, int range, Color col)
    {
        // clear the list just in case.
        ClearHighlightedTiles();

        // get tiles withing character range
        // create the bounds object yourself with new BoundsInt(origin, size)
        Vector3Int pos = Vector3Int.FloorToInt(position);
        // https://medium.com/@allencoded/unity-tilemaps-and-storing-individual-tile-data-8b95d87e9f32
        // This is our Dictionary of tiles
        var tiles = GameTiles.instance.tiles;
        // highlight tiles in movement range
        // list with tiles
        var markedTiles = new List<WorldTile>();

        // adding char position
        Vector3Int tilePos = tilemap.WorldToCell(position);
        Vector3Int worldPoint;

        if (tiles.TryGetValue(tilePos, out _tile))
        {
            HighlightTile(_tile, col);
            _tile.WithinRange = true;
            highlightedTiles.Add(_tile);
            markedTiles.Add(_tile);
        }

        // number of steps character can make
        // TODO: this seems not optimal, it lags unity for 1 min when range is 10;
        for (int i = 0; i < range; i++)
        {
            var newMarkedTiles = new List<WorldTile>();

            // for each tile in marked tiles
            foreach (WorldTile tile in markedTiles)
            {
                for (int x = -1; x <= 1; x++)
                {
                    for (int y = -1; y <= 1; y++)
                    {
                        // https://youtu.be/2ycN6ZkWgOo?t=598
                        // (using the XOR operator instead) that way, we'll skip the current tile (:
                        int neighbourX = tile.LocalPlace.x + x;
                        int neighbourY = tile.LocalPlace.y + y;
                        if (x == 0 ^ y == 0)
                        {
                            worldPoint = new Vector3Int(neighbourX, neighbourY, 0);
                            if (tiles.TryGetValue(worldPoint, out _tile))
                            {
                                // can you walk on the tile? 
                                if (PlayerIsTileWalkable(worldPoint))
                                {
                                    newMarkedTiles.Add(_tile);
                                    // can you stop on the tile?
                                    if (!highlightedTiles.Contains(_tile) && PlayerCanIStopOnTheTile(worldPoint))
                                        HighlightTile(_tile, col);
                                }
                            }
                        }
                    }
                }

            }
            markedTiles = newMarkedTiles;
        }

        // remember what tiles you highlight
        return highlightedTiles;
    }

    // this function will return false if the tile is not walkable
    // you can't walk on:
    // - obstacle tiles
    // - tiles that contain an obstacle object
    bool PlayerIsTileWalkable(Vector3Int pos)
    {
        if (tiles.TryGetValue(pos, out _tile))
        {
            // if tile is marked as obstacle it is not walkable
            if (_tile.IsObstacle)
                return false;

            // creating a collider in the middle of the tile
            Vector3 colPos = new Vector3(_tile.LocalPlace.x + 0.5f, _tile.LocalPlace.y + 0.5f, _tile.LocalPlace.z);
            Collider2D col = Physics2D.OverlapCircle(colPos, 0.2f);

            if (col != null)
            {
                // you can't walk on obstacles
                if (col.transform.CompareTag("Obstacle") || col.transform.CompareTag("Stone"))
                    return false;

                // you can't walk on tiles enemies are standing on
                if (col.transform.CompareTag("EnemyCollider"))
                    return false;
            }
        }
        // if there is no tile you cannot walk there
        else
            return false;

        // if we don't return before we can walk on that tile
        return true;
    }


    // this function will return false if you can't stop on the tile
    // you can't stop on:
    // - trap tiles
    // - tiles your allies are standing on

    bool PlayerCanIStopOnTheTile(Vector3Int pos)
    {
        // creating a collider in the middle of the tile
        Vector3 colPos = new Vector3(pos.x + 0.5f, pos.y + 0.5f, pos.z);
        Collider2D col = Physics2D.OverlapCircle(colPos, 0.2f);

        // there is nothing to collide with on the tile
        if (col == null)
            return true;

        // you can't stop on that tile
        if (col.transform.CompareTag("Trap") || col.transform.CompareTag("PlayerCollider") || col.transform.CompareTag("EnemyCollider"))
            return false;

        // if we don't return before we can stop on that tile
        return true;
    }

    /* Enemy movement highlighting */
    public void HiglightEnemyMovementRange(Vector3 position, int range, Color col)
    {
        // clear the list just in case.
        ClearHighlightedTiles();

        // get movement range of the character
        // get tiles withing character range
        // create the bounds object yourself with new BoundsInt(origin, size)
        Vector3Int pos = Vector3Int.FloorToInt(position);

        // list with tiles
        var markedTiles = new List<WorldTile>();

        // adding char position
        Vector3Int tilePos = tilemap.WorldToCell(position);
        Vector3Int worldPoint;

        if (tiles.TryGetValue(tilePos, out _tile))
        {
            HighlightTile(_tile, col);
            markedTiles.Add(_tile);
        }

        // number of steps character can make
        // TODO: this seems not optimal, it lags unity for 1 min when range is 10;
        // TODO: I am checking waaay too many tiles
        for (int i = 0; i < range; i++)
        {
            var newMarkedTiles = new List<WorldTile>();

            // for each tile in marked tiles
            foreach (WorldTile tile in markedTiles)
            {
                for (int x = -1; x <= 1; x++)
                {
                    for (int y = -1; y <= 1; y++)
                    {
                        // https://youtu.be/2ycN6ZkWgOo?t=598
                        // (using the XOR operator instead) that way, we'll skip the current tile (:
                        int neighbourX = tile.LocalPlace.x + x;
                        int neighbourY = tile.LocalPlace.y + y;
                        if (x == 0 ^ y == 0)
                        {
                            worldPoint = new Vector3Int(neighbourX, neighbourY, 0);
                            if (tiles.TryGetValue(worldPoint, out _tile))
                            {
                                // can you calk on the tile? 
                                if (EnemyIsTileWalkable(worldPoint))
                                {
                                    newMarkedTiles.Add(_tile);
                                    // can you stop on the tile?
                                    if (!highlightedTiles.Contains(_tile) && EnemyCanIStopOnTheTile(worldPoint))
                                        HighlightTile(_tile, col);
                                }
                            }
                        }
                    }
                }
            }
            markedTiles = newMarkedTiles;
        }
    }

    // this function will return false if the tile is not walkable
    // you can't walk on:
    // - obstacle tiles
    // - tiles that contain an obstacle object
    public bool EnemyIsTileWalkable(Vector3Int pos)
    {
        if (tiles.TryGetValue(pos, out _tile))
        {
            // if tile is marked as obstacle it is not walkable
            if (_tile.IsObstacle)
                return false;

            // creating a collider in the middle of the tile
            Vector3 colPos = new Vector3(_tile.LocalPlace.x + 0.5f, _tile.LocalPlace.y + 0.5f, _tile.LocalPlace.z);
            Collider2D col = Physics2D.OverlapCircle(colPos, 0.2f);

            if (col != null)
            {
                // you can't walk on obstacles
                if (col.transform.CompareTag("Obstacle") || col.transform.CompareTag("Stone"))
                    return false;

                // you can't walk on tiles enemies are standing on
                if (col.transform.CompareTag("PlayerCollider"))
                    return false;
            }
        }
        // if there is no tile you cannot walk there
        else
            return false;

        // if we don't return before we can walk on that tile
        return true;
    }


    // this function will return false if you can't stop on the tile
    // you can't stop on:
    // - tiles your allies are standing on
    public bool EnemyCanIStopOnTheTile(Vector3Int pos)
    {
        // creating a collider in the middle of the tile
        Vector3 colPos = new Vector3(pos.x + 0.5f, pos.y + 0.5f, pos.z);
        Collider2D col = Physics2D.OverlapCircle(colPos, 0.2f);

        // there is nothing to collide with on the tile
        if (col == null)
            return true;

        // you can't stop on that tile
        if (col.transform.CompareTag("EnemyCollider"))
            return false;

        // if we don't return before we can stop on that tile
        return true;
    }

    public void ClearHighlightedTile(Vector3 position)
    {
        Vector3Int tilePos = tilemap.WorldToCell(position);
        if (tiles.TryGetValue(tilePos, out _tile))
        {
            // remove flag from the tile
            // remove it from the list
            _tile.WithinRange = false;
            highlightedTiles.Remove(_tile);

            // remove flasher from that tile
            foreach (GameObject flasher in flashers)
            {
                if (flasher != null && flasher.transform.position == new Vector3(_tile.LocalPlace.x + flasherXOffset,
                                                                    _tile.LocalPlace.y + flasherYOffset, _tile.LocalPlace.z))
                {
                    Destroy(flasher);

                }
            }

        }
    }

    public void ClearHighlightedTiles()
    {
        // destory flashers
        foreach (GameObject flasher in flashers)
        {
            GameObject.Destroy(flasher);
        }
        // clear the list
        flashers.Clear();

        // clear highlights 
        foreach (WorldTile tile in highlightedTiles)
        {
            tile.ClearHighlightAndTags();
        }

        // clear the list
        highlightedTiles.Clear();
    }

    void HighlightTile(WorldTile tile, Color col)
    {
        GameObject flasher = Instantiate(flasherPrefab, new Vector3(tile.LocalPlace.x + flasherXOffset, tile.LocalPlace.y + flasherYOffset, tile.LocalPlace.z), Quaternion.identity);
        flasher.GetComponent<Flasher>().StartFlashing(col);
        flashers.Add(flasher);
        flasher.transform.SetParent(flasherHolder.transform);
        //tile.Highlight(col);
        //flashCoroutines.Add(flash);

        _tile.WithinRange = true;
        highlightedTiles.Add(_tile);
    }

}




