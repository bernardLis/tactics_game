using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
public class PotentialTarget
{
    public GameObject gObject;
    public float distanceToTarget;

    public PotentialTarget(GameObject _gameObject, float _distanceToTarget)
    {
        gObject = _gameObject;
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
        Vector3 tilePos = tilemap.WorldToCell(gObject.transform.position);

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

            // if attacker is standing on it, it should go into the list, but also if there is some object 
            // like trap or other stuff that is passable
            Collider2D col = Physics2D.OverlapCircle(_tile.GetMiddleOfTile(), 0.2f);
            if (col == null)
                continue;
            
            if (col.transform.parent.gameObject == _attacker)
                freeTiles.Add(_tile);
        }

        List<AttackPosition> attackPositions = new();
        CharacterStats stats = gObject.GetComponent<CharacterStats>();

        foreach (WorldTile tile in freeTiles)
            attackPositions.Add(new AttackPosition(gObject, tile, stats.CalculateAttackDir(tile.GetMiddleOfTile())));

        return attackPositions;
    }

}
