using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Pathfinding;
using System.Threading.Tasks;

public class BattleCharacterController : Singleton<BattleCharacterController>
{
    // global utilities
    HighlightManager _highlighter;
    CharacterUI _characterUI;
    BattleInputController _battleInputController;
    MovePointController _movePointController;
    TurnManager _turnManager;

    // tilemap
    Tilemap _tilemap;
    WorldTile _tile;
    WorldTile _selectedTile;

    // I will be caching them for selected character
    [HideInInspector] public GameObject SelectedCharacter;
    CharacterStats _playerStats;
    PlayerCharSelection _playerCharSelection;
    AILerp _aiLerp;

    // state
    public CharacterState CharacterState { get; private set; }

    // selection
    bool _isSelectionBlocked; // block mashing select character coz it breaks the highlight - if you quickly switch between selection of 2 chars.

    // node blocker
    BlockManager _blockManager;
    List<SingleNodeBlocker> _nodeBlockers;
    BlockManager.TraversalProvider _traversalProvider;

    // movement
    GameObject _tempObject;
    public bool HasCharacterStartedMoving { get; private set; }
    public bool IsMovingBack { get; private set; }

    LineRenderer _pathRenderer;

    // interactions
    public Ability SelectedAbility { get; private set; }
    List<GameObject> _viableTargets = new();
    bool _isInteracting;

    public static event Action<CharacterState> OnCharacterStateChanged;

    protected override void Awake()
    {
        base.Awake();
    }

    void Start()
    {
        _tilemap = BattleManager.Instance.GetComponent<TileManager>().Tilemap;

        _highlighter = HighlightManager.Instance;
        _battleInputController = BattleInputController.Instance;
        _characterUI = CharacterUI.Instance;
        _movePointController = MovePointController.Instance;
        _turnManager = TurnManager.Instance;

        _pathRenderer = GetComponent<LineRenderer>();

        _blockManager = FindObjectOfType<BlockManager>();

        MovePointController.OnMove += MovePointController_OnMove;
    }

    void OnDestroy()
    {
        MovePointController.OnMove -= MovePointController_OnMove;
    }

    void MovePointController_OnMove(Vector3 pos)
    {
        StartCoroutine(DrawPath());
    }

    void Update()
    {
        // TODO: is there a better way? 
        if (SelectedCharacter == null)
            return;

        if (HasCharacterStartedMoving && _tempObject != null
            && Vector3.Distance(SelectedCharacter.transform.position, _tempObject.transform.position) <= 0.1f)
            CharacterReachedDestination();
    }

    // https://www.youtube.com/watch?v=4I0vonyqMi8&t=193s
    public void UpdateCharacterState(CharacterState newState)
    {
        CharacterState = newState;
        // TODO: this is not really implemented...
        switch (newState)
        {
            case CharacterState.None:
                break;
            case CharacterState.Selected:
                HandleCharacterSelected();
                break;
            case CharacterState.Moved:
                break;
            case CharacterState.SelectingInteractionTarget:
                break;
            case CharacterState.SelectingFaceDir:
                break;
            case CharacterState.ConfirmingInteraction:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        }
        OnCharacterStateChanged?.Invoke(newState);
    }

    async void HandleCharacterSelected()
    {
        // highlight
        _battleInputController.SetInputAllowed(false);
        await _highlighter.HighlightCharacterMovementRange(_playerStats, Tags.Enemy);
        _battleInputController.SetInputAllowed(true);

        _isSelectionBlocked = false;
    }

    public void Select(Collider2D[] cols)
    {
        if (TurnManager.BattleState != BattleState.PlayerTurn)
            return;

        ClearPathRenderer();
        foreach (Collider2D c in cols)
        {
            // select character
            if (c != null && CanSelectCharacter(c))
            {
                SelectCharacter(c.gameObject);
                return;
            }
        }

        // get the tile movepoint is on
        if (TileManager.Tiles.TryGetValue(_tilemap.WorldToCell(transform.position), out _tile))
            _selectedTile = _tile;

        if (SelectedCharacter == null)
            return;

        // Move
        if (CanMoveCharacter() && _selectedTile.WithinRange)
        {
            Move();
            return;
        }

        Interact();
    }

    bool CanSelectCharacter(Collider2D col)
    {
        if (_isSelectionBlocked)
            return false;
        if (col == null)
            return false;
        // can select only player characters
        if (!col.transform.CompareTag(Tags.Player))
            return false;
        // character allows selection
        if (!col.GetComponentInParent<PlayerCharSelection>().CanBeSelected())
            return false;
        // don't allow to select another character if you have moved this character and did not take the action
        if (SelectedCharacter != null && _playerCharSelection.HasMovedThisTurn && !_playerCharSelection.HasFinishedTurn)
            return false;
        // don't allow to select another character if you are triggering ability;
        if (SelectedAbility != null)
            return false;

        return true;
    }

