using UnityEngine;
using System.Linq;
using Pathfinding;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using System.Threading.Tasks;

[CreateAssetMenu(menuName = "ScriptableObject/Brain/Brain")]
public class Brain : BaseScriptableObject
{
    public Equipment Body;
    public Weapon Weapon;

    // global
    protected Highlighter _highlighter;
    CameraManager _cameraManager;

    // tilemap
    protected Tilemap _tilemap;
    protected WorldTile _tile;

    // Game Object components
    protected GameObject _characterGameObject;
    protected EnemyStats _enemyStats;

    // movement
    protected Seeker _seeker;
    protected AILerp _aiLerp;
    protected AIDestinationSetter _destinationSetter;

    // interaction
    protected CharacterRendererManager _characterRendererManager;
    public GameObject Target;
    protected GameObject _tempObject;
    protected List<PotentialTarget> _potentialTargets;

    public Ability[] AbilitiesToInstantiate;
    protected List<Ability> _abilities;
    protected Ability _selectedAbility;


    public virtual void Initialize(GameObject self)
    {
        _highlighter = BattleManager.instance.GetComponent<Highlighter>();
        _cameraManager = CameraManager.instance;

        _tilemap = BattleManager.instance.GetComponent<TileManager>().Tilemap;

        _characterGameObject = self;
        _enemyStats = _characterGameObject.GetComponent<EnemyStats>();
        _characterGameObject.GetComponent<EnemyAI>().SetBrain(this);

        _seeker = _characterGameObject.GetComponent<Seeker>();
        _aiLerp = _characterGameObject.GetComponent<AILerp>();
        _destinationSetter = _characterGameObject.GetComponent<AIDestinationSetter>();

        _characterRendererManager = _characterGameObject.GetComponentInChildren<CharacterRendererManager>();

        // instantiate abilities
        _abilities = new();
        foreach (Ability ability in AbilitiesToInstantiate)
        {
            Ability abilityClone = Instantiate(ability);
            abilityClone.Initialize(_characterGameObject);
            _abilities.Add(abilityClone);
        }
    }

    public virtual void Select()
    {
        Target = null;
        _cameraManager.SetTarget(_characterGameObject.transform);
        _highlighter.HiglightEnemyMovementRange(_characterGameObject.transform.position,
                                               _enemyStats.MovementRange.GetValue(), Helpers.GetColor("movementBlue"));
    }

    public virtual void Move()
    {
        // meant to be overwritten
    }

    public virtual async Task Interact()
    {
        await _selectedAbility.HighlightTargetable(_characterGameObject);
        await Task.Delay(300);
        await _selectedAbility.HighlightAreaOfEffect(Target.transform.position);
        await Task.Delay(500);
        await _selectedAbility.TriggerAbility(Target);
    }

    protected List<PotentialTarget> GetPotentialTargets(string targetTag)
    {
        if (_seeker == null)
            return null;

        GameObject[] targetCharacters = GameObject.FindGameObjectsWithTag(targetTag);
        List<PotentialTarget> potentialTargets = new();
        // check distance between self and each player character,
        foreach (var targetChar in targetCharacters)
        {
            // https://arongranberg.com/astar/documentation/dev_4_1_6_17dee0ac/calling-pathfinding.php
            Path p = _seeker.StartPath(_characterGameObject.transform.position, targetChar.transform.position);
            p.BlockUntilCalculated();
            // The path is calculated now
            // distance is the path length 
            // https://arongranberg.com/astar/docs_dev/class_pathfinding_1_1_path.php#a1076ed6812e2b4f98dca64b74dabae5d
            float distance = p.GetTotalLength();
            PotentialTarget potentialTarget = new PotentialTarget(targetChar, distance);
            potentialTargets.Add(potentialTarget);
        }

        potentialTargets = potentialTargets.OrderByDescending(entry => entry.DistanceToTarget).ToList();
        return potentialTargets;
    }

    protected Vector3 GetDestinationWithoutTarget(List<PotentialTarget> potentialTargets)
    {
        Vector3 destinationPos = GetDestinationCloserTo(potentialTargets.FirstOrDefault());
        // get a random tile if there are no good tiles on path
        if (destinationPos == Vector3.zero)
            destinationPos = _highlighter.HighlightedTiles[Random.Range(0, _highlighter.HighlightedTiles.Count)].GetMiddleOfTile();

        return destinationPos;
    }

    protected Vector3 GetDestinationCloserTo(PotentialTarget target)
    {
        Path p = _seeker.StartPath(_characterGameObject.transform.position, target.GameObj.transform.position);
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
            tilePos = _tilemap.WorldToCell(p.vectorPath[i]);
            if (!TileManager.Tiles.TryGetValue(tilePos, out _tile))
                continue;

            // check if it is within reach and is not the tile I am currently standing on
            if (_tile.WithinRange)
                return _tile.GetMiddleOfTile();
        }

        return Vector3.zero; // TODO: this is dangerous
    }

}
