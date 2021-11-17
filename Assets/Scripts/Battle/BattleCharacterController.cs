using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Pathfinding;

public class BattleCharacterController : MonoBehaviour
{
    // tilemap
    Tilemap tilemap;
    WorldTile _tile;
    Dictionary<Vector3, WorldTile> tiles;
    WorldTile selectedTile;

    // global utilities
    Highlighter highlighter;
    BattleUI battleUI;
    BattleInputController battleInputController;

    // I will be caching them for selected character
    GameObject selectedCharacter;
    PlayerStats playerStats;
    PlayerCharSelection playerCharSelection;
    AILerp aILerp;
    AIDestinationSetter destinationSetter;
    CharacterRendererManager characterRendererManager;

    // selection
    bool isSelectionBlocked; // block mashing select character coz it breaks the highlight - if you quickly switch between selection of 2 chars.

    // movement
    GameObject tempObject;
    bool hasCharacterStartedMoving;
    bool hasCharacterGoneBack;

    Seeker seeker;
    LineRenderer pathRenderer;

    // interactions
    Ability selectedAbility;

    public static BattleCharacterController instance;
    void Awake()
    {
        #region singleton

        // singleton
        if (instance != null)
        {
            Debug.LogWarning("More than one instance of TurnManager found");
            return;
        }
        instance = this;
        #endregion
    }

    void Start()
    {
        tilemap = TileMapInstance.instance.GetComponent<Tilemap>();
        tiles = GameTiles.instance.tiles; // This is our Dictionary of tiles

        highlighter = Highlighter.instance;
        battleInputController = BattleInputController.instance;
        battleUI = BattleUI.instance;

        seeker = GetComponent<Seeker>();
        pathRenderer = GetComponent<LineRenderer>();
    }

    void Update()
    {
        // TODO: is there a better way? 
        if (selectedCharacter == null)
            return;

        if (hasCharacterStartedMoving && tempObject != null
            && Vector3.Distance(selectedCharacter.transform.position, tempObject.transform.position) <= 0.1f)
            CharacterReachedDestination();
    }

    public void Select(Collider2D _col)
    {
        ClearPathRenderer();

        // get the tile movepoint is on
        if (tiles.TryGetValue(tilemap.WorldToCell(transform.position), out _tile))
            selectedTile = _tile;

        // select character
        if (_col != null && CanSelectCharacter(_col))
        {
            SelectCharacter(_col.transform.parent.gameObject);
            return;
        }

        if (selectedCharacter == null)
            return;

        // when character is selected

        // Move
        if (CanMoveCharacter() && selectedTile.WithinRange)
        {
            Move();
            return;
        }

        // Interact with something when ability is selected && tile is within range
        if (_col != null && !playerCharSelection.hasFinishedTurn && selectedTile.WithinRange)
        {
            Interact(_col);
            return;
        }

        // trigger defend even if there is no collider TODO: dunno how to manage that...
        if (selectedAbility.aType == AbilityType.DEFEND)
        {
            Interact(null);
            return;
        }
    }

    bool CanSelectCharacter(Collider2D _col)
    {
        if (isSelectionBlocked)
            return false;
        if (_col == null)
            return false;
        // can select only player characters
        if (!_col.transform.CompareTag("PlayerCollider"))
            return false;
        // character allows selection
        if (!_col.GetComponentInParent<PlayerCharSelection>().CanBeSelected())
            return false;
        // don't allow to select another character if you have moved this character and did not take the action
        if (selectedCharacter != null && playerCharSelection.hasMovedThisTurn && !playerCharSelection.hasFinishedTurn)
            return false;
        // don't allow to select another character if you are triggering ability;
        if (selectedAbility != null)
            return false;

        return true;
    }

    bool CanMoveCharacter()
    {
        if (playerCharSelection.hasMovedThisTurn)
            return false;
        if (playerCharSelection.hasFinishedTurn)
            return false;
        if (selectedAbility != null)
            return false;

        return true;
    }

    async void SelectCharacter(GameObject _character)
    {
        isSelectionBlocked = true;

        if (_character.GetComponent<PlayerCharSelection>().hasMovedThisTurn)
        {
            Debug.Log("Character has moved this turn already");
            return;
        }

        // character specific ui
        if (playerCharSelection != null)
            playerCharSelection.DeselectCharacter();

        // cache
        selectedCharacter = _character;
        playerStats = selectedCharacter.GetComponent<PlayerStats>();
        playerCharSelection = selectedCharacter.GetComponent<PlayerCharSelection>();
        aILerp = selectedCharacter.GetComponent<AILerp>();

        //seeker = selectedCharacter.GetComponent<Seeker>();
        destinationSetter = selectedCharacter.GetComponent<AIDestinationSetter>();
        characterRendererManager = selectedCharacter.GetComponentInChildren<CharacterRendererManager>();

        // character specific ui
        playerCharSelection.SelectCharacter();

        // UI
        battleUI.ShowCharacterUI(playerStats);

        // highlight
        await highlighter.HiglightPlayerMovementRange(selectedCharacter.transform.position, playerStats.movementRange.GetValue(),
                                                new Color(0.53f, 0.52f, 1f, 1f));
        isSelectionBlocked = false;
    }

