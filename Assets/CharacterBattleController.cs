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
    MovePointController movePointController;
    BattleUI battleUI;

    // I will be caching them for selected character
    GameObject selectedCharacter;
    PlayerStats playerStats;
    PlayerCharSelection playerCharSelection;
    AILerp aILerp;
    AIDestinationSetter destinationSetter;
    CharacterRendererManager characterRendererManager;


    void Start()
    {
        tilemap = TileMapInstance.instance.GetComponent<Tilemap>();
        tiles = GameTiles.instance.tiles; // This is our Dictionary of tiles

        highlighter = Highlighter.instance;
        movePointController = MovePointController.instance;
        battleUI = BattleUI.instance;
    }

    void Update()
    {
        // TODO: find a better way to reset flag for movepoint after character reaches the destination
        if (aILerp != null && aILerp.reachedDestination)
            movePointController.blockMovePoint = false;
    }

    public void Select(Collider2D _col)
    {
        // get the tile movepoint is on
        if (tiles.TryGetValue(tilemap.WorldToCell(transform.position), out _tile))
            selectedTile = _tile;

        // when character is selected
        if (selectedCharacter != null)
        {
            // Move
            if (!playerCharSelection.hasMovedThisTurn && selectedTile.WithinRange)
                Move();

            // Interact with something when ability is selected && tile is within range

            return;
        }

        // Select character
        if (_col.transform.CompareTag("PlayerCollider"))
        {
            SelectCharacter(_col.transform.parent.gameObject);
            return;
        }

        Debug.Log("trying to select: " + _col);
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
        battleUI.ShowCharacterUI(playerStats.currentHealth, playerStats.currentMana, playerStats.character);
        
        // highlight
        highlighter.HiglightPlayerMovementRange(selectedCharacter.transform.position, playerStats.movementRange.GetValue(),
                                                new Color(0.53f, 0.52f, 1f, 1f));
    }

    void UnselectCharacter()
    {
        selectedCharacter = null;
        playerStats = null;
        playerCharSelection = null;

        highlighter.ClearHighlightedTiles();
    }

    void Move()
    {
        highlighter.ClearHighlightedTiles();
        playerCharSelection.hasMovedThisTurn = true;

        destinationSetter.target = transform;

        // disable movepoint
        movePointController.blockMovePoint = true;
    }

    void Back()
    {
        // well... maybe i need 2 backs for unselecting and moving back, but maybe i don't need for unselecting? ???!!@!@#
        // only one back is needed coz you can 'always' select abilities
        // back will be going to the starting character destination - held by player char selection
        // need to reset moved this turn

    }
}
