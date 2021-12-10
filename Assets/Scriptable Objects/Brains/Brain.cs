using UnityEngine;
using System.Linq;
using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "Brain/Brain")]
public class Brain : ScriptableObject
{
    // global
    protected Highlighter highlighter;

    // tilemap
    protected Dictionary<Vector3, WorldTile> tiles;
    protected Tilemap tilemap;
    protected WorldTile _tile;
    protected WorldTile currentTile;
    protected WorldTile targetTile;

    // Game Object components
    protected GameObject characterGameObject;
    protected EnemyStats enemyStats;
    protected Seeker seeker;


    public virtual void Initialize(GameObject _self)
    {
        highlighter = GameManager.instance.GetComponent<Highlighter>();

        // This is our Dictionary of tiles
        tiles = GameTiles.instance.tiles;
        tilemap = TileMapInstance.instance.GetComponent<Tilemap>();

        characterGameObject = _self;
        enemyStats = characterGameObject.GetComponent<EnemyStats>();
        seeker = characterGameObject.GetComponent<Seeker>();
        characterGameObject.GetComponent<EnemyAI>().brain = this;
    }

    // TODO: I might do it all async
    public virtual void Select()
    {
        highlighter.HiglightEnemyMovementRange(characterGameObject.transform.position,
                                               enemyStats.movementRange.GetValue(), Helpers.GetColor("movementBlue"));
    }

    public virtual void Move()
    {
        // meant to be overwritten
    }

    public virtual void Interact()
    {
        // meant to be overwritten
    }

    protected Dictionary<GameObject, float> GetPlayerCharactersOderedByDistance()
    {
        if (seeker == null)
            return null;

        GameObject[] playerCharacters = GameObject.FindGameObjectsWithTag("Player");
        Dictionary<GameObject, float> distToPlayerCharacters = new();
        // check distance between self and each player character,
        foreach (var pCharacter in playerCharacters)
        {
            // https://arongranberg.com/astar/documentation/dev_4_1_6_17dee0ac/calling-pathfinding.php
            Path p = seeker.StartPath(characterGameObject.transform.position, pCharacter.transform.position);
            p.BlockUntilCalculated();
            // The path is calculated now
            // distance is the path length 
            // https://arongranberg.com/astar/docs_dev/class_pathfinding_1_1_path.php#a1076ed6812e2b4f98dca64b74dabae5d
            float distance = p.GetTotalLength();
            distToPlayerCharacters.Add(pCharacter, distance);
        }

        distToPlayerCharacters.OrderByDescending(entry => entry.Value);
        return distToPlayerCharacters;
    }

    protected Dictionary<WorldTile, int> GetFreeTilesAround(GameObject _character)
    {
        List<WorldTile> freeTiles = new();

        // get players tile and then get the tile up, left, right and left from him
        Vector3 tilePos = tilemap.WorldToCell(_character.transform.position);

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
            
            // TODO: if you are standing on it, it should go into the list

            if (highlighter.CanEnemyWalkOnTile(_tile) && highlighter.CanEnemyStopOnTile(_tile))
                freeTiles.Add(_tile);
        }

        Dictionary<WorldTile, int> tilesWithAttackDirection = new();
        CharacterStats stats = _character.GetComponent<CharacterStats>();

        foreach (WorldTile tile in freeTiles)
        {
            Vector3 pos = new Vector3(tile.LocalPlace.x+0.5f, tile.LocalPlace.y+0.5f,tile.LocalPlace.z);
            tilesWithAttackDirection.Add(tile, stats.CalculateAttackDir(pos));
        }

        return tilesWithAttackDirection;
    }
}
