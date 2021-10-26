using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;

public class MovePointController : MonoBehaviour
{

    Highlighter highlighter;
    BattlePreparationController battlePreparationController;

    AbilityUI abilityUI;

    public GameObject selected;

    PlayerInput playerInput;
    Camera cam;

    // tiles
    public Tilemap tilemap;
    WorldTile _tile;
    public Dictionary<Vector3, WorldTile> tiles;

    // my scripts
    PlayerCharSelection charSelection;
    PlayerCharMovementController playerCharMovementController;
    PlayerCharInteractionController playerCharInteractionController;

    EnemyCharSelection enemyCharSelection;

    public bool blockMovePoint = false;
    bool firstEnable = false;


    public static MovePointController instance;
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

        FindObjectOfType<TurnManager>().enemyTurnEndEvent += OnEnemyTurnEnd;
        FindObjectOfType<TurnManager>().playerTurnEndEvent += OnPlayerTurnEnd;

        highlighter = GameManager.instance.GetComponent<Highlighter>();
        abilityUI = GameUI.instance.GetComponent<AbilityUI>();

        // This is our Dictionary of tiles
        tiles = GameTiles.instance.tiles;
        tilemap = (Tilemap)TileMapInstance.instance.GetComponent<Tilemap>();

        // TODO: Supposedly, this is an expensive call
        cam = Camera.main;

