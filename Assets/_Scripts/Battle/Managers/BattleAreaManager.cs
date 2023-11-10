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
    [SerializeField] Transform _floorHolder;
    [SerializeField] GameObject _floorPrefab;

    [SerializeField] GameObject _tilePrefab;
    [SerializeField] Building _homeBuilding;
    [SerializeField] List<Building> _buildings;

    GameObject _floor;

    [HideInInspector] public BattleTile HomeTile;
    List<BattleTile> _tiles = new();
    public List<BattleTile> PurchasedTiles = new();

    public event Action OnTilePurchased;

    public void Initialize()
    {
        float tileScale = _floorPrefab.transform.localScale.x;
        _floor = Instantiate(_floorPrefab,
                new Vector3(-tileScale * 0.5f, 0, -tileScale * 0.5f), // floor offset to make tiles centered
                Quaternion.identity);
        _floor.transform.SetParent(_floorHolder);

        CreateArea();
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

                Building building = _buildings[Random.Range(0, _buildings.Count)];
                if (pos == Vector3.zero)
                    building = _homeBuilding;

                BattleTile bt = InstantiateTile(pos, building);
                _tiles.Add(bt);

                if (pos == Vector3.zero)
                    HomeTile = bt;
            }
        }
        PurchasedTiles.Add(HomeTile);
        HomeTile.EnableTile();
        HomeTile.HandleBorders(new Color(1f, 0.22f, 0f, 0.2f)); // magic color
    }

    public void TilePurchased(BattleTile tile)
    {
        PurchasedTiles.Add(tile);
        OnTilePurchased?.Invoke();
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

    public void ReplaceTile(BattleTile tile, Building newBuilding)
    {
        BattleTile newBattleTile = InstantiateTile(tile.transform.position, newBuilding);

        _tiles.Insert(_tiles.IndexOf(tile), newBattleTile);
        _tiles.Remove(tile);
        Destroy(tile.gameObject);
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
