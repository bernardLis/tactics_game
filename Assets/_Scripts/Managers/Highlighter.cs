using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;

public class Highlighter : MonoBehaviour
{
    // TODO: this script can be better

    // global
    BattleInputController battleInputController;

    // https://medium.com/@allencoded/unity-tilemaps-and-storing-individual-tile-data-8b95d87e9f32
    // tilemap
    Tilemap tilemap;
    WorldTile _tile;

    WorldTile charTile;
    public List<WorldTile> highlightedTiles = new();

    // TODO: when I rewrite enemies I can change it to marked tiles.
    List<WorldTile> previousMarkedTiles = new();

    // flashers
    List<GameObject> flashers = new();
    [Header("Flasher")]
    [SerializeField] GameObject flasherHolder;
    [SerializeField] GameObject flasherPrefab;
    public float flasherXOffset = 0.4f;
    public float flasherYOffset = 0.6f;

    public static Highlighter instance;
    void Awake()
    {
        #region Singleton
        // singleton
        if (instance != null)
        {
            Debug.LogWarning("More than one instance of Highlighter found");
            return;
        }
        instance = this;
        #endregion

        tilemap = TileMapInstance.instance.GetComponent<Tilemap>();
    }

    public void Start()
    {
        battleInputController = BattleInputController.instance;
    }

    public WorldTile HighlightSingle(Vector3 position, Color col)
    {
        // TODO: should I make it all async?
        // making sure there is only one highlight at the time
        ClearHighlightedTiles().GetAwaiter();

        Vector3Int tilePos = tilemap.WorldToCell(position);
        if (GameTiles.tiles.TryGetValue(tilePos, out _tile))
        {
            HighlightTile(_tile, col);
            return _tile;
        }
        return null;
    }

    // TODO: this
    public void HighlightRectanglePlayer(Vector2 SWcorner, int length, int height, Color col)
    {
        for (int i = 0; i < length; i++)
        {
            for (int j = 0; j < height; j++)
            {
                Vector3 position = new Vector3(SWcorner.x + i, SWcorner.y + j, 0f);
                Vector3Int tilePos = tilemap.WorldToCell(position);
               // if(tiles == null)
              //      tiles = GameTiles.instance.tiles;
                // continue looping if the tile does not exist
                if (!GameTiles.tiles.TryGetValue(tilePos, out _tile))
                    continue;

                if (CanPlayerWalkOnTile(_tile) && CanPlayerStopOnTile(_tile))
                    HighlightTile(_tile, col);
            }
        }
    }

    // TODO: this is a mess...
    public async Task HighlightTiles(Vector3 position, int range, Color col, bool diagonal, bool self)
    {
        // TODO: does this suck
        battleInputController.SetInputAllowed(false);

        // making sure there is only one highlight at the time
        await ClearHighlightedTiles();

        // get the tile character is currently standing on
        Vector3Int tilePos = tilemap.WorldToCell(position);

        if (GameTiles.tiles.TryGetValue(tilePos, out _tile))
        {
            previousMarkedTiles.Add(_tile);
            charTile = _tile;
        }

        for (int i = 0; i < range; i++)
            await HandleTileHighlighting(col, diagonal, self);

        // TODO: does this suck
        battleInputController.SetInputAllowed(true);
    }

