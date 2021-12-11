using UnityEngine;
using System.Linq;
using Pathfinding;
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

    protected List<PotentialTarget> GetPotentialTargets(string _tag)
    {
        if (seeker == null)
            return null;

        GameObject[] playerCharacters = GameObject.FindGameObjectsWithTag(_tag);
        List<PotentialTarget> potentialTargets = new();
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
            PotentialTarget potentialTarget = new PotentialTarget(pCharacter, distance);
            potentialTargets.Add(potentialTarget);
        }

        potentialTargets = potentialTargets.OrderByDescending(entry => entry.distanceToTarget).ToList();
        return potentialTargets;
    }

    // get destination will be different for each brain
    protected Vector3 GetDestinationCloserTo(PotentialTarget _target)
    {
        Path p = seeker.StartPath(characterGameObject.transform.position, _target.self.transform.position);
        p.BlockUntilCalculated();
        // The path is calculated now
        // We got our path back
        if (p.error)
            return Vector3.zero;

        Vector3Int tilePos;
        // Yay, now we can get a Vector3 representation of the path
        // loop from the target to self
        for (int i = p.vectorPath.Count - 1; i >= 0; i--)
        {
            tilePos = tilemap.WorldToCell(p.vectorPath[i]);
            if (!tiles.TryGetValue(tilePos, out _tile))
                continue;

            // check if it is within reach and is not the tile I am currently standing on
            if (_tile.WithinRange)
                return _tile.GetMiddleOfTile();
        }

        // no within range tile that is on path
        // selecting a random tile
        // TODO: something smarter
        WorldTile randomTile = highlighter.highlightedTiles[Random.Range(0, highlighter.highlightedTiles.Count)];
        return randomTile.GetMiddleOfTile();
    }

}
