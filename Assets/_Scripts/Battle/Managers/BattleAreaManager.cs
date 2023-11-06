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
    [SerializeField] GameObject _homeTilePrefab;
    [SerializeField] List<GameObject> _tilePrefabs;

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
                GameObject prefab = _tilePrefabs[Random.Range(0, _tilePrefabs.Count)];
                if (pos == Vector3.zero)
                    prefab = _homeTilePrefab;

                GameObject tile = Instantiate(prefab, _floorHolder); ;
                tile.transform.position = pos;
                BattleTile bt = tile.GetComponent<BattleTile>();
                bt.Initialize(tileScale);
                _tiles.Add(bt);
                tile.SetActive(false);

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

    public void ReplaceTile(BattleTile tile, GameObject newTile)
    {
        GameObject newTileObject = Instantiate(newTile, _floorHolder);
        newTileObject.transform.position = tile.transform.position;
        BattleTile newBattleTile = newTileObject.GetComponent<BattleTile>();
        newBattleTile.Initialize(HomeTile.Scale);
        _tiles.Insert(_tiles.IndexOf(tile), newBattleTile);
        _tiles.Remove(tile);
        newTileObject.SetActive(false);
        Destroy(tile.gameObject);
    }
}
