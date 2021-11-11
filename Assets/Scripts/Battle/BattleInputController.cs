using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class BattleInputController : MonoBehaviour
{
    // input system
    PlayerInput playerInput;

    // tilemap
    Tilemap tilemap;
    WorldTile _tile;
    Dictionary<Vector3, WorldTile> tiles;

    // global utilities
    Camera cam;
    BattleUI battleUI;

    // local
    MovePointController movePointController;
    CharacterBattleController characterBattleController;
    BattlePreparationController battlePreparationController;
    OscilateScale oscilateScale;

    [HideInInspector] public bool allowInput { get; private set; }

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

    void Start()
    {
        FindObjectOfType<TurnManager>().EnemyTurnEndEvent += OnEnemyTurnEnd;
        FindObjectOfType<TurnManager>().PlayerTurnEndEvent += OnPlayerTurnEnd;

        playerInput = GetComponent<PlayerInput>();

        tiles = GameTiles.instance.tiles;
        tilemap = TileMapInstance.instance.GetComponent<Tilemap>();

        // TODO: Supposedly, this is an expensive call
        cam = Camera.main;
        battleUI = BattleUI.instance;

        movePointController = MovePointController.instance;
        characterBattleController = GetComponent<CharacterBattleController>();
        battlePreparationController = GetComponent<BattlePreparationController>();
        oscilateScale = GetComponentInChildren<OscilateScale>();

        allowInput = true;
    }

    void OnEnable()
    {
        // inputs
        playerInput = GetComponent<PlayerInput>();

        // hacky way to make sure it is subscribed only once (TODO: does that wokr? XD)
        UnsubscribeInputActions();
        SubscribeInputActions();
    }

    void OnDisable()
    {
        if (playerInput == null)
            return;

        UnsubscribeInputActions();
    }

    void SubscribeInputActions()
    {
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

        playerInput.actions["CancelEverything"].performed += ctx => CancelEverything();


        // char placement specific for now
        playerInput.actions["SelectNextCharacter"].performed += ctx => SelectNextCharacter();
        playerInput.actions["SelectPreviousCharacter"].performed += ctx => SelectPreviousCharacter();
    }

    void UnsubscribeInputActions()
    {
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

        playerInput.actions["CancelEverything"].performed -= ctx => CancelEverything();


        // char placement specific for now
        playerInput.actions["SelectNextCharacter"].performed -= ctx => SelectNextCharacter();
        playerInput.actions["SelectPreviousCharacter"].performed -= ctx => SelectPreviousCharacter();
    }

    public bool IsInputAllowed()
    {
        return allowInput;
    }

    public void SetInputAllowed(bool isAllowed)
    {
        allowInput = isAllowed;
    }

    void LeftMouseClick()
    {
        if (!allowInput) // TODO: ||EventSystem.current.IsPointerOverGameObject() << throws an error;
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
        Vector2 vectorY = new Vector2(0, direction.y).normalized;

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

    void SelectNextCharacter()
    {
        if (TurnManager.battleState == BattleState.PREPARATION)
        {
            battlePreparationController.SelectNextCharacter();
            return;
        }

    }

    void SelectPreviousCharacter()
    {
        if (TurnManager.battleState == BattleState.PREPARATION)
        {
            battlePreparationController.SelectPreviousCharacter();
            return;
        }
    }

    void CancelEverything()
    {
        allowInput = true;
        characterBattleController.Back();
        characterBattleController.UnselectCharacter();
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

    void OnEnemyTurnEnd()
    {
        allowInput = true;
        oscilateScale.SetOscilation(true);
    }

    void OnPlayerTurnEnd()
    {
        allowInput = false;
        oscilateScale.SetOscilation(false);
    }
}