    async Task HandleTileHighlighting(Color col, bool diagonal, bool self)
    {
        // for each tile in marked tiles
        var newMarkedTiles = new List<WorldTile>();
        Vector3Int worldPoint;

        foreach (WorldTile markedTile in previousMarkedTiles)
        {
            // +/-  x
            for (int x = -1; x <= 1; x++)
            {
                // excluding diagonals
                if (!diagonal)
                    worldPoint = new Vector3Int(markedTile.LocalPlace.x + x, charTile.LocalPlace.y, 0);
                else
                    worldPoint = new Vector3Int(markedTile.LocalPlace.x + x, markedTile.LocalPlace.y, 0);

                // excluding not tiles
                if (!GameTiles.tiles.TryGetValue(worldPoint, out _tile))
                    continue;

                // excluding self
                if (!self && _tile == charTile)
                    continue;

                // exclude obstacle tiles
                if (_tile.IsObstacle)
                    continue;

                if (!highlightedTiles.Contains(_tile))
                    HighlightTile(_tile, col);
                if (!newMarkedTiles.Contains(_tile))
                    newMarkedTiles.Add(_tile);
            }
            // +/- y
            for (int y = -1; y <= 1; y++)
            {
                // excluding diagonals
                if (!diagonal)
                    worldPoint = new Vector3Int(charTile.LocalPlace.x, markedTile.LocalPlace.y + y, 0);
                else
                    worldPoint = new Vector3Int(markedTile.LocalPlace.x, markedTile.LocalPlace.y + y, 0);

                // excluding not tiles
                if (!GameTiles.tiles.TryGetValue(worldPoint, out _tile))
                    continue;

                // excluding self
                if (!self && _tile == charTile)
                    continue;

                // exclude obstacle tiles
                if (_tile.IsObstacle)
                    continue;

                if (!highlightedTiles.Contains(_tile))
                    HighlightTile(_tile, col);
                if (!newMarkedTiles.Contains(_tile))
                    newMarkedTiles.Add(_tile);
            }
        }
        // delay to make it appear in a cool way (sequentially)
        previousMarkedTiles = newMarkedTiles;
        await Task.Delay(50);
    }

    /* Player movement highlighting */
    public async Task HiglightPlayerMovementRange(Vector3 position, int range, Color col)
    {
        // TODO: does this suck
        battleInputController.SetInputAllowed(false);

        // clear the list just in case.
        await ClearHighlightedTiles();

        // adding char position
        Vector3Int tilePos = tilemap.WorldToCell(position);

        if (GameTiles.tiles.TryGetValue(tilePos, out _tile))
        {
            previousMarkedTiles.Add(_tile);
            highlightedTiles.Add(_tile);

        }

        // TODO: this seems not optimal, it lags unity for 1 min when range is 10;
        for (int i = 0; i < range; i++)
            await HandlePlayerMovementHighlighting(col);

        // TODO: does this suck
        battleInputController.SetInputAllowed(true);
    }