    bool CanMoveCharacter()
    {
        if (_playerCharSelection.HasMovedThisTurn)
            return false;
        if (_playerCharSelection.HasFinishedTurn)
            return false;
        if (SelectedAbility != null)
            return false;

        return true;
    }

    // for ability button clicks
    public bool CanSelectAbility()
    {
        if (!_battleInputController.IsInputAllowed())
            return false;
        if (SelectedCharacter == null)
            return false;
        if (_playerCharSelection.HasFinishedTurn)
            return false;
        if (HasCharacterStartedMoving) // don't allow button click when character is moving;
            return false;
        return true;
    }

    void SelectCharacter(GameObject character)
    {
        _isSelectionBlocked = true;

        // character specific ui
        if (_playerCharSelection != null)
            _playerCharSelection.DeselectCharacter();

        // cache
        SelectedCharacter = character;
        _playerStats = SelectedCharacter.GetComponent<CharacterStats>();
        _playerCharSelection = SelectedCharacter.GetComponent<PlayerCharSelection>();
        _aiLerp = SelectedCharacter.GetComponent<AILerp>();
        _aiLerp.canSearch = false;

        // character specific ui
        _playerCharSelection.SelectCharacter();

        UpdateCharacterState(CharacterState.Selected);
    }


    void Move()
    {
        ClearPathRenderer();

        HasCharacterStartedMoving = true;

        // TODO: should I make it all async?
        _highlighter.ClearHighlightedTiles().GetAwaiter();

        var path = GetPathTo(transform);
        _aiLerp.SetPath(path);

        _tempObject = new GameObject("Destination");
        _tempObject.transform.position = transform.position;
        _aiLerp.destination = _tempObject.transform.position;

        _characterUI.DisableSkillButtons();

        _playerCharSelection.SetCharacterMoved(true);
    }

    public async void Back()
    {
        ClearPathRenderer();

        // if back is called during character movement return
        if (HasCharacterStartedMoving)
            return;

        if (SelectedCharacter == null)
            return;

        if (CharacterState == CharacterState.ConfirmingInteraction)
        {
            await BackFromConfirmingInteraction();
            return;
        }

        if (CharacterState == CharacterState.SelectingFaceDir)
        {
            await BackFromFaceDirSelection();
            return;
        }

        if (SelectedAbility != null)
        {
            BackFromAbilitySelection();
            return;
        }

        if (!_playerCharSelection.HasMovedThisTurn)
        {
            UnselectCharacter();
            return;
        }

        // flag reset
        _playerCharSelection.SetCharacterMoved(false);

        IsMovingBack = true;
        HasCharacterStartedMoving = true;

        // move character to character's starting position quickly.
        _aiLerp.speed = 15;
        transform.position = _playerCharSelection.PositionTurnStart;

        _tempObject = new GameObject("Back Destination");
        _tempObject.transform.position = _playerCharSelection.PositionTurnStart;
        var path = GetPathTo(_tempObject.transform);
        _aiLerp.SetPath(path);
        _aiLerp.destination = _tempObject.transform.position;

        _characterUI.DisableSkillButtons();
    }

    void CharacterReachedDestination()
    {
        _characterUI.EnableSkillButtons();

        if (_tempObject != null)
            Destroy(_tempObject);

        // reset flag
        HasCharacterStartedMoving = false;

        // check if it was back or normal move
        if (!IsMovingBack)
        {
            UpdateCharacterState(CharacterState.Moved);
            return;
        }

        // highlight movement range if character was going back
        IsMovingBack = false;
        // remove statuses that were applied when moving
        _playerStats.ResolveGoingBack();

        UpdateCharacterState(CharacterState.Selected);
    }

    public void SetSelectedAbility(Ability ability)
    {
        ClearPathRenderer();
        SelectedAbility = ability;
        UpdateCharacterState(CharacterState.SelectingInteractionTarget);
    }

    public async void Interact()
    {
        // TODO: this is a meh schema
        if (_isInteracting)
            return;
        _isInteracting = true;

        // highlight aoe
        if (CharacterState == CharacterState.SelectingInteractionTarget)
        {
            _battleInputController.SetInputAllowed(false);
            await SelectedAbility.HighlightAreaOfEffect(transform.position);
            _battleInputController.SetInputAllowed(true);

            _isInteracting = false;
            return;
        }

        // fixes a bug where when you walked and clicked on yourself you ended your turn
        if (SelectedAbility == null)
        {
            _isInteracting = false;
            return;
        }
        _battleInputController.SetInputAllowed(false);
        await SelectedAbility.TriggerAbility(_highlighter.HighlightedTiles);
        _battleInputController.SetInputAllowed(true);
        _isInteracting = false;

        FinishCharacterTurn();
    }


