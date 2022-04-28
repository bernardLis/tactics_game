using UnityEngine;
using System.Linq;
using Pathfinding;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using System.Threading.Tasks;

[CreateAssetMenu(menuName = "ScriptableObject/Brain/Brain")]
public class Brain : BaseScriptableObject
{
    public string Name;
    public Sprite Portrait;

    // TODO: curves instead of floats
    public float StrengthMultiplier;
    public float IntelligenceMultiplier;
    public float AgilityMultiplier;
    public float StaminaMultiplier;

    public Equipment Body;
    public Weapon Weapon;

    // global
    protected Highlighter _highlighter;
    CameraManager _cameraManager;
    protected TurnManager _turnManager;
    InfoCardUI _infoCardUI;

    // tilemap
    protected Tilemap _tilemap;
    protected WorldTile _tile;

    // Game Object components
    protected GameObject _characterGameObject;
    protected EnemyStats _enemyStats;

    // node blocker
    BlockManager _blockManager;
    List<SingleNodeBlocker> _nodeBlockers;
    BlockManager.TraversalProvider _traversalProvider;

    // movement
    protected AILerp _aiLerp;

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
        _highlighter = BattleManager.Instance.GetComponent<Highlighter>();
        _cameraManager = Helpers.Camera.GetComponent<CameraManager>();
        _turnManager = TurnManager.Instance;
        _infoCardUI = InfoCardUI.Instance;
        _blockManager = FindObjectOfType<BlockManager>();

        _tilemap = BattleManager.Instance.GetComponent<TileManager>().Tilemap;

        _characterGameObject = self;
        _enemyStats = _characterGameObject.GetComponent<EnemyStats>();
        _characterGameObject.GetComponent<EnemyAI>().SetBrain(this);

        _aiLerp = _characterGameObject.GetComponent<AILerp>();
        _aiLerp.canSearch = false;

        _characterRendererManager = _characterGameObject.GetComponentInChildren<CharacterRendererManager>();

        // instantiate abilities
        _abilities = new();
        foreach (Ability ability in AbilitiesToInstantiate)
        {
            Ability abilityClone = Instantiate(ability);
            abilityClone.Initialize(_characterGameObject);
            _abilities.Add(abilityClone);

            _enemyStats.Abilities.Add(abilityClone);
        }
    }

    public virtual async Task Select()
    {
        _infoCardUI.ShowCharacterCard(_enemyStats);

        Target = null;
        _cameraManager.SetTarget(_characterGameObject.transform);
        await _highlighter.HighlightCharacterMovementRange(_enemyStats, Tags.Player);

        // node blockers
        AstarPath.active.Scan();

        _nodeBlockers = new();
        foreach (GameObject e in _turnManager.GetPlayerCharacters())
        {
            e.GetComponent<CharacterSelection>().ActivateSingleNodeBlocker();
            _nodeBlockers.Add(e.GetComponent<SingleNodeBlocker>());
        }
    }

    public virtual async Task Move()
    {
        // meant to be overwritten
        await Task.Delay(1); // To silence errors.
    }

    public virtual async Task Interact()
    {
        _infoCardUI.ShowManaChange(_enemyStats, -_selectedAbility.ManaCost);

        await _selectedAbility.HighlightTargetable(_characterGameObject);
        await Task.Delay(300);
        await _selectedAbility.HighlightAreaOfEffect(Target.transform.position);
        await Task.Delay(500);
        await _selectedAbility.TriggerAbility(_highlighter.HighlightedTiles);

        _infoCardUI.ShowCharacterCard(_enemyStats);
    }

    protected void Defend()
    {
        // TODO: this is wrong way to select defend ability, but it let's keep it for now.
        _selectedAbility = _abilities.FirstOrDefault(a => a.Id == "5f7d8c47-7ec1-4abf-b8ec-74ea82be327f");
        Target = _characterGameObject;
    }

    protected List<PotentialTarget> GetPotentialTargets(string targetTag)
    {
        GameObject[] targetCharacters = GameObject.FindGameObjectsWithTag(targetTag);
        List<PotentialTarget> potentialTargets = new();
        // check distance between self and each player character,
        foreach (GameObject targetChar in targetCharacters)
        {
            targetChar.GetComponent<CharacterSelection>().DeactivateSingleNodeBlocker();
            ABPath p = GetPathTo(targetChar.transform);

            // distance is the path length 
            // https://arongranberg.com/astar/docs_dev/class_pathfinding_1_1_path.php#a1076ed6812e2b4f98dca64b74dabae5d
            float distance = p.GetTotalLength();
            PotentialTarget potentialTarget = new PotentialTarget(targetChar, distance);
            potentialTargets.Add(potentialTarget);

            targetChar.GetComponent<CharacterSelection>().ActivateSingleNodeBlocker();
        }

        potentialTargets = potentialTargets.OrderByDescending(entry => entry.DistanceToTarget).ToList();
        return potentialTargets;
    }

    protected PotentialTarget GetClosestPotentialTargetWithTag(string tag)
    {
        List<PotentialTarget> opponents = GetPotentialTargets(tag);
        opponents.Reverse();
        foreach (PotentialTarget opp in opponents)
            if (opp.DistanceToTarget != 0)
                return opp;
        return opponents[0];
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
        ABPath p = GetPathTo(target.GameObj.transform);

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

    protected ABPath GetPathTo(Transform t)
    {
        // when pathing to single node blocker, path is 0
        CharacterSelection charSelection = t.GetComponent<CharacterSelection>();
        if (charSelection != null)
            charSelection.DeactivateSingleNodeBlocker();

        // Scanning graph breaks node blockers. Whenever I scan graph I need to add node blockers again.
        // https://arongranberg.com/astar/documentation/dev_4_0_6_e07eb1b/class_single_node_blocker.php
        _traversalProvider = new BlockManager.TraversalProvider(_blockManager, BlockManager.BlockMode.OnlySelector, _nodeBlockers);

        // https://arongranberg.com/astar/docs_dev/calling-pathfinding.php
        // Create a new Path object
        ABPath path = ABPath.Construct(_characterGameObject.transform.position, t.position, null);
        // Make the path use a specific traversal provider
        path.traversalProvider = _traversalProvider;
        // Calculate the path
        AstarPath.StartPath(path);
        AstarPath.BlockUntilCalculated(path);

        if (charSelection != null)
            charSelection.ActivateSingleNodeBlocker();

        return path;
    }

    protected void SetPath(ABPath p)
    {
        _aiLerp.SetPath(p);
        _aiLerp.destination = _tempObject.transform.position;
    }
}
