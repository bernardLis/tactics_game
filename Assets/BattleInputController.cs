using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class BattleInputController : MonoBehaviour
{
    // UI elements

    // input system
    PlayerInput playerInput;

    // tilemap
    Tilemap tilemap;
    WorldTile _tile;
    Dictionary<Vector3, WorldTile> tiles;


    // global utilities
    Camera cam;
    MovePointController movePointController;
    CharacterBattleController characterBattleController;
    BattleUI battleUI;

    public bool allowInput;

    public static BattleInputController instance;
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

    // Start is called before the first frame update
    void Start()
    {
        battleUI = BattleUI.instance;

        playerInput = GetComponent<PlayerInput>();

        tiles = GameTiles.instance.tiles;
        tilemap = (Tilemap)TileMapInstance.instance.GetComponent<Tilemap>();

        // TODO: Supposedly, this is an expensive call
        cam = Camera.main;
        movePointController = MovePointController.instance;
        characterBattleController = GetComponent<CharacterBattleController>();
        battleUI = BattleUI.instance;

        allowInput = true;
    }

    void OnEnable()
    {
        // inputs
        playerInput = GetComponent<PlayerInput>();

        playerInput.actions["LeftMouseClick"].performed += ctx => LeftMouseClick();
        playerInput.actions["ArrowMovement"].performed += ctx => Move(ctx.ReadValue<Vector2>());

        playerInput.actions["SelectClick"].performed += ctx => SelectClick();

        playerInput.actions["QButtonClick"].performed += ctx => QButtonClickInput();
        playerInput.actions["WButtonClick"].performed += ctx => WButtonClickInput();
        playerInput.actions["EButtonClick"].performed += ctx => EButtonClickInput();
        playerInput.actions["RButtonClick"].performed += ctx => RButtonClickInput();
        playerInput.actions["TButtonClick"].performed += ctx => TButtonClickInput();
        playerInput.actions["YButtonClick"].performed += ctx => YButtonClickInput();

        playerInput.actions["Back"].performed += ctx => BackClick();
    }

    void OnDisable()
    {
        if (playerInput == null)
            return;

        playerInput.actions["LeftMouseClick"].performed -= ctx => LeftMouseClick();
        playerInput.actions["ArrowMovement"].performed -= ctx => Move(ctx.ReadValue<Vector2>());

        playerInput.actions["SelectClick"].performed -= ctx => SelectClick();

        playerInput.actions["QButtonClick"].performed -= ctx => QButtonClickInput();
        playerInput.actions["WButtonClick"].performed -= ctx => WButtonClickInput();
        playerInput.actions["EButtonClick"].performed -= ctx => EButtonClickInput();
        playerInput.actions["RButtonClick"].performed -= ctx => RButtonClickInput();
        playerInput.actions["TButtonClick"].performed += ctx => TButtonClickInput();
        playerInput.actions["YButtonClick"].performed += ctx => YButtonClickInput();


        playerInput.actions["Back"].performed -= ctx => BackClick();
    }

    public bool isInputAllowed()
    {
        return allowInput;
    }

    void LeftMouseClick()
    {
        if (!allowInput || EventSystem.current.IsPointerOverGameObject())
            return;

        Vector3 mousePos = Mouse.current.position.ReadValue();
        mousePos.z = 1; // select distance = 1 unit(s) from the camera
        Vector3Int tilePos = tilemap.WorldToCell(cam.ScreenToWorldPoint(mousePos));
        if (!tiles.TryGetValue(tilePos, out _tile))
            return;

        Vector3 pos = new Vector3(_tile.LocalPlace.x + 0.5f, _tile.LocalPlace.y + 0.5f, _tile.LocalPlace.z);

        movePointController.Move(pos);
    }

    void Move(Vector2 direction)
    {
        if (!allowInput)
            return;

        // TODO: this is wrong, but it works.
        // with only normalize, if you press both arrows at the same time you will get (0.7, 0.7) vector        
        direction.Normalize();
        Vector2 vectorX = new Vector2(direction.x, 0).normalized;
        Vector2 vectorY = new Vector2 (0, direction.y).normalized;

        movePointController.Move(new Vector3(transform.position.x + vectorX.x, transform.position.y + vectorY.y, transform.position.z));
    }
    void SelectClick()
    {
        movePointController.HandleSelectClick();
    }

    void BackClick()
    {
        if (!allowInput)
            return;

        characterBattleController.Back();
    }

    // when you click Q on keyboard I want to simulate clicking a button with mouse
    void QButtonClickInput()
    {
        battleUI.SimulateQButtonClicked();
    }
    void WButtonClickInput()
    {
        battleUI.SimulateWButtonClicked();
    }
    void EButtonClickInput()
    {
        battleUI.SimulateEButtonClicked();
    }
    void RButtonClickInput()
    {
        battleUI.SimulateRButtonClicked();
    }
    void TButtonClickInput()
    {
        battleUI.SimulateTButtonClicked();
    }
    void YButtonClickInput()
    {
        battleUI.SimulateYButtonClicked();
    }




}