    void BackFromAbilitySelection()
    {
        _isInteracting = false;
        SelectedAbility = null;
        _highlighter.ClearHighlightedTiles().GetAwaiter();

        if (_playerCharSelection.HasMovedThisTurn)
            UpdateCharacterState(CharacterState.Moved);
        else
            UpdateCharacterState(CharacterState.Selected);

    }

    async Task BackFromFaceDirSelection()
    {
        _isInteracting = false;

        // TODO:cache face direction ui if it is the right approach
        SelectedCharacter.GetComponent<FaceDirectionUI>().HideUI();
        _playerCharSelection.ToggleSelectionArrow(true);

        // abilities that can target self should go back to Select target
        if (SelectedAbility.CanTargetSelf)
        {
            // it changes the state too
            _battleInputController.SetInputAllowed(false);
            await SelectedAbility.HighlightTargetable(SelectedCharacter);
            GetViableTargets();
            _battleInputController.SetInputAllowed(true);
            return;
        }

        // deselect ability if it was an ability that goes straight to facing dir;
        BackFromAbilitySelection();
    }

    async Task BackFromConfirmingInteraction()
    {
        _isInteracting = false;
        _battleInputController.SetInputAllowed(false);
        await SelectedAbility.HighlightTargetable(SelectedCharacter);
        GetViableTargets();
        _battleInputController.SetInputAllowed(true);

        UpdateCharacterState(CharacterState.SelectingInteractionTarget);
    }

    void FinishCharacterTurn()
    {
        _isInteracting = false;

        // set flags in player char selection
        if (_playerCharSelection != null)
            _playerCharSelection.FinishCharacterTurn();

        // clearing the cache here
        UnselectCharacter();
        AstarPath.active.ScanAsync();

        UpdateCharacterState(CharacterState.None);
    }

    public void UnselectCharacter()
    {
        // character specific ui
        if (_playerCharSelection != null)
            _playerCharSelection.DeselectCharacter();

        SelectedCharacter = null;
        _playerStats = null;
        _playerCharSelection = null;
        SelectedAbility = null;

        // highlight
        _highlighter.ClearHighlightedTiles().GetAwaiter();

        UpdateCharacterState(CharacterState.None);
    }

    // TODO: this probably shouldn't be here 
    IEnumerator DrawPath()
    {
        ClearPathRenderer();

        // only draw path when character is selected
        if (SelectedCharacter == null)
            yield break;

        // don't draw path if ability is selected
        if (SelectedAbility != null)
            yield break;

        // get the tile movepoint is on
        if (!TileManager.Tiles.TryGetValue(_tilemap.WorldToCell(transform.position), out _tile))
            yield break;
        // don't draw path to tiles you can't reach
        if (!_tile.WithinRange)
            yield break;

        ABPath path = GetPathTo(transform);
        yield return StartCoroutine(path.WaitForPath());

        if (path.error)
            yield break;

        for (int i = 0; i < path.vectorPath.Count; i++)
        {
            _pathRenderer.positionCount = path.vectorPath.Count;
            _pathRenderer.SetPosition(i, new Vector3(path.vectorPath[i].x, path.vectorPath[i].y, -1f));
        }
    }

    void ClearPathRenderer()
    {
        _pathRenderer.positionCount = 0;
    }

    ABPath GetPathTo(Transform t)
    {
        // Scanning graph breaks node blockers. Whenever I scan graph I need to add node blockers again.
        // https://arongranberg.com/astar/documentation/dev_4_0_6_e07eb1b/class_single_node_blocker.php
        _nodeBlockers = new();
        foreach (GameObject e in _turnManager.GetEnemies())
        {
            e.GetComponent<CharacterSelection>().ActivateSingleNodeBlocker();
            _nodeBlockers.Add(e.GetComponent<SingleNodeBlocker>());
        }
        _traversalProvider = new BlockManager.TraversalProvider(_blockManager, BlockManager.BlockMode.OnlySelector, _nodeBlockers);

        // https://arongranberg.com/astar/docs_dev/calling-pathfinding.php
        // Create a new Path object
        ABPath path = ABPath.Construct(SelectedCharacter.transform.position, t.position, null);
        // Make the path use a specific traversal provider
        path.traversalProvider = _traversalProvider;
        // Calculate the path
        AstarPath.StartPath(path);
        AstarPath.BlockUntilCalculated(path);

        return path;
    }

    public void GetViableTargets()
    {
        // for all highlighted tiles, check if selected ability can interact with that
        // add them to the list 
        // move movepoint to the first one
        _viableTargets.Clear();

        foreach (WorldTile tile in _highlighter.HighlightedTiles)
        {
            Collider2D[] cols = Physics2D.OverlapCircleAll(tile.GetMiddleOfTile(), 0.2f);
            foreach (Collider2D c in cols)
                if (SelectedAbility.IsTargetViable(c.gameObject))
                    _viableTargets.Add(c.gameObject);
        }

        if (_viableTargets.Count > 0)
            _movePointController.Move(_viableTargets[0].transform.position);
    }
}
