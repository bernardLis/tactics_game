using UnityEngine;
using System.Linq;
using Pathfinding;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using System.Threading.Tasks;

[CreateAssetMenu(menuName = "ScriptableObject/Brain/Brain")]
public class Brain : BaseScriptableObject
{
    // global
    protected Highlighter highlighter;
    BasicCameraFollow basicCameraFollow;

    // tilemap
    protected Tilemap tilemap;
    protected WorldTile _tile;

    // Game Object components
    protected GameObject characterGameObject;
    protected EnemyStats enemyStats;

    // movement
    protected Seeker seeker;
    protected AILerp aiLerp;
    protected AIDestinationSetter destinationSetter;

    // interaction
    protected CharacterRendererManager characterRendererManager;
    public GameObject target;
    protected GameObject tempObject;
    protected List<PotentialTarget> potentialTargets;

    public Ability[] abilitiesToInstantiate;
    protected List<Ability> abilities;
    protected Ability selectedAbility;


    public virtual void Initialize(GameObject _self)
    {
        highlighter = GameManager.instance.GetComponent<Highlighter>();
        basicCameraFollow = BasicCameraFollow.instance;

        // This is our Dictionary of tiles
        tilemap = TileMapInstance.instance.GetComponent<Tilemap>();

        characterGameObject = _self;
        enemyStats = characterGameObject.GetComponent<EnemyStats>();
        characterGameObject.GetComponent<EnemyAI>().brain = this;

        seeker = characterGameObject.GetComponent<Seeker>();
        aiLerp = characterGameObject.GetComponent<AILerp>();
        destinationSetter = characterGameObject.GetComponent<AIDestinationSetter>();

        characterRendererManager = characterGameObject.GetComponentInChildren<CharacterRendererManager>();

        // instantiate abilities
        abilities = new();
        foreach (Ability a in abilitiesToInstantiate)
        {
            var c = Instantiate(a);
            c.Initialize(characterGameObject);
            abilities.Add(c);
        }
    }

    public virtual void Select()
    {
        target = null;
        basicCameraFollow.followTarget = characterGameObject.transform;
        highlighter.HiglightEnemyMovementRange(characterGameObject.transform.position,
                                               enemyStats.movementRange.GetValue(), Helpers.GetColor("movementBlue"));
    }

    public virtual void Move()
    {
        // meant to be overwritten
    }

    public virtual async Task Interact()
    {
        await selectedAbility.HighlightTargetable(characterGameObject);
        await Task.Delay(300);
        await selectedAbility.HighlightAreaOfEffect(target.transform.position);
        await Task.Delay(500);
        await selectedAbility.TriggerAbility(target);
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

    protected Vector3 GetDestinationWithoutTarget(List<PotentialTarget> potentialTargets)
    {
        Vector3 destinationPos = GetDestinationCloserTo(potentialTargets.FirstOrDefault());
        // get a random tile if there are no good tiles on path
        if (destinationPos == Vector3.zero)
            destinationPos = highlighter.highlightedTiles[Random.Range(0, highlighter.highlightedTiles.Count)].GetMiddleOfTile();

        return destinationPos;
    }

    // get destination will be different for each brain
    protected Vector3 GetDestinationCloserTo(PotentialTarget _target)
    {
        Path p = seeker.StartPath(characterGameObject.transform.position, _target.gObject.transform.position);
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
            if (!TileManager.tiles.TryGetValue(tilePos, out _tile))
                continue;

            // check if it is within reach and is not the tile I am currently standing on
            if (_tile.WithinRange)
                return _tile.GetMiddleOfTile();
        }

        return Vector3.zero;
    }

}