        playerInput = GetComponent<PlayerInput>();
        battlePreparationController = GetComponent<BattlePreparationController>();
    }

    void Start()
    {
        UpdateTileInfoUI();
        UpdateCharacterInfoUI();
    }

    void OnEnable()
    {
        //controls.Enable();
        // inputs
        playerInput = GetComponent<PlayerInput>();

        playerInput.actions["LeftMouseClick"].performed += ctx => LeftMouseClick();

        playerInput.actions["ArrowUpClick"].performed += ctx => ArrowUpClick();
        playerInput.actions["ArrowDownClick"].performed += ctx => ArrowDownClick();
        playerInput.actions["ArrowLeftClick"].performed += ctx => ArrowLeftClick();
        playerInput.actions["ArrowRightClick"].performed += ctx => ArrowRightClick();
        playerInput.actions["SelectClick"].performed += ctx => SelectClick();

        playerInput.actions["Test"].performed += ctx => Test();
        playerInput.actions["TestY"].performed += ctx => TestY();

        // TODO: THIS SUCKS but document ui is not ready on the first enable.
        if (!firstEnable)
        {
            UpdateTileInfoUI();
            UpdateCharacterInfoUI();
        }
        firstEnable = true;
    }

    void OnDisable()
    {
        GameUI.instance.HideTileInfoUI();
        GameUI.instance.HideCharacterInfoUI();

        if (playerInput == null)
            return;

        playerInput.actions["LeftMouseClick"].performed -= ctx => LeftMouseClick();

        playerInput.actions["ArrowUpClick"].performed -= ctx => ArrowUpClick();
        playerInput.actions["ArrowDownClick"].performed -= ctx => ArrowDownClick();
        playerInput.actions["ArrowLeftClick"].performed -= ctx => ArrowLeftClick();
        playerInput.actions["ArrowRightClick"].performed -= ctx => ArrowRightClick();
        playerInput.actions["SelectClick"].performed -= ctx => SelectClick();

        playerInput.actions["Test"].performed -= ctx => Test();
        playerInput.actions["TestY"].performed -= ctx => TestY();
    }

    // INPUT
    void Test()
    {
        Debug.Log("T is clicked ");
    }

    void TestY()
    {
        Debug.Log("Y is clicked");
    }

    // TODO: character being placed

    void LeftMouseClick()
    {
        if (!blockMovePoint)
        {
            Vector3 mousePos = Mouse.current.position.ReadValue(); // TODO: this is wrong, right? input is a different system
            mousePos.z = 1; // select distance = 1 unit(s) from the camera
            Vector3Int tilePos = tilemap.WorldToCell(cam.ScreenToWorldPoint(mousePos));
            if (tiles.TryGetValue(tilePos, out _tile))
            {
                transform.position = new Vector3(_tile.LocalPlace.x + 0.5f, _tile.LocalPlace.y + 0.5f, _tile.LocalPlace.z);

                ClearEnemyHighlight();

                UpdateTileInfoUI();
                UpdateCharacterInfoUI();
                UpdateAbilityUI();

                // character being placed
                if (battlePreparationController.characterBeingPlaced != null)
                    battlePreparationController.UpdateCharacterBeingPlacedPosition();
            }
        }
    }

    void ArrowUpClick()
    {
        if (!blockMovePoint)
            Move(Vector3.up);
    }
    void ArrowDownClick()
    {
        if (!blockMovePoint)
            Move(Vector3.down);
    }
    void ArrowLeftClick()
    {
        if (!blockMovePoint)
            Move(Vector3.left);
    }
    void ArrowRightClick()
    {
        if (!blockMovePoint)
            Move(Vector3.right);
    }

    void Move(Vector3 movePos)
    {
        ClearEnemyHighlight();
        transform.position += movePos;

        UpdateTileInfoUI();
        UpdateCharacterInfoUI();
        UpdateAbilityUI();

        // TODO: character being placed
        if (battlePreparationController.characterBeingPlaced != null)
            battlePreparationController.UpdateCharacterBeingPlacedPosition();
    }

    void ClearEnemyHighlight()
    {
        // clear enemy highlight if needed
        if (enemyCharSelection != null)
        {
            highlighter.ClearHighlightedTiles();
            enemyCharSelection = null;
        }
    }

    void SelectClick()
    {
        // check if there is a selectable object at selector's position
        // only one selectable object can occypy space: 
        Collider2D col = Physics2D.OverlapCircle(transform.position, 0.2f);
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 0.2f);

        // For placing characters during prep
        if (TurnManager.battleState == BattleState.PREPARATION && battlePreparationController.characterBeingPlaced != null)
        {
            Vector3Int tilePos = tilemap.WorldToCell(transform.position);
            if (tiles.TryGetValue(tilePos, out _tile))
            {
                if (!_tile.WithinRange)
                    return;
            }
            battlePreparationController.PlaceCharacter();
            return;
        }


        // select controlled by player object
        if (selected == null && col == null)
        {
            Debug.Log("Selecting nothingness");
        }
        else if (selected == null && !col.transform.CompareTag("PlayerCollider"))
        {
            Debug.Log("Selecting something you can't control!");
        }
        else if (selected == null && col.transform.CompareTag("PlayerCollider"))
        {
            Select(col.transform.parent.gameObject);
        }
        // move if the space is empty
        else if (playerCharMovementController.enabled && col == null)
        {
            // check if target is within range
            // TODO: is this a correct place and way to check? 
            Vector3Int tilePos = tilemap.WorldToCell(transform.position);
            if (tiles.TryGetValue(tilePos, out _tile))
            {
                if (!_tile.WithinRange)
                {
                    Debug.Log("Not within range");
                    return;
                }
            }

            playerCharMovementController.Move(transform);
        }
        // interact
        else if (playerCharInteractionController.enabled)
        {
            // check if target is within range
            // TODO: is this a correct place and way to check? 
            Vector3Int tilePos = tilemap.WorldToCell(transform.position);
            if (tiles.TryGetValue(tilePos, out _tile))
            {
                if (!_tile.WithinRange)
                {
                    Debug.Log("Not within range");
                    return;
                }
            }

            if (col != null)
            {
                // character colliders are children
                if (col.transform.CompareTag("PlayerCollider") || col.transform.CompareTag("EnemyCollider"))
                    playerCharInteractionController.selectedAbility.TriggerAbility(col.transform.parent.gameObject);
                // environment objects colliders are on the object itself 
                else
                    playerCharInteractionController.selectedAbility.TriggerAbility(col.gameObject);

                // hide ui
                abilityUI.HideAbilityUI();
            }
            else
            {
                // there is no one (nothing) to interact with, it will be cool if I could create stuff on the map.
                Debug.Log("nothing to interact with");
            }
        }
        // if selected player's location "move" there;
        else if (selected == col.transform.parent.gameObject && col.transform.CompareTag("PlayerCollider"))
            playerCharMovementController.Move(transform);
        else
            Debug.Log("What am I selecting?");
    }

    public void UpdateTileInfoUI()
    {
        // tile info
        Vector3Int tilePos = tilemap.WorldToCell(transform.position);
        string tileUIText = "";

        if (tiles.TryGetValue(tilePos, out _tile))
        {
            if (_tile.IsObstacle)
                tileUIText = "Obstacle. ";
        }

        // check if there is a character standing there
        Collider2D col = Physics2D.OverlapCircle(transform.position, 0.2f);

        // return if there is no object on the tile
        if (col != null)
        {
            if (col.transform.CompareTag("Obstacle") || col.transform.CompareTag("Trap"))
            {
                UIText textScript = col.transform.GetComponent<UIText>();
                if (textScript != null)
                    tileUIText = tileUIText + textScript.displayText;
            }
        }

        // hide/show the whole panel
        if (tileUIText == "")
        {
            GameUI.instance.HideTileInfoUI();
        }
        else
        {
            GameUI.instance.UpdateTileInfoUI(tileUIText);
            GameUI.instance.ShowTileInfoUI();
        }
    }

    void UpdateCharacterInfoUI()
    {
        // check if there is a character standing there
        Collider2D col = Physics2D.OverlapCircle(transform.position, 0.2f);

        // return if there is no object on the tile
        if (col == null)
        {
            GameUI.instance.HideCharacterInfoUI();
            return;
        }

        // highlight how far can enemy character move
        if (col.transform.CompareTag("EnemyCollider")
            && (playerCharInteractionController == null || playerCharInteractionController.enabled == false)
            && (playerCharMovementController == null || playerCharMovementController.enabled == false))
        {
            enemyCharSelection = col.transform.parent.gameObject.GetComponent<EnemyCharSelection>();
            enemyCharSelection.HiglightMovementRange();
        }

        // display stats of characters
        if (col.transform.CompareTag("PlayerCollider") || col.transform.CompareTag("EnemyCollider"))
        {
            CharacterStats stats = col.transform.parent.GetComponent<CharacterStats>();

            GameUI.instance.UpdateCharacterInfoUI(stats.currentHealth, stats.maxHealth.GetValue(),
            stats.currentMana, stats.maxMana.GetValue());

            GameUI.instance.ShowCharacterInfoUI();
        }
        else
        {
            GameUI.instance.HideCharacterInfoUI();
        }
    }
    void UpdateAbilityUI()
    {
        if (playerCharInteractionController != null)
        {
            if (playerCharInteractionController.enabled && playerCharInteractionController.selectedAbility != null)
            {
                abilityUI.ShowAbilityUI();
                string name = playerCharInteractionController.selectedAbility.aName;
                string result = "";

                // check it's within range;
                Vector3Int tilePos = tilemap.WorldToCell(transform.position);
                if (tiles.TryGetValue(tilePos, out _tile))
                {
                    if (!_tile.WithinRange)
                    {
                        abilityUI.UpdateAbilityUI(name, result);
                        return;
                    }
                }

                Collider2D col = Physics2D.OverlapCircle(transform.position, 0.2f);
                if (col != null)
                {
                    // TODO: if it is attack/heal it has to be character
                    if (col.transform.CompareTag("PlayerCollider") || col.transform.CompareTag("EnemyCollider"))
                    {
                        CharacterStats myStats = selected.GetComponent<CharacterStats>();
                        CharacterStats stats = col.transform.parent.GetComponent<CharacterStats>();

                        if (playerCharInteractionController.selectedAbility.aType == "attack")
                            result = "health: -" + (playerCharInteractionController.selectedAbility.value + myStats.strength.GetValue() - stats.armor.GetValue());
                        else if (playerCharInteractionController.selectedAbility.aType == "heal")
                            result = "health: +" + (playerCharInteractionController.selectedAbility.value + myStats.intelligence.GetValue());
                    }
                    // TODO: if it is a move ability is can also be boulder
                }

                abilityUI.UpdateAbilityUI(name, result);

            }
            else
            {
                abilityUI.HideAbilityUI();
            }
        }
    }

    void Select(GameObject obj)
    {
        charSelection = obj.GetComponent<PlayerCharSelection>();
        if (!charSelection.movedThisTurn)
        {
            selected = obj;
            charSelection.SelectCharacter();

            playerCharMovementController = selected.GetComponent<PlayerCharMovementController>();
            playerCharMovementController.enabled = true;

            playerCharInteractionController = selected.GetComponent<PlayerCharInteractionController>();
        }
        else
        {
            Debug.Log("Character moved this turn already");
        }
    }

    public void UnselectSelected()
    {
        // incase we end the turn with highlighted tiles;
        highlighter.ClearHighlightedTiles();

        // reset controllers
        if (playerCharMovementController != null)
        {
            playerCharMovementController.enabled = false;
            playerCharMovementController = null;
        }
        if (playerCharInteractionController != null)
        {
            playerCharInteractionController.enabled = false;
            playerCharInteractionController = null;
        }
        if (charSelection != null)
        {
            charSelection.UnselectCharacter();
            charSelection = null;
        }

        blockMovePoint = false;
        selected = null;
    }

    void OnEnemyTurnEnd()
    {
        blockMovePoint = false;

        GameObject[] playerChars = GameObject.FindGameObjectsWithTag("PlayerCollider");
        if (playerChars.Length > 0)
            transform.position = playerChars[0].transform.position;

        gameObject.SetActive(true);

        // camera follows the movepoint again
        BasicCameraFollow.instance.followTarget = transform;

        UpdateTileInfoUI();
    }

    void OnPlayerTurnEnd()
    {
        UnselectSelected();
        gameObject.SetActive(false);
    }

}

