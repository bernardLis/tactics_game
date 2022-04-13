using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Highlighter : Singleton<Highlighter>
{
    // TODO: this script can be better

    // global
    BattleInputController _battleInputController;

    // tilemap https://medium.com/@allencoded/unity-tilemaps-and-storing-individual-tile-data-8b95d87e9f32
    Tilemap _tilemap;
    WorldTile _tile;

    WorldTile _charTile;
    public List<WorldTile> HighlightedTiles = new();

    // TODO: when I rewrite enemies I can change it to marked tiles.
    List<WorldTile> _previousMarkedTiles = new();

    // flashers
    List<GameObject> _flashers = new();
    [Header("Flasher")]
    [SerializeField] GameObject _flasherHolder;
    [SerializeField] GameObject _flasherPrefab;
    float _flasherXOffset = 0.4f;
    float _flasherYOffset = 0.6f;

    protected override void Awake()
    {
        base.Awake();
        _tilemap = BattleManager.Instance.GetComponent<TileManager>().Tilemap;
    }

    public void Start()
    {
        _battleInputController = BattleInputController.Instance;
    }

    public WorldTile HighlightSingle(Vector3 position, Color col)
    {
        // TODO: should I make it all async?
        // making sure there is only one highlight at the time
        ClearHighlightedTiles().GetAwaiter();

        Vector3Int tilePos = _tilemap.WorldToCell(position);
        if (TileManager.Tiles.TryGetValue(tilePos, out _tile))
        {
            HighlightTile(_tile, col);
            return _tile;
        }
        return null;
    }

    // TODO: this
    public void HighlightRectanglePlayer(Vector2 SWcorner, int width, int height, Color col)
    {
        ClearHighlightedTiles().GetAwaiter();

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                Vector3 position = new Vector3(SWcorner.x + i, SWcorner.y + j, 0f);
                Vector3Int tilePos = _tilemap.WorldToCell(position);
                // if(tiles == null)
                //      tiles = TileManager.instance.tiles;
                // continue looping if the tile does not exist
                if (!TileManager.Tiles.TryGetValue(tilePos, out _tile))
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
        _battleInputController.SetInputAllowed(false);

        // making sure there is only one highlight at the time
        await ClearHighlightedTiles();

        // get the tile character is currently standing on
        Vector3Int tilePos = _tilemap.WorldToCell(position);

        if (TileManager.Tiles.TryGetValue(tilePos, out _tile))
        {
            _previousMarkedTiles.Add(_tile);
            _charTile = _tile;
        }

        for (int i = 0; i < range; i++)
            await HandleTileHighlighting(col, diagonal, self);

        // TODO: does this suck
        _battleInputController.SetInputAllowed(true);
    }

    async Task HandleTileHighlighting(Color col, bool diagonal, bool self)
    {
        // for each tile in marked tiles
        var newMarkedTiles = new List<WorldTile>();
        Vector3Int worldPoint;

        foreach (WorldTile markedTile in _previousMarkedTiles)
        {
            // +/-  x
            for (int x = -1; x <= 1; x++)
            {
                // excluding diagonals
                if (!diagonal)
                    worldPoint = new Vector3Int(markedTile.LocalPlace.x + x, _charTile.LocalPlace.y, 0);
                else
                    worldPoint = new Vector3Int(markedTile.LocalPlace.x + x, markedTile.LocalPlace.y, 0);

                // excluding not tiles
                if (!TileManager.Tiles.TryGetValue(worldPoint, out _tile))
                    continue;

                // excluding self
                if (!self && _tile == _charTile)
                    continue;

                // exclude obstacle tiles
                if (_tile.IsObstacle)
                    continue;

                if (!HighlightedTiles.Contains(_tile))
                    HighlightTile(_tile, col);
                if (!newMarkedTiles.Contains(_tile))
                    newMarkedTiles.Add(_tile);
            }
            // +/- y
            for (int y = -1; y <= 1; y++)
            {
                // excluding diagonals
                if (!diagonal)
                    worldPoint = new Vector3Int(_charTile.LocalPlace.x, markedTile.LocalPlace.y + y, 0);
                else
                    worldPoint = new Vector3Int(markedTile.LocalPlace.x, markedTile.LocalPlace.y + y, 0);

                // excluding not tiles
                if (!TileManager.Tiles.TryGetValue(worldPoint, out _tile))
                    continue;

                // excluding self
                if (!self && _tile == _charTile)
                    continue;

                // exclude obstacle tiles
                if (_tile.IsObstacle)
                    continue;

                if (!HighlightedTiles.Contains(_tile))
                    HighlightTile(_tile, col);
                if (!newMarkedTiles.Contains(_tile))
                    newMarkedTiles.Add(_tile);
            }
        }
        // delay to make it appear in a cool way (sequentially)
        _previousMarkedTiles = newMarkedTiles;
        await Task.Delay(50);
    }

    /* Player movement highlighting */
    public async Task HiglightPlayerMovementRange(Vector3 position, int range, Color col)
    {
        // TODO: does this suck
        _battleInputController.SetInputAllowed(false);

        // clear the list just in case.
        await ClearHighlightedTiles();

        // adding char position
        Vector3Int tilePos = _tilemap.WorldToCell(position);

        if (TileManager.Tiles.TryGetValue(tilePos, out _tile))
        {
            _previousMarkedTiles.Add(_tile);
            HighlightedTiles.Add(_tile);

        }

        // TODO: this seems not optimal, it lags unity for 1 min when range is 10;
        for (int i = 0; i < range; i++)
            await HandlePlayerMovementHighlighting(col);

        // TODO: does this suck
        _battleInputController.SetInputAllowed(true);
    }

    async Task HandlePlayerMovementHighlighting(Color col)
    {
        Vector3Int worldPoint;
        var newMarkedTiles = new List<WorldTile>();

        // for each tile in marked tiles
        foreach (WorldTile tile in _previousMarkedTiles)
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

                        if (!TileManager.Tiles.TryGetValue(worldPoint, out _tile))
                            continue;

                        // can you walk on the tile? 
                        if (!CanPlayerWalkOnTile(_tile))
                            continue;

                        newMarkedTiles.Add(_tile);

                        // can you stop on the tile?
                        if (!HighlightedTiles.Contains(_tile) && CanPlayerStopOnTile(_tile))
                            HighlightTile(_tile, col);
                    }
                }
            }
        }
        _previousMarkedTiles = newMarkedTiles;
        await Task.Delay(50);
    }



    // this function will return false if the tile is not walkable
    // you can't walk on:
    // - obstacle tiles
    // - tiles that contain an obstacle object
    bool CanPlayerWalkOnTile(WorldTile tile)
    {
        // if tile is marked as obstacle it is not walkable
        if (tile.IsObstacle)
            return false;

        // creating a collider in the middle of the tile
        Collider2D col = Physics2D.OverlapCircle(tile.GetMiddleOfTile(), 0.2f);

        if (col == null)
            return true;

        // you can't walk on obstacles
        if (col.transform.CompareTag(Tags.Obstacle) || col.transform.CompareTag(Tags.PushableObstacle))
            return false;

        // you can't walk on tiles enemies are standing on
        if (col.transform.CompareTag(Tags.EnemyCollider))
            return false;

        // if we don't return before we can walk on that tile
        return true;
    }


    // this function will return false if you can't stop on the tile
    // you can't stop on:
    // - trap tiles
    // - tiles your allies are standing on
    bool CanPlayerStopOnTile(WorldTile tile)
    {
        // creating a collider in the middle of the tile
        Collider2D col = Physics2D.OverlapCircle(tile.GetMiddleOfTile(), 0.2f);

        // there is nothing to collide with on the tile
        if (col == null)
            return true;

        // you can't stop on that tile
        if (col.transform.CompareTag(Tags.Trap) || col.transform.CompareTag(Tags.PlayerCollider) || col.transform.CompareTag(Tags.EnemyCollider))
            return false;

        // if we don't return before we can stop on that tile
        return true;
    }

    /* Enemy movement highlighting */
    public async Task HiglightEnemyMovementRange(Vector3 position, int range, Color col)
    {
        // TODO: should I make it all async?
        // clear the list just in case.
        await ClearHighlightedTiles();

        // list with tiles
        var markedTiles = new List<WorldTile>();

        // adding char position
        Vector3Int tilePos = _tilemap.WorldToCell(position);
        Vector3Int worldPoint;

        if (TileManager.Tiles.TryGetValue(tilePos, out _tile))
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
                            if (!TileManager.Tiles.TryGetValue(worldPoint, out _tile))
                                continue;

                            // can you calk on the tile? 
                            if (!CanEnemyWalkOnTile(_tile))
                                continue;

                            newMarkedTiles.Add(_tile);

                            // can you stop on the tile?
                            if (!HighlightedTiles.Contains(_tile) && CanEnemyStopOnTile(_tile))
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
    public bool CanEnemyWalkOnTile(WorldTile tile)
    {
        // if tile is marked as obstacle it is not walkable
        if (tile.IsObstacle)
            return false;

        // creating a collider in the middle of the tile
        Collider2D col = Physics2D.OverlapCircle(tile.GetMiddleOfTile(), 0.2f);

        if (col == null)
            return true;

        // you can't walk on obstacles TODO: better check layer
        if (col.transform.CompareTag(Tags.Obstacle) || col.transform.CompareTag(Tags.PushableObstacle))
            return false;

        // you can't walk on tiles enemies are standing on
        if (col.transform.CompareTag(Tags.PlayerCollider))
            return false;

        // if we don't return before we can walk on that tile
        return true;
    }


    // this function will return false if you can't stop on the tile
    // you can't stop on:
    // - tiles your allies are standing on
    public bool CanEnemyStopOnTile(WorldTile tile)
    {
        // creating a collider in the middle of the tile
        Collider2D col = Physics2D.OverlapCircle(tile.GetMiddleOfTile(), 0.2f);

        // there is nothing to collide with on the tile
        if (col == null)
            return true;

        // you can't stop on that tile
        if (col.transform.CompareTag(Tags.EnemyCollider))
            return false;

        // if we don't return before we can stop on that tile
        return true;
    }

    public void ClearHighlightedTile(Vector3 position)
    {
        Vector3Int tilePos = _tilemap.WorldToCell(position);
        if (!TileManager.Tiles.TryGetValue(tilePos, out _tile))
            return;

        // remove flasher from that tile
        foreach (GameObject flasher in _flashers)
        {
            if (flasher == null)
                continue;

            if (flasher.transform.position == new Vector3(_tile.LocalPlace.x + _flasherXOffset, _tile.LocalPlace.y + _flasherYOffset, _tile.LocalPlace.z))
                flasher.GetComponent<Flasher>().StopFlashing();
        }

        // remove flag from the tile
        // remove it from the list
        _tile.WithinRange = false;
        HighlightedTiles.Remove(_tile);
    }

    public async Task ClearHighlightedTiles()
    {
        // destory flashers
        foreach (GameObject flasher in _flashers)
        {
            if (flasher == null)
                continue;

            flasher.GetComponent<Flasher>().StopFlashing();
        }

        // clear highlights 
        foreach (WorldTile tile in HighlightedTiles)
        {
            tile.ClearHighlightAndTags();
        }

        // clear the lists
        _flashers.Clear();
        HighlightedTiles.Clear();
        _previousMarkedTiles.Clear();

        await Task.Yield();
    }

    void HighlightTile(WorldTile tile, Color col)
    {
        GameObject flasher = Instantiate(_flasherPrefab, new Vector3(tile.LocalPlace.x + _flasherXOffset, tile.LocalPlace.y + _flasherYOffset, tile.LocalPlace.z), Quaternion.identity);
        flasher.GetComponent<Flasher>().StartFlashing(col);
        _flashers.Add(flasher);
        flasher.transform.SetParent(_flasherHolder.transform);

        _tile.WithinRange = true;
        HighlightedTiles.Add(_tile);
    }
}