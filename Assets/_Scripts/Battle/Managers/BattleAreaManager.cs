using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleAreaManager : MonoBehaviour
{
    [SerializeField] GameObject _tilePrefab;
    Vector2 _areaSize = new Vector2(5, 5); // has to be even for now

    List<BattleLandTile> _tiles = new List<BattleLandTile>();

    // Start is called before the first frame update
    public void Initialize()
    {
        CreateArea();
    }

    void CreateArea()
    {
        int halfAreaSizeX = (int)(_areaSize.x * 0.5f);
        int halfAreaSizeY = (int)(_areaSize.y * 0.5f);
        for (int x = -halfAreaSizeX; x <= halfAreaSizeX; x++)
        {
            for (int y = -halfAreaSizeY; y <= halfAreaSizeY; y++)
            {
                float posX = x * _tilePrefab.transform.localScale.x * 10;
                float posZ = y * _tilePrefab.transform.localScale.z * 10; // TODO: idk why * 10...
                Vector3 pos = new Vector3(posX, 0, posZ);
                GameObject tile = Instantiate(_tilePrefab, pos, Quaternion.identity);
                _tiles.Add(tile.GetComponent<BattleLandTile>());
                tile.transform.SetParent(transform);

                if (pos == Vector3.zero) continue;
                tile.SetActive(false);
            }
        }
    }

    // TODO: there must be a smarter way to get adjacent tiles
    public List<BattleLandTile> GetAdjacentTiles(BattleLandTile tile)
    {
        float tileScale = tile.transform.localScale.x * 10;
        List<BattleLandTile> adjacentTiles = new List<BattleLandTile>();
        Vector3 tilePos = tile.transform.position;
        foreach (BattleLandTile t in _tiles)
        {
            if (t.transform.position == tilePos) continue;

            if (t.transform.position.x == tilePos.x)
            {
                if (t.transform.position.z == tilePos.z + tileScale ||
                    t.transform.position.z == tilePos.z - tileScale)
                {
                    adjacentTiles.Add(t);
                }
            }
            else if (t.transform.position.z == tilePos.z)
            {
                if (t.transform.position.x == tilePos.x + tileScale ||
                    t.transform.position.x == tilePos.x - tileScale)
                {
                    adjacentTiles.Add(t);
                }
            }
        }

        Debug.Log($"adjacentTiles {adjacentTiles.Count}");

        return adjacentTiles;
    }


}
