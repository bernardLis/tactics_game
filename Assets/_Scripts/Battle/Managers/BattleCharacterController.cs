using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Pathfinding;
using System.Threading.Tasks;

public enum CharacterState { None, Selected, SelectingInteractionTarget, SelectingFaceDir, ConfirmingInteraction }

public class BattleCharacterController : Singleton<BattleCharacterController>
{
    // global utilities
    Highlighter _highlighter;
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
    CharacterRendererManager _characterRendererManager;

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
    bool _hasCharacterStartedMoving;
    bool _hasCharacterGoneBack;

    LineRenderer _pathRenderer;

    // interactions
    public Ability SelectedAbility { get; private set; }
    bool _isInteracting;

    // TODO: I am currently, not using that, but I have a feeling that it will be useful.
    public static event Action<CharacterState> OnCharacterStateChanged;

    protected override void Awake()
    {
        base.Awake();
    }

    void Start()
    {
        _tilemap = BattleManager.Instance.GetComponent<TileManager>().Tilemap;

        _highlighter = Highlighter.Instance;
        _battleInputController = BattleInputController.Instance;
        _characterUI = CharacterUI.Instance;
        _movePointController = MovePointController.Instance;
        _turnManager = TurnManager.Instance;

        _pathRenderer = GetComponent<LineRenderer>();

        _blockManager = FindObjectOfType<BlockManager>();
    }

    void Update()
    {
        // TODO: is there a better way? 
        if (SelectedCharacter == null)
            return;

        if (_hasCharacterStartedMoving && _tempObject != null
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


    public void Select(Collider2D col)
    {
        ClearPathRenderer();

        // get the tile movepoint is on
        if (TileManager.Tiles.TryGetValue(_tilemap.WorldToCell(transform.position), out _tile))
            _selectedTile = _tile;

        // select character
        if (col != null && CanSelectCharacter(col))
        {
            SelectCharacter(col.gameObject);
            return;
        }

        if (SelectedCharacter == null)
            return;

        // when character is selected

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
        if (_hasCharacterStartedMoving) // don't allow button click when character is moving;
            return false;
        return true;
    }

    async void SelectCharacter(GameObject character)
    {
        _isSelectionBlocked = true;
        UpdateCharacterState(CharacterState.Selected);

        if (character.GetComponent<PlayerCharSelection>().HasMovedThisTurn)
            return;

        // character specific ui
        if (_playerCharSelection != null)
            _playerCharSelection.DeselectCharacter();

        // cache
        SelectedCharacter = character;
        _playerStats = SelectedCharacter.GetComponent<CharacterStats>();
        _playerCharSelection = SelectedCharacter.GetComponent<PlayerCharSelection>();
        _aiLerp = SelectedCharacter.GetComponent<AILerp>();
        _aiLerp.canSearch = false;
        _characterRendererManager = SelectedCharacter.GetComponentInChildren<CharacterRendererManager>();

        // character specific ui
        _playerCharSelection.SelectCharacter();

        // UI
        _characterUI.ShowCharacterUI(_playerStats);

        // highlight
        _battleInputController.SetInputAllowed(false);
        await _highlighter.HighlightCharacterMovementRange(_playerStats, Tags.Enemy);
        _battleInputController.SetInputAllowed(true);

        _isSelectionBlocked = false;
    }

    void Move()
    {
        ClearPathRenderer();

        _hasCharacterStartedMoving = true;

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
        _movePointController.UpdateDisplayInformation();

        // if back is called during character movement return
        if (_hasCharacterStartedMoving)
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

        _hasCharacterGoneBack = true;
        _hasCharacterStartedMoving = true;

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
        _hasCharacterStartedMoving = false;

        // TODO: maybe here check if there is interaction target that we are standing on and allow that interaction

        // check if it was back or normal move
        if (!_hasCharacterGoneBack)
            return;

        // highlight movement range if character was going back
        _hasCharacterGoneBack = false;
        _battleInputController.SetInputAllowed(false);
        _highlighter.HighlightCharacterMovementRange(_playerStats, Tags.Enemy).GetAwaiter(); // TODO:
        _battleInputController.SetInputAllowed(true);
    }

    public void SetSelectedAbility(Ability ability)
    {
        ClearPathRenderer();
        SelectedAbility = ability;

        _movePointController.UpdateDisplayInformation();
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
            _characterRendererManager.Face((transform.position - SelectedCharacter.transform.position).normalized);
            _battleInputController.SetInputAllowed(false);
            await SelectedAbility.HighlightAreaOfEffect(transform.position);
            _battleInputController.SetInputAllowed(true);

            _movePointController.UpdateDisplayInformation();
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

        UpdateCharacterState(CharacterState.Selected);

        SelectedAbility = null;
        _characterUI.HideAbilityTooltip();

        _highlighter.ClearHighlightedTiles().GetAwaiter();
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
            _battleInputController.SetInputAllowed(true);
            return;
        }

        // deselect ability if it was an ability that goes straight to facing dir;
        BackFromAbilitySelection();
        UpdateCharacterState(CharacterState.Selected);
    }

    async Task BackFromConfirmingInteraction()
    {
        _isInteracting = false;
        UpdateCharacterState(CharacterState.SelectingInteractionTarget);
        _battleInputController.SetInputAllowed(false);
        await SelectedAbility.HighlightTargetable(SelectedCharacter);
        _battleInputController.SetInputAllowed(true);

    }


    void FinishCharacterTurn()
    {
        _isInteracting = false;

        // necessary for movepoint to correctly update UI;
        UpdateCharacterState(CharacterState.None);

        // update ui through movepoint
        _movePointController.UpdateDisplayInformation();

        // set flags in player char selection
        if (_playerCharSelection != null)
            _playerCharSelection.FinishCharacterTurn();

        // clearing the cache here
        UnselectCharacter();
    }

    public void UnselectCharacter()
    {
        UpdateCharacterState(CharacterState.None);

        // character specific ui
        if (_playerCharSelection != null)
            _playerCharSelection.DeselectCharacter();

        SelectedCharacter = null;
        _playerStats = null;
        _playerCharSelection = null;
        SelectedAbility = null;

        // UI
        _characterUI.HideCharacterUI();

        // highlight
        _highlighter.ClearHighlightedTiles().GetAwaiter();
    }

    // TODO: this probably shouldn't be here 
    public void DrawPath()
    {
        ClearPathRenderer();

        // only draw path when character is selected
        if (SelectedCharacter == null)
            return;

        // don't draw path if ability is selected
        if (SelectedAbility != null)
            return;

        // get the tile movepoint is on
        if (!TileManager.Tiles.TryGetValue(_tilemap.WorldToCell(transform.position), out _tile))
            return;
        // don't draw path to tiles you can't reach
        if (!_tile.WithinRange)
            return;

        ABPath path = GetPathTo(transform);

        if (path.error)
            return;

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
        AstarPath.active.Scan();
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
}
