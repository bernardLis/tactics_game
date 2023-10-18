using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class BattleAreaManager : MonoBehaviour
{
    [SerializeField] Transform _floorHolder;
    [SerializeField] GameObject _floorPrefab;
    [SerializeField] GameObject _tilePrefab;

    GameObject _floor;

    [HideInInspector] public BattleLandTile HomeTile;
    List<BattleLandTile> _tiles = new List<BattleLandTile>();

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
                GameObject tile = Instantiate(_tilePrefab, _floorHolder);
                Vector3 pos = new Vector3(x * tileScale, 0, z * tileScale);
                tile.transform.position = pos;
                BattleLandTile bt = tile.GetComponent<BattleLandTile>();
                bt.Initialize(tileScale);
                _tiles.Add(bt);
                tile.SetActive(false);

                if (pos == Vector3.zero)
                    HomeTile = tile.GetComponent<BattleLandTile>();
            }
        }

        HomeTile.EnableTile();
    }


    // TODO: there must be a smarter way to get adjacent tiles
    public List<BattleLandTile> GetAdjacentTiles(BattleLandTile tile)
    {
        List<BattleLandTile> adjacentTiles = new List<BattleLandTile>();
        Vector3 tilePos = tile.transform.position;
        foreach (BattleLandTile t in _tiles)
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
}
