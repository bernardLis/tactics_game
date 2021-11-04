using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Pathfinding;

public class CharacterBattleController : MonoBehaviour
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

    // interactions
    Ability selectedAbility;

    public static CharacterBattleController instance;
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
    }

    void Update()
    {
        if (aILerp != null && !aILerp.isMoving)
            battleInputController.allowInput = true;
    }

    public void Select(Collider2D _col)
    {
        Debug.Log("trying to select: " + _col);

        // get the tile movepoint is on
        if (tiles.TryGetValue(tilemap.WorldToCell(transform.position), out _tile))
            selectedTile = _tile;

        // select character
        // TODO: this is quite convoluted...
        if (_col != null && _col.transform.CompareTag("PlayerCollider")
        && !(selectedCharacter != null && playerCharSelection.hasMovedThisTurn && !playerCharSelection.hasFinishedTurn) // don't allow to select another character if you have moved this character and did not take the action
        && selectedAbility == null // don't allow to select another character if you are triggering ability;
        && _col.GetComponentInParent<PlayerCharSelection>().CanBeSelected()) // and character allows selection
        {        
            SelectCharacter(_col.transform.parent.gameObject);
            return;
        }

        if (selectedCharacter == null)
            return;

        // when character is selected
        // Move
        if ((!playerCharSelection.hasMovedThisTurn && !playerCharSelection.hasFinishedTurn) 
            && selectedTile.WithinRange 
            && selectedAbility == null)
            Move();

        // Interact with something when ability is selected && tile is within range
        if (_col != null && !playerCharSelection.hasFinishedTurn && selectedTile.WithinRange)
            Interact(_col);
    }

    void SelectCharacter(GameObject _character)
    {
        if (_character.GetComponent<PlayerCharSelection>().hasMovedThisTurn)
        {
            Debug.Log("Character has moved this turn already");
            return;
        }

        // cache
        selectedCharacter = _character;
        playerStats = selectedCharacter.GetComponent<PlayerStats>();
        playerCharSelection = selectedCharacter.GetComponent<PlayerCharSelection>();
        aILerp = selectedCharacter.GetComponent<AILerp>();
        destinationSetter = selectedCharacter.GetComponent<AIDestinationSetter>();
        characterRendererManager = selectedCharacter.GetComponentInChildren<CharacterRendererManager>();

        // UI
        battleUI.ShowCharacterUI(playerStats);

        // highlight
        highlighter.HiglightPlayerMovementRange(selectedCharacter.transform.position, playerStats.movementRange.GetValue(),
                                                new Color(0.53f, 0.52f, 1f, 1f));
    }

    void UnselectCharacter()
    {
        selectedCharacter = null;
        playerStats = null;
        playerCharSelection = null;

        selectedAbility = null;

        // UI
        battleUI.HideCharacterUI();

        // highlight
        highlighter.ClearHighlightedTiles();
    }

    void Move()
    {
        highlighter.ClearHighlightedTiles();

        destinationSetter.target = transform;
        playerCharSelection.hasMovedThisTurn = true;

        // disable input
        battleInputController.allowInput = false;
    }

    public bool canInteract()
    {
        if (!battleInputController.isInputAllowed())
            return false;
        if (selectedCharacter == null)
            return false;
        if (playerCharSelection.hasFinishedTurn)
            return false;

        return true;
    }

    public void SetSelectedAbility(Ability ability)
    {
        selectedAbility = ability;
    }

    void Interact(Collider2D col)
    {
        Debug.Log("interact " + col.gameObject);
        // returns true if ability was triggered successfuly.
        if (!selectedAbility.TriggerAbility(col.transform.parent.gameObject))
            return;

        FinishCharacterTurn();
    }

    public void Back()
    {
        if (selectedCharacter == null)
            return;

        if (selectedAbility != null)
        {
            BackFromAbilitySelection();
            return;
        }

        // flag reset
        playerCharSelection.hasMovedThisTurn = false;

        // move move point and character to character's starting position quickly.
        aILerp.speed = 15;
        transform.position = playerCharSelection.positionTurnStart;
        destinationSetter.target = transform;

        UnselectCharacter();
    }

    void BackFromAbilitySelection()
    {
        selectedAbility = null;
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


}