    async Task HandlePlayerMovementHighlighting(Color col)
    {
        Vector3Int worldPoint;
        var newMarkedTiles = new List<WorldTile>();

        // for each tile in marked tiles
        foreach (WorldTile tile in previousMarkedTiles)
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

                        if (!GameTiles.tiles.TryGetValue(worldPoint, out _tile))
                            continue;

                        // can you walk on the tile? 
                        if (!CanPlayerWalkOnTile(_tile))
                            continue;

                        newMarkedTiles.Add(_tile);

                        // can you stop on the tile?
                        if (!highlightedTiles.Contains(_tile) && CanPlayerStopOnTile(_tile))
                            HighlightTile(_tile, col);
                    }
                }
            }
        }
        previousMarkedTiles = newMarkedTiles;
        await Task.Delay(50);
    }



    // this function will return false if the tile is not walkable
    // you can't walk on:
    // - obstacle tiles
    // - tiles that contain an obstacle object
    bool CanPlayerWalkOnTile(WorldTile _tile)
    {
        // if tile is marked as obstacle it is not walkable
        if (_tile.IsObstacle)
            return false;

        // creating a collider in the middle of the tile
        Collider2D col = Physics2D.OverlapCircle(_tile.GetMiddleOfTile(), 0.2f);

        if (col == null)
            return true;

        // you can't walk on obstacles
        if (col.transform.CompareTag("Obstacle") || col.transform.CompareTag("Stone"))
            return false;

        // you can't walk on tiles enemies are standing on
        if (col.transform.CompareTag("EnemyCollider"))
            return false;

        // if we don't return before we can walk on that tile
        return true;
    }


    // this function will return false if you can't stop on the tile
    // you can't stop on:
    // - trap tiles
    // - tiles your allies are standing on
    bool CanPlayerStopOnTile(WorldTile _tile)
    {
        // creating a collider in the middle of the tile
        Collider2D col = Physics2D.OverlapCircle(_tile.GetMiddleOfTile(), 0.2f);

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
        // TODO: should I make it all async?
        // clear the list just in case.
        ClearHighlightedTiles().GetAwaiter();

        // list with tiles
        var markedTiles = new List<WorldTile>();

        // adding char position
        Vector3Int tilePos = tilemap.WorldToCell(position);
        Vector3Int worldPoint;

        if (GameTiles.tiles.TryGetValue(tilePos, out _tile))
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
                            if (!GameTiles.tiles.TryGetValue(worldPoint, out _tile))
                                continue;

                            // can you calk on the tile? 
                            if (!CanEnemyWalkOnTile(_tile))
                                continue;

                            newMarkedTiles.Add(_tile);

                            // can you stop on the tile?
                            if (!highlightedTiles.Contains(_tile) && CanEnemyStopOnTile(_tile))
                                HighlightTile(_tile, col);
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
    public bool CanEnemyWalkOnTile(WorldTile _tile)
    {
        // if tile is marked as obstacle it is not walkable
        if (_tile.IsObstacle)
            return false;

        // creating a collider in the middle of the tile
        Collider2D col = Physics2D.OverlapCircle(_tile.GetMiddleOfTile(), 0.2f);

        if (col == null)
            return true;

        // you can't walk on obstacles
        if (col.transform.CompareTag("Obstacle") || col.transform.CompareTag("Stone"))
            return false;

        // you can't walk on tiles enemies are standing on
        if (col.transform.CompareTag("PlayerCollider"))
            return false;

        // if we don't return before we can walk on that tile
        return true;
    }


    // this function will return false if you can't stop on the tile
    // you can't stop on:
    // - tiles your allies are standing on
    public bool CanEnemyStopOnTile(WorldTile _tile)
    {
        // creating a collider in the middle of the tile
        Collider2D col = Physics2D.OverlapCircle(_tile.GetMiddleOfTile(), 0.2f);

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
        if (!GameTiles.tiles.TryGetValue(tilePos, out _tile))
            return;

        // remove flasher from that tile
        foreach (GameObject flasher in flashers)
        {
            if (flasher == null)
                continue;

            if (flasher.transform.position == new Vector3(_tile.LocalPlace.x + flasherXOffset, _tile.LocalPlace.y + flasherYOffset, _tile.LocalPlace.z))
                flasher.GetComponent<Flasher>().StopFlashing();
        }

        // remove flag from the tile
        // remove it from the list
        _tile.WithinRange = false;
        highlightedTiles.Remove(_tile);
    }

    public async Task ClearHighlightedTiles()
    {
        // destory flashers
        foreach (GameObject flasher in flashers)
        {
            if (flasher == null)
                continue;

            flasher.GetComponent<Flasher>().StopFlashing();
        }

        // clear highlights 
        foreach (WorldTile tile in highlightedTiles)
        {
            tile.ClearHighlightAndTags();
        }

        // clear the lists
        flashers.Clear();
        highlightedTiles.Clear();
        previousMarkedTiles.Clear();

        await Task.Yield();
    }

    void HighlightTile(WorldTile tile, Color col)
    {
        GameObject flasher = Instantiate(flasherPrefab, new Vector3(tile.LocalPlace.x + flasherXOffset, tile.LocalPlace.y + flasherYOffset, tile.LocalPlace.z), Quaternion.identity);
        flasher.GetComponent<Flasher>().StartFlashing(col);
        flashers.Add(flasher);
        flasher.transform.SetParent(flasherHolder.transform);

        _tile.WithinRange = true;
        highlightedTiles.Add(_tile);
    }
}




