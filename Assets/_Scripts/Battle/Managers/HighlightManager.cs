using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;

public class HighlightManager : Singleton<HighlightManager>
{

    // global
    BattleInputController _battleInputController;

    // tilemap https://medium.com/@allencoded/unity-tilemaps-and-storing-individual-tile-data-8b95d87e9f32
    Tilemap _tilemap;
    WorldTile _tile;

    WorldTile _charTile;
    public List<WorldTile> HighlightedTiles = new();

    List<WorldTile> _markedTiles = new();

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

    public async Task HighlightSingle(Vector3 position, Color col)
    {
        await ClearHighlightedTiles();

        Vector3Int tilePos = _tilemap.WorldToCell(position);
        if (TileManager.Tiles.TryGetValue(tilePos, out _tile))
            HighlightTile(_tile, col);
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

                // continue looping if the tile does not exist
                if (!TileManager.Tiles.TryGetValue(tilePos, out _tile))
                    continue;

                if (CanCharacterWalkOnTile(_tile, Tags.Enemy) && CanCharacterStopOnTile(_tile))
                    HighlightTile(_tile, col);
            }
        }
    }

    /* Player movement highlighting */
    public async Task HighlightCharacterMovementRange(CharacterStats stats, string opponentTag)
    {
        // clear the list just in case.
        await ClearHighlightedTiles();

        // adding char position
        Vector3Int tilePos = _tilemap.WorldToCell(stats.transform.position);

        AddTileForHighlighting(tilePos, Helpers.GetColor("movementBlue"));

        // TODO: this seems not optimal, it lags unity for 1 min when range is 10;
        for (int i = 0; i < stats.MovementRange.GetValue(); i++)
            await HandleCharacterMovementHighlighting(Helpers.GetColor("movementBlue"), opponentTag);
    }

    async Task HandleCharacterMovementHighlighting(Color col, string opponentTag)
    {
        Vector3Int worldPoint;

        List<WorldTile> markedTilesCopy = new List<WorldTile>(_markedTiles);
        _markedTiles.Clear();

        // for each tile in marked tiles
        foreach (WorldTile tile in markedTilesCopy)
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
                        if (!CanCharacterWalkOnTile(_tile, opponentTag))
                            continue;

                        _markedTiles.Add(_tile);

                        // can you stop on the tile?
                        if (!HighlightedTiles.Contains(_tile) && CanCharacterStopOnTile(_tile))
                            HighlightTile(_tile, col);
                    }
                }
            }
        }
        await Task.Delay(50);
    }

    bool CanCharacterWalkOnTile(WorldTile tile, string opponentTag)
    {
        if (IsTileObstacle(tile))
            return false;

        // creating a collider in the middle of the tile
        Collider2D[] cols = Physics2D.OverlapCircleAll(tile.GetMiddleOfTile(), 0.2f);
        foreach (Collider2D c in cols)
            if (c.transform.CompareTag(opponentTag)) // you can't walk on tiles opponents are standing on
                return false;

        // if we don't return before we can walk on that tile
        return true;
    }

    public bool CanCharacterStopOnTile(WorldTile tile)
    {
        if (IsTileObstacle(tile))
            return false;

        // creating a collider in the middle of the tile
        Collider2D[] cols = Physics2D.OverlapCircleAll(tile.GetMiddleOfTile(), 0.2f);
        foreach (Collider2D c in cols)
            if (c.transform.CompareTag(Tags.Player) || c.transform.CompareTag(Tags.Enemy))
                return false;

        // if we don't return before we can stop on that tile
        return true;
    }

    bool IsTileObstacle(WorldTile tile)
    {
        // if tile is marked as obstacle it is not walkable
        if (tile.IsObstacle)
            return true;

        if (IsThereObstacleGameObject(tile))
            return true;

        return false;
    }

    bool IsThereObstacleGameObject(WorldTile tile)
    {
        // creating a collider in the middle of the tile
        Collider2D[] cols = Physics2D.OverlapCircleAll(tile.GetMiddleOfTile(), 0.2f);
        foreach (Collider2D c in cols)
            if (c.transform.CompareTag(Tags.Obstacle) || c.transform.CompareTag(Tags.PushableObstacle))
                return true;

        return false;
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
        _markedTiles.Clear();

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

    /* Ability */

    public async Task HighlightAbilityRange(Ability ability)
    {
        await ClearHighlightedTiles();

        Vector3 characterPos = ability.CharacterGameObject.transform.position;
        if (ability.Range == 0)
        {
            await HighlightSingle(characterPos, ability.HighlightColor);
            return;
        }

        // adding char position
        Vector3Int tilePos = _tilemap.WorldToCell(characterPos);

        if (TileManager.Tiles.TryGetValue(tilePos, out _tile))
            _charTile = _tile;
        _markedTiles.Add(_charTile);

        for (int i = 0; i < ability.Range; i++)
            await HandleAbilityHighlighting(ability, false);
    }

    public async Task HighlightAbilityLineAOE(Ability ability, Vector3 targetPos)
    {
        await ClearHighlightedTiles();

        // TOOD: incorrect

        Vector3 characterPos = ability.CharacterGameObject.transform.position;
        Vector3 dir = (targetPos - characterPos).normalized;
        Debug.Log($"dir: {dir}");

        float distance = Vector3.Distance(characterPos, targetPos);
        Debug.Log($"dir: {distance}");

        for (int i = 1; i <= distance; i++)
        {
            Vector3 pos = characterPos + dir * i;
            // add selected point to highlight
            Vector3Int tilePos = _tilemap.WorldToCell(pos);

            //Vector3 posAdjusted = new Vector3(pos.x - 0.5f, pos.y - 0.5f);
            Debug.Log($"dir: {tilePos}");
            Debug.Log($"CanAbilityTargetTile(ability, posAdjusted): {CanAbilityTargetTile(ability, tilePos)}");

            if (CanAbilityTargetTile(ability, tilePos))
                AddTileForHighlighting(tilePos, ability.HighlightColor);
        }
    }

    public async Task HighlightAbilityAOE(Ability ability, Vector3 middlePos)
    {
        await ClearHighlightedTiles();

        Vector3 characterPos = ability.CharacterGameObject.transform.position;
        if (ability.AreaOfEffect == 0)
        {
            await HighlightSingle(middlePos, ability.HighlightColor);
            return;
        }

        // add selected point to highlight
        Vector3Int tilePos = _tilemap.WorldToCell(middlePos);
        AddTileForHighlighting(tilePos, ability.HighlightColor);
        if (TileManager.Tiles.TryGetValue(tilePos, out _tile))
            _charTile = _tile;

        for (int i = 0; i < ability.AreaOfEffect; i++)
            await HandleAbilityHighlighting(ability, true);
    }


    async Task HandleAbilityHighlighting(Ability ability, bool highlightingAOE)
    {
        bool diagonal = ability.CanTargetDiagonally;
        Color color = ability.HighlightColor;

        if (highlightingAOE)
            diagonal = true;

        // for each tile in marked tiles
        List<WorldTile> markedTilesCopy = new List<WorldTile>(_markedTiles);
        _markedTiles.Clear();

        Vector3Int worldPointX;
        Vector3Int worldPointY;

        foreach (WorldTile markedTile in markedTilesCopy)
        {
            for (int i = -1; i <= 1; i++)
            {
                if (!diagonal)
                {
                    worldPointX = new Vector3Int(markedTile.LocalPlace.x + i, _charTile.LocalPlace.y, 0);
                    worldPointY = new Vector3Int(_charTile.LocalPlace.x, markedTile.LocalPlace.y + i, 0);
                }
                else
                {
                    worldPointX = new Vector3Int(markedTile.LocalPlace.x + i, markedTile.LocalPlace.y, 0);
                    worldPointY = new Vector3Int(markedTile.LocalPlace.x, markedTile.LocalPlace.y + i, 0);
                }

                if (highlightingAOE || CanAbilityTargetTile(ability, worldPointX))
                    AddTileForHighlighting(worldPointX, color);
                if (highlightingAOE || CanAbilityTargetTile(ability, worldPointY))
                    AddTileForHighlighting(worldPointY, color);
            }
        }
        // delay to make it appear in a cool way (sequentially)
        await Task.Delay(20);
    }

    bool CanAbilityTargetTile(Ability ability, Vector3 worldPoint)
    {
        bool self = ability.CanTargetSelf;

        // excluding not tiles
        if (!TileManager.Tiles.TryGetValue(worldPoint, out _tile))
            return false;

        // excluding self
        if (!self && _charTile.LocalPlace == worldPoint)
            return false;

        return true;
    }

    void AddTileForHighlighting(Vector3 pos, Color color)
    {
        if (!TileManager.Tiles.TryGetValue(pos, out _tile))
            return;

        if (!HighlightedTiles.Contains(_tile))
            HighlightTile(_tile, color);
        if (!_markedTiles.Contains(_tile))
            _markedTiles.Add(_tile);
    }

}