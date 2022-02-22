using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.InputSystem;

public class BattleInputController : MonoBehaviour
{
    // input system
    PlayerInput playerInput;

    // tilemap
    Tilemap tilemap;
    WorldTile _tile;

    // global utilities
    Camera cam;
    CharacterUI characterUI;

    // local
    MovePointController movePointController;
    BattleCharacterController battleCharacterController;
    BattleDeploymentController battleDeploymentController;
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
        TurnManager.OnBattleStateChanged += TurnManager_OnBattleStateChanged;

        playerInput = GetComponent<PlayerInput>();

        tilemap = BattleManager.instance.GetComponent<TileManager>().tilemap;

        // TODO: Supposedly, this is an expensive call
        cam = Camera.main;
        characterUI = CharacterUI.instance;

        movePointController = MovePointController.instance;
        battleCharacterController = GetComponent<BattleCharacterController>();
        battleDeploymentController = GetComponent<BattleDeploymentController>();
        oscilateScale = GetComponentInChildren<OscilateScale>();
    }

    void OnEnable()
    {
        // inputs
        playerInput = GetComponent<PlayerInput>();

        SubscribeInputActions();
    }

    void OnDisable()
    {
        if (playerInput == null)
            return;

        UnsubscribeInputActions();
    }

    void TurnManager_OnBattleStateChanged(BattleState _state)
    {
        if (_state == BattleState.Deployment)
            HandleDeployment();

        if (_state == BattleState.PlayerTurn)
            HandlePlayerTurn();

        if (_state == BattleState.EnemyTurn)
            HandleEnemyTurn();
    }

    void OnDestroy()
    {
        TurnManager.OnBattleStateChanged -= TurnManager_OnBattleStateChanged;
    }

    void HandleDeployment()
    {
        allowInput = true;
    }

    void HandlePlayerTurn()
    {
        SetInputAllowed(true);
        oscilateScale.SetOscilation(true);
    }

    void HandleEnemyTurn()
    {
        SetInputAllowed(false);
        oscilateScale.SetOscilation(false);
    }

    void SubscribeInputActions()
    {
        // char placement specific for now
        playerInput.actions["SelectNextCharacter"].performed += SelectNextCharacter;
        playerInput.actions["SelectPreviousCharacter"].performed += SelectPreviousCharacter;

        playerInput.actions["LeftMouseClick"].performed += LeftMouseClick;
        playerInput.actions["ArrowMovement"].performed += Move;

        playerInput.actions["SelectClick"].performed += SelectClick;

        playerInput.actions["AButtonClick"].performed += AButtonClickInput;
        playerInput.actions["SButtonClick"].performed += SButtonClickInput;
        playerInput.actions["DButtonClick"].performed += DButtonClickInput;

        playerInput.actions["QButtonClick"].performed += QButtonClickInput;
        playerInput.actions["WButtonClick"].performed += WButtonClickInput;
        playerInput.actions["EButtonClick"].performed += EButtonClickInput;
        playerInput.actions["RButtonClick"].performed += RButtonClickInput;

        playerInput.actions["Back"].performed += BackClick;

        playerInput.actions["CancelEverything"].performed += CancelEverything;
    }

    void UnsubscribeInputActions()
    {
        // char placement specific for now
        playerInput.actions["SelectNextCharacter"].performed -= SelectNextCharacter;
        playerInput.actions["SelectPreviousCharacter"].performed -= SelectPreviousCharacter;

        playerInput.actions["LeftMouseClick"].performed -= LeftMouseClick;
        playerInput.actions["ArrowMovement"].performed -= Move;

        playerInput.actions["SelectClick"].performed -= SelectClick;

        playerInput.actions["AButtonClick"].performed -= AButtonClickInput;
        playerInput.actions["SButtonClick"].performed -= SButtonClickInput;
        playerInput.actions["DButtonClick"].performed -= DButtonClickInput;

        playerInput.actions["QButtonClick"].performed -= QButtonClickInput;
        playerInput.actions["WButtonClick"].performed -= WButtonClickInput;
        playerInput.actions["EButtonClick"].performed -= EButtonClickInput;
        playerInput.actions["RButtonClick"].performed -= RButtonClickInput;

        playerInput.actions["Back"].performed -= BackClick;

        playerInput.actions["CancelEverything"].performed -= CancelEverything;
    }

    public bool IsInputAllowed()
    {
        return allowInput;
    }

    public void SetInputAllowed(bool _isAllowed)
    {
        if (_isAllowed)
            characterUI.EnableSkillButtons();
        if (!_isAllowed)
            characterUI.DisableSkillButtons();

        allowInput = _isAllowed;
    }

    void LeftMouseClick(InputAction.CallbackContext _ctx)
    {
        if (battleCharacterController.characterState == CharacterState.SelectingFaceDir)
            return;

        if (!allowInput) // TODO: ||EventSystem.current.IsPointerOverGameObject() << throws an error;
            return;

        Vector3 mousePos = Mouse.current.position.ReadValue();
        mousePos.z = 0; // select distance = 1 unit(s) from the camera
        Vector3Int tilePos = tilemap.WorldToCell(cam.ScreenToWorldPoint(mousePos));
        if (!TileManager.tiles.TryGetValue(tilePos, out _tile))
            return;

        Vector3 pos = _tile.GetMiddleOfTile();

        movePointController.Move(pos);
    }

    void Move(InputAction.CallbackContext _ctx)
    {
        Vector2 direction = _ctx.ReadValue<Vector2>();

        // Selecting face direction with arrows
        if (battleCharacterController.characterState == CharacterState.SelectingFaceDir)
        {
            if (direction == Vector2.up)
                battleCharacterController.selectedCharacter.GetComponent<FaceDirectionUI>().SimulateUpButtonClicked();
            if (direction == Vector2.left)
                battleCharacterController.selectedCharacter.GetComponent<FaceDirectionUI>().SimulateLeftButtonClicked();
            if (direction == Vector2.right)
                battleCharacterController.selectedCharacter.GetComponent<FaceDirectionUI>().SimulateRightButtonClicked();
            if (direction == Vector2.down)
                battleCharacterController.selectedCharacter.GetComponent<FaceDirectionUI>().SimulateDownButtonClicked();

            return;
        }

        if (!allowInput)
            return;

        // TODO: this is wrong, but it works.
        // with only normalize, if you press both arrows at the same time you will get (0.7, 0.7) vector        
        direction.Normalize();
        Vector2 vectorX = new Vector2(direction.x, 0).normalized;
        Vector2 vectorY = new Vector2(0, direction.y).normalized;

        movePointController.Move(new Vector3(transform.position.x + vectorX.x, transform.position.y + vectorY.y, transform.position.z));
    }
    void SelectClick(InputAction.CallbackContext _ctx)
    {
        movePointController.HandleSelectClick();
    }

    void BackClick(InputAction.CallbackContext _ctx)
    {
        if (!allowInput)
            return;

        battleCharacterController.Back();
        movePointController.UpdateDisplayInformation();
    }

    void SelectNextCharacter(InputAction.CallbackContext _ctx)
    {
        if (TurnManager.battleState == BattleState.Deployment)
            battleDeploymentController.SelectNextCharacter();
    }

    void SelectPreviousCharacter(InputAction.CallbackContext _ctx)
    {
        if (TurnManager.battleState == BattleState.Deployment)
            battleDeploymentController.SelectPreviousCharacter();
    }

    void CancelEverything(InputAction.CallbackContext _ctx)
    {
        allowInput = true;
        battleCharacterController.Back();
        battleCharacterController.UnselectCharacter();
    }

    // when you click Q on keyboard I want to simulate clicking a button with mouse
    void AButtonClickInput(InputAction.CallbackContext _ctx)
    {
        characterUI.SimulateAButtonClicked();
    }
    void SButtonClickInput(InputAction.CallbackContext _ctx)
    {
        characterUI.SimulateSButtonClicked();
    }
    void DButtonClickInput(InputAction.CallbackContext _ctx)
    {
        characterUI.SimulateDButtonClicked();
    }


    void QButtonClickInput(InputAction.CallbackContext _ctx)
    {
        characterUI.SimulateQButtonClicked();
    }
    void WButtonClickInput(InputAction.CallbackContext _ctx)
    {
        characterUI.SimulateWButtonClicked();
    }
    void EButtonClickInput(InputAction.CallbackContext _ctx)
    {
        characterUI.SimulateEButtonClicked();
    }
    void RButtonClickInput(InputAction.CallbackContext _ctx)
    {
        characterUI.SimulateRButtonClicked();
    }


}
