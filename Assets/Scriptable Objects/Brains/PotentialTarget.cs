using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
public class PotentialTarget
{
    public GameObject self;
    public float distanceToTarget;

    public PotentialTarget(GameObject _self, float _distanceToTarget)
    {
        self = _self;
        distanceToTarget = _distanceToTarget;
    }


    public List<AttackPosition> GetAttackPositions(GameObject _attacker)
    {
        // This is our Dictionary of tiles
        Dictionary<Vector3, WorldTile> tiles = GameTiles.instance.tiles;
        Tilemap tilemap = TileMapInstance.instance.GetComponent<Tilemap>();
        WorldTile _tile;

        Highlighter highlighter = Highlighter.instance;

        List<WorldTile> freeTiles = new();

        // get players tile and then get the tile up, left, right and left from him
        Vector3 tilePos = tilemap.WorldToCell(self.transform.position);

        // check tiles around target player 
        Vector3[] tilesAroundPlayer = {
            new Vector3(tilePos.x+1, tilePos.y, tilePos.z),
            new Vector3(tilePos.x-1, tilePos.y, tilePos.z),
            new Vector3(tilePos.x, tilePos.y+1, tilePos.z),
            new Vector3(tilePos.x, tilePos.y-1, tilePos.z)
        };

        // for each point check if there is a within reach tile
        foreach (Vector3 point in tilesAroundPlayer)
        {
            if (!tiles.TryGetValue(point, out _tile))
                continue;

            if (highlighter.CanEnemyWalkOnTile(_tile) && highlighter.CanEnemyStopOnTile(_tile))
            {
                freeTiles.Add(_tile);
                continue;
            }

            // if attacker is standing on it, it should go into the list
            Collider2D col = Physics2D.OverlapCircle(point, 0.2f);
            if (col == null)
                continue;

            if (col.gameObject == _attacker)
                freeTiles.Add(_tile);
        }

        List<AttackPosition> attackPositions = new();
        CharacterStats stats = self.GetComponent<CharacterStats>();

        foreach (WorldTile tile in freeTiles)
        {
            Vector3 pos = new Vector3(tile.LocalPlace.x + 0.5f, tile.LocalPlace.y + 0.5f, tile.LocalPlace.z);
            attackPositions.Add(new AttackPosition(self, tile, stats.CalculateAttackDir(pos)));
        }

        return attackPositions;
    }

}
