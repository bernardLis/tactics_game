using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class MovePointController : MonoBehaviour
{
    GameUI gameUI;
    BattlePreparationController battlePreparationController;
    CharacterBattleController characterBattleController;

    AbilityUI abilityUI;

    public GameObject selected;

    //PlayerInput playerInput;
    Camera cam;

    // tiles
    Tilemap tilemap;
    WorldTile _tile;
    Dictionary<Vector3, WorldTile> tiles;

    // my scripts
    PlayerCharSelection charSelection;
    //PlayerCharMovementController playerCharMovementController;
    PlayerCharInteractionController playerCharInteractionController;

    EnemyCharSelection enemyCharSelection;

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

        gameUI = GameUI.instance;

        abilityUI = gameUI.GetComponent<AbilityUI>();

        // This is our Dictionary of tiles
        tiles = GameTiles.instance.tiles;
        tilemap = (Tilemap)TileMapInstance.instance.GetComponent<Tilemap>();

        // TODO: Supposedly, this is an expensive call
        cam = Camera.main;

        //playerInput = GetComponent<PlayerInput>();
        battlePreparationController = GetComponent<BattlePreparationController>();
        characterBattleController = GetComponent<CharacterBattleController>();
    }

    void Start()
    {
        UpdateTileInfoUI();
        UpdateCharacterInfoUI();
    }

    void OnEnable()
    {
        /*
        // inputs
        playerInput = GetComponent<PlayerInput>();

        playerInput.actions["LeftMouseClick"].performed += ctx => LeftMouseClick();

        playerInput.actions["ArrowUpClick"].performed += ctx => ArrowUpClick();
        playerInput.actions["ArrowDownClick"].performed += ctx => ArrowDownClick();
        playerInput.actions["ArrowLeftClick"].performed += ctx => ArrowLeftClick();
        playerInput.actions["ArrowRightClick"].performed += ctx => ArrowRightClick();
        playerInput.actions["SelectClick"].performed += ctx => SelectClick();

        playerInput.actions["QButtonClick"].performed += ctx => QButtonClickInput();
        playerInput.actions["WButtonClick"].performed += ctx => WButtonClickInput();
        playerInput.actions["EButtonClick"].performed += ctx => EButtonClickInput();
        playerInput.actions["RButtonClick"].performed += ctx => RButtonClickInput();

        playerInput.actions["Back"].performed += ctx => BackClick();

        playerInput.actions["Test"].performed += ctx => Test();
        playerInput.actions["TestY"].performed += ctx => TestY();
        */

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
        gameUI.HideTileInfoUI();
        //gameUI.HideCharacterInfoUI();
        /*
        if (playerInput == null)
            return;

        playerInput.actions["LeftMouseClick"].performed -= ctx => LeftMouseClick();

        playerInput.actions["ArrowUpClick"].performed -= ctx => ArrowUpClick();
        playerInput.actions["ArrowDownClick"].performed -= ctx => ArrowDownClick();
        playerInput.actions["ArrowLeftClick"].performed -= ctx => ArrowLeftClick();
        playerInput.actions["ArrowRightClick"].performed -= ctx => ArrowRightClick();
        playerInput.actions["SelectClick"].performed -= ctx => SelectClick();

        playerInput.actions["QButtonClick"].performed -= ctx => QButtonClickInput();
        playerInput.actions["WButtonClick"].performed -= ctx => WButtonClickInput();
        playerInput.actions["EButtonClick"].performed -= ctx => EButtonClickInput();
        playerInput.actions["RButtonClick"].performed -= ctx => RButtonClickInput();

        playerInput.actions["Back"].performed -= ctx => BackClick();

        playerInput.actions["Test"].performed -= ctx => Test();
        playerInput.actions["TestY"].performed -= ctx => TestY();
        */
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

        Vector3 mousePos = Mouse.current.position.ReadValue();
        mousePos.z = 1; // select distance = 1 unit(s) from the camera
        Vector3Int tilePos = tilemap.WorldToCell(cam.ScreenToWorldPoint(mousePos));
        if (!tiles.TryGetValue(tilePos, out _tile))
            return;

        transform.position = new Vector3(_tile.LocalPlace.x + 0.5f, _tile.LocalPlace.y + 0.5f, _tile.LocalPlace.z);

        UpdateTileInfoUI();
        UpdateCharacterInfoUI();
        UpdateAbilityUI();

        // character being placed
        if (battlePreparationController.characterBeingPlaced != null)
            battlePreparationController.UpdateCharacterBeingPlacedPosition();
    }


    public void Move(Vector3 pos)
    {

        // block moving out form tile map
        Vector3Int tilePos = tilemap.WorldToCell(pos);
        if (!tiles.TryGetValue(tilePos, out _tile))
            return;

        transform.position = pos;

        UpdateTileInfoUI();
        UpdateCharacterInfoUI();
        UpdateAbilityUI();

        // TODO: character being placed
        if (battlePreparationController.characterBeingPlaced != null)
            battlePreparationController.UpdateCharacterBeingPlacedPosition();
    }

    public void HandleSelectClick()
    {
        // check if there is a selectable object at selector's position
        // only one selectable object can occypy space: 
        Collider2D col = Physics2D.OverlapCircle(transform.position, 0.2f);

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

        Select(col);
        return;
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
            gameUI.HideTileInfoUI();
        }
        else
        {
            gameUI.UpdateTileInfoUI(tileUIText);
            gameUI.ShowTileInfoUI();
        }
    }

    void UpdateCharacterInfoUI()
    {
        // check if there is a character standing there
        Collider2D col = Physics2D.OverlapCircle(transform.position, 0.2f);

        // return if there is no object on the tile
        if (col == null)
        {

            //gameUI.HideCharacterInfoUI();
            return;
        }
        /*
        // highlight how far can enemy character move
        if (col.transform.CompareTag("EnemyCollider")
            && (playerCharInteractionController == null || playerCharInteractionController.enabled == false))
        //&& (playerCharMovementController == null || playerCharMovementController.enabled == false))
        {
            enemyCharSelection = col.transform.parent.gameObject.GetComponent<EnemyCharSelection>();
            enemyCharSelection.HiglightMovementRange();
        }
        */

        // display stats of characters
        if (col.transform.CompareTag("PlayerCollider") || col.transform.CompareTag("EnemyCollider"))
        {
            CharacterStats stats = col.transform.parent.GetComponent<CharacterStats>();

            //gameUI.UpdateCharacterInfoUI(stats.currentHealth, stats.maxHealth.GetValue(),
            //stats.currentMana, stats.maxMana.GetValue());

            //gameUI.ShowCharacterInfoUI();
        }
        else
        {
            //gameUI.HideCharacterInfoUI();
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

    void Select(Collider2D obj)
    {
        characterBattleController.Select(obj);
    }

    void OnEnemyTurnEnd()
    {
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
        //UnselectSelected();
        gameObject.SetActive(false);
    }

}