    void Move()
    {
        hasCharacterStartedMoving = true;

        // TODO: should I make it all async?
#pragma warning disable CS4014
        highlighter.ClearHighlightedTiles();

        tempObject = new GameObject("Destination");
        tempObject.transform.position = transform.position;
        destinationSetter.target = tempObject.transform;

        battleUI.DisableSkillButtons();

        playerCharSelection.SetCharacterMoved(true);
    }

    public void Back()
    {
        ClearPathRenderer();

        // if back is called during character movement return
        if (hasCharacterStartedMoving)
            return;

        if (selectedCharacter == null)
            return;

        if (selectedAbility != null)
        {
            BackFromAbilitySelection();
            return;
        }

        if (!playerCharSelection.hasMovedThisTurn)
        {
            UnselectCharacter();
            return;
        }

        // flag reset
        playerCharSelection.SetCharacterMoved(false);

        hasCharacterGoneBack = true;
        hasCharacterStartedMoving = true;

        // move character to character's starting position quickly.
        aILerp.speed = 15;

        transform.position = playerCharSelection.positionTurnStart;

        tempObject = new GameObject("Back Destination");
        tempObject.transform.position = playerCharSelection.positionTurnStart;
        destinationSetter.target = tempObject.transform;

        battleUI.DisableSkillButtons();
    }

    void CharacterReachedDestination()
    {
        battleUI.EnableSkillButtons();

        if (tempObject != null)
            Destroy(tempObject);

        // reset flag
        hasCharacterStartedMoving = false;

        // check if it was back or normal move
        if (!hasCharacterGoneBack)
            return;

        // highlight movement range if character was going back
        hasCharacterGoneBack = false;
        highlighter.HiglightPlayerMovementRange(selectedCharacter.transform.position, playerStats.movementRange.GetValue(),
                                    new Color(0.53f, 0.52f, 1f, 1f));
    }

    public bool CanInteract()
    {
        if (!battleInputController.IsInputAllowed())
            return false;
        if (selectedCharacter == null)
            return false;
        if (playerCharSelection.hasFinishedTurn)
            return false;
        if (hasCharacterStartedMoving) // don't allow button click when character is on the move;
            return false;
        return true;
    }

    public void SetSelectedAbility(Ability ability)
    {
        ClearPathRenderer();
        selectedAbility = ability;
    }

    async void Interact(Collider2D col)
    {
        // defend ability // TODO: await in if ... hmmmmmmm....
        if (col == null && selectedAbility.aType == AbilityType.DEFEND && await selectedAbility.TriggerAbility(null))
        {
            FinishCharacterTurn();
            return;
        }

        // returns true if ability was triggered successfuly. // TODO: await in if ... hmmmmmmm....
        if (!await selectedAbility.TriggerAbility(col.transform.parent.gameObject))
            return;

        FinishCharacterTurn();
    }

    void BackFromAbilitySelection()
    {
        selectedAbility = null;
        battleUI.HideAbilityTooltip();
        highlighter.ClearHighlightedTiles();
    }

    void FinishCharacterTurn()
    {
        // set flags in player char selection
        playerCharSelection.FinishCharacterTurn();

        // clearing the cache here
        UnselectCharacter();

        // finish character's turn after the interaction is performed
        TurnManager.instance.PlayerCharacterTurnFinished();
    }

    public void UnselectCharacter()
    {
        // character specific ui
        if (playerCharSelection != null)
            playerCharSelection.DeselectCharacter();

        selectedCharacter = null;
        playerStats = null;
        playerCharSelection = null;
        selectedAbility = null;

        // UI
        battleUI.HideCharacterUI();

        // highlight
        highlighter.ClearHighlightedTiles();
    }

    // TODO: this probably shouldn't be here 
    public void DrawPath()
    {
        ClearPathRenderer();

        // only draw path when character is selected
        if (selectedCharacter == null)
            return;

        // don't draw path if ability is selected
        if (selectedAbility != null)
            return;

        // get the tile movepoint is on
        if (!tiles.TryGetValue(tilemap.WorldToCell(transform.position), out _tile))
            return;
        // don't draw path to tiles you can't reach
        if (!_tile.WithinRange)
            return;

        // https://arongranberg.com/astar/docs_dev/calling-pathfinding.php
        var path = seeker.StartPath(selectedCharacter.transform.position, transform.position, OnPathComplete);
    }

    void OnPathComplete(Path p)
    {
        if (p.error)
            return;

        // draw path with line renderer
        for (int i = 0; i < p.vectorPath.Count; i++)
        {
            pathRenderer.positionCount = p.vectorPath.Count;
            pathRenderer.SetPosition(i, new Vector3(p.vectorPath[i].x, p.vectorPath[i].y, -1f));
        }
    }

    void ClearPathRenderer()
    {
        pathRenderer.positionCount = 0;
    }
}
