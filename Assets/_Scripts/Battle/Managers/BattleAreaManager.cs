using System;
using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;
public class BattleAreaManager : MonoBehaviour
{
    BattleManager _battleManager;

    [SerializeField] Transform _floorHolder;
    [SerializeField] GameObject _floorPrefab;

    [SerializeField] GameObject _tilePrefab;
    [SerializeField] Building _homeBuilding;
    List<Building> _unlockedBuildings = new();

    [SerializeField] List<GameObject> _cornerTileIndicators = new();

    GameObject _floor;

    [HideInInspector] public BattleTile HomeTile;
    [HideInInspector] public List<BattleTile> _cornerTiles = new();
    List<BattleTile> _tiles = new();
    public List<BattleTile> UnlockedTiles = new();

    public event Action<BattleTile> OnTileUnlocked;
    public event Action<BattleTile> OnBossTileUnlocked;

    public void Initialize()
    {
        _battleManager = BattleManager.Instance;

        float tileScale = _floorPrefab.transform.localScale.x;
        _floor = Instantiate(_floorPrefab,
                new Vector3(-tileScale * 0.5f, 0, -tileScale * 0.5f), // floor offset to make tiles centered
                Quaternion.identity);
        _floor.transform.SetParent(_floorHolder);

        _unlockedBuildings = GameManager.Instance.GameDatabase.GetUnlockedBuildings();

        CreateArea();
        StartCoroutine(UnlockTilesPeriodicallyCoroutine());
    }

    void CreateArea()
    {
        // TODO: create area magic numbers
        // current set-up only works when
        // I get 100 tiles with surface scaled to floor scale
        float tileScale = _floorPrefab.transform.localScale.x;
        for (int x = -5; x < 5; x++)
        {
            for (int z = -5; z < 5; z++)
            {
                Vector3 pos = new(x * tileScale, 0, z * tileScale);
                Building building = _unlockedBuildings[Random.Range(0, _unlockedBuildings.Count)];
                if (pos == Vector3.zero)
                    building = _homeBuilding;

                BattleTile bt = InstantiateTile(pos, building);
                _tiles.Add(bt);

                if (pos == Vector3.zero)
                    HomeTile = bt;

                // put corner tiles into the list
                if (pos.x == -5 * tileScale && pos.z == -5 * tileScale ||
                    pos.x == -5 * tileScale && pos.z == 4 * tileScale ||
                    pos.x == 4 * tileScale && pos.z == -5 * tileScale ||
                    pos.x == 4 * tileScale && pos.z == 4 * tileScale)
                {
                    _cornerTiles.Add(bt);
                }
            }
        }
        UnlockedTiles.Add(HomeTile);
        HomeTile.EnableTile();
        HomeTile.Secured();
        HomeTile.HandleBorders();

        SetCornerTileIndicators();
    }

    void SetCornerTileIndicators()
    {
        for (int i = 0; i < 4; i++)
            Instantiate(_cornerTileIndicators[i], _cornerTiles[i].transform.position, Quaternion.Euler(270f, 0, 0));
    }

    IEnumerator UnlockTilesPeriodicallyCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(60f);
            if (_battleManager.IsGameLoopBlocked) continue;
            UnlockNextTile();
        }
    }

    public void DebugSpawnBossTile()
    {
        BattleTile selectedTile = SelectTile();
        OnBossTileUnlocked?.Invoke(selectedTile);
    }

    public void UnlockNextTile(BattleTile givenTile = null)
    {
        BattleTile selectedTile = givenTile ?? SelectTile();

        // TODO: balance when boss spawns
        if (UnlockedTiles.Count > 300)
        {
            OnBossTileUnlocked?.Invoke(selectedTile);
            return;
        }

        if (UnlockedTiles.Contains(selectedTile))
        {
            Debug.LogError($"Trying to unlock tile that is already unlocked");
            return;
        }
        selectedTile.EnableTile();
        UnlockedTiles.Add(selectedTile);
        OnTileUnlocked?.Invoke(selectedTile);
    }

    BattleTile SelectTile()
    {
        List<BattleTile> allPossibleTiles = new();
        foreach (BattleTile tile in UnlockedTiles)
        {
            foreach (BattleTile t in GetAdjacentTiles(tile))
            {
                if (allPossibleTiles.Contains(t)) continue;
                if (UnlockedTiles.Contains(t)) continue;
                allPossibleTiles.Add(t);
            }
        }

        return ChooseTileClosestToHero(allPossibleTiles);
    }

    BattleTile ChooseTileClosestToHero(List<BattleTile> tiles)
    {
        float minDistance = float.MaxValue;
        BattleTile closestTile = null;
        foreach (BattleTile tile in tiles)
        {
            float distance = Vector3.Distance(tile.transform.position, _battleManager.BattleHero.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                closestTile = tile;
            }
        }
        return closestTile;
    }

    public List<BattleTile> GetTilesAroundPlayer()
    {

        BattleTile currentTile = GetTileFromPosition(_battleManager.BattleHero.transform.position);
        List<BattleTile> tilesCloseToHero = GetAdjacentTiles(currentTile);
        tilesCloseToHero.Add(currentTile);
        List<BattleTile> activeTiles = new();
        foreach (BattleTile t in tilesCloseToHero)
            if (t.gameObject.activeSelf) activeTiles.Add(t);

        return activeTiles;
    }

    public BattleTile GetRandomUnlockedTile()
    {
        return UnlockedTiles[Random.Range(0, UnlockedTiles.Count)];
    }

    // TODO: there must be a smarter way to get adjacent tiles
    public List<BattleTile> GetAdjacentTiles(BattleTile tile)
    {
        List<BattleTile> adjacentTiles = new List<BattleTile>();
        Vector3 tilePos = tile.transform.position;
        foreach (BattleTile t in _tiles)
        {
            if (t.transform.position == tilePos) continue;

            if (t.transform.position.x == tilePos.x)
            {
                if (t.transform.position.z == tilePos.z + HomeTile.Scale ||
                    t.transform.position.z == tilePos.z - HomeTile.Scale)
                {
                    adjacentTiles.Add(t);
                }
            }
            else if (t.transform.position.z == tilePos.z)
            {
                if (t.transform.position.x == tilePos.x + HomeTile.Scale ||
                    t.transform.position.x == tilePos.x - HomeTile.Scale)
                {
                    adjacentTiles.Add(t);
                }
            }
        }
        return adjacentTiles;
    }

    public BattleTile ReplaceTile(BattleTile tile, Building newBuilding)
    {
        BattleTile newBattleTile = InstantiateTile(tile.transform.position, newBuilding);

        _tiles.Insert(_tiles.IndexOf(tile), newBattleTile);
        _tiles.Remove(tile);
        Destroy(tile.gameObject);

        return newBattleTile;
    }

    BattleTile InstantiateTile(Vector3 pos, Building b)
    {
        GameObject tile = Instantiate(_tilePrefab, _floorHolder);
        tile.transform.position = pos;
        tile.SetActive(false);

        Building buildingInstance = Instantiate(b);
        BattleTile bt = tile.GetComponent<BattleTile>();
        bt.Initialize(buildingInstance);

        return bt;
    }

    public BattleTile GetTileFromPosition(Vector3 pos)
    {
        foreach (BattleTile tile in _tiles)
        {
            // tile pos is a middle of tile
            // so we need to check if pos is inside tile
            if (pos.x > tile.transform.position.x - tile.Scale * 0.5f &&
                pos.x < tile.transform.position.x + tile.Scale * 0.5f &&
                pos.z > tile.transform.position.z - tile.Scale * 0.5f &&
                pos.z < tile.transform.position.z + tile.Scale * 0.5f)
            {
                return tile;
            }
        }
        return null;
    }

    /* ASTAR */
    struct AStarTile
    {
        public AStarTile(BattleTile tile, float g, float h,
                         BattleTile parent = null, List<BattleTile> children = null)
        {
            Tile = tile;

            G = g;
            H = h;
            F = G + H;

            Children = children;
            Parent = parent;
        }

        public BattleTile Tile;

        public float F; //F = G + H
        public float G; // G is the distance between the current node and the start node.
        public float H; //H is the heuristic — estimated distance from the current node to the end node.

        public BattleTile Parent;
        public List<BattleTile> Children;
    }

    //https://medium.com/@nicholas.w.swift/easy-a-star-pathfinding-7e6689c7f7b2
    public List<BattleTile> GetTilePathFromTo(BattleTile startTile, BattleTile endTile)
    {
        List<AStarTile> openList = new();
        List<AStarTile> closedList = new();
        openList.Add(new AStarTile(startTile, 0, 0));

        while (openList.Count > 0)
        {
            AStarTile currentAStarTile = openList[0];
            foreach (AStarTile tile in openList)
            {
                if (tile.F < currentAStarTile.F)
                    currentAStarTile = tile;
            }

            openList.Remove(currentAStarTile);
            closedList.Add(currentAStarTile);

            if (currentAStarTile.Tile == endTile)
            {
                List<BattleTile> path = new();
                path.Add(currentAStarTile.Tile);
                while (currentAStarTile.Tile != startTile)
                {
                    foreach (AStarTile tile in closedList)
                    {
                        if (tile.Tile == currentAStarTile.Parent)
                        {
                            path.Add(tile.Tile);
                            currentAStarTile = tile;
                            break;
                        }
                    }
                }
                path.Reverse();
                return path;
            }

            List<BattleTile> adjacentTiles = GetAdjacentTiles(currentAStarTile.Tile);
            foreach (BattleTile tile in adjacentTiles)
            {
                if (!tile.gameObject.activeSelf) continue;
                if (closedList.Exists(t => t.Tile == tile)) continue;

                float g = currentAStarTile.G + 1;
                float h = Vector3.Distance(tile.transform.position, endTile.transform.position);

                AStarTile child = new(tile, g, h, currentAStarTile.Tile);

                if (openList.Exists(t => t.Tile == tile))
                {
                    AStarTile openTile = openList.Find(t => t.Tile == tile);
                    if (g > openTile.G) continue;
                }
                openList.Add(child);
            }
        }
        Debug.LogError($"Did not find the path from {startTile} to {endTile}");
        return new();
    }
}
