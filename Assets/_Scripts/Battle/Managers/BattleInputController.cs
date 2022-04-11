using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.InputSystem;

public class BattleInputController : Singleton<BattleInputController>
{
    // input system
    PlayerInput _playerInput;

    // tilemap
    Tilemap _tilemap;
    WorldTile _tile;

    // global utilities
    Camera _cam;
    CharacterUI _characterUI;

    // local
    MovePointController _movePointController;
    BattleCharacterController _battleCharacterController;
    BattleDeploymentController _battleDeploymentController;
    OscilateScale _oscilateScale;

    [HideInInspector] public bool AllowInput { get; private set; }

    protected override void Awake()
    {
        base.Awake();
    }

    void Start()
    {
        TurnManager.OnBattleStateChanged += TurnManager_OnBattleStateChanged;

        _playerInput = GetComponent<PlayerInput>();

        _tilemap = BattleManager.Instance.GetComponent<TileManager>().Tilemap;

        // TODO: Supposedly, this is an expensive call
        _cam = Camera.main;
        _characterUI = CharacterUI.Instance;

        _movePointController = MovePointController.Instance;
        _battleCharacterController = GetComponent<BattleCharacterController>();
        _battleDeploymentController = GetComponent<BattleDeploymentController>();
        _oscilateScale = GetComponentInChildren<OscilateScale>();
    }

    void OnEnable()
    {
        // inputs
        _playerInput = GetComponent<PlayerInput>();

        SubscribeInputActions();
    }

    void OnDisable()
    {
        if (_playerInput == null)
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
        if (_state == BattleState.Won)
            AllowInput = false;
        if (_state == BattleState.Lost)
            AllowInput = false;
    }

    void OnDestroy()
    {
        TurnManager.OnBattleStateChanged -= TurnManager_OnBattleStateChanged;
    }

    void HandleDeployment()
    {
        AllowInput = true;
    }

    void HandlePlayerTurn()
    {
        SetInputAllowed(true);
        _oscilateScale.SetOscilation(true);
    }

    void HandleEnemyTurn()
    {
        SetInputAllowed(false);
        _oscilateScale.SetOscilation(false);
    }

    void SubscribeInputActions()
    {
        // char placement specific for now
        _playerInput.actions["SelectNextCharacter"].performed += SelectNextCharacter;
        _playerInput.actions["SelectPreviousCharacter"].performed += SelectPreviousCharacter;

        _playerInput.actions["LeftMouseClick"].performed += LeftMouseClick;
        _playerInput.actions["ArrowMovement"].performed += Move;

        _playerInput.actions["SelectClick"].performed += SelectClick;

        _playerInput.actions["OpenInventoryClick"].performed += OpenInventoryClicked;

        _playerInput.actions["AButtonClick"].performed += AbilityButtonClicked;
        _playerInput.actions["SButtonClick"].performed += AbilityButtonClicked;

        _playerInput.actions["QButtonClick"].performed += AbilityButtonClicked;
        _playerInput.actions["WButtonClick"].performed += AbilityButtonClicked;
        _playerInput.actions["EButtonClick"].performed += AbilityButtonClicked;
        _playerInput.actions["RButtonClick"].performed += AbilityButtonClicked;

        _playerInput.actions["Back"].performed += BackClick;

        _playerInput.actions["CancelEverything"].performed += CancelEverything;
    }

    void UnsubscribeInputActions()
    {
        // char placement specific for now
        _playerInput.actions["SelectNextCharacter"].performed -= SelectNextCharacter;
        _playerInput.actions["SelectPreviousCharacter"].performed -= SelectPreviousCharacter;

        _playerInput.actions["LeftMouseClick"].performed -= LeftMouseClick;
        _playerInput.actions["ArrowMovement"].performed -= Move;

        _playerInput.actions["SelectClick"].performed -= SelectClick;

        _playerInput.actions["OpenInventoryClick"].performed -= OpenInventoryClicked;

        _playerInput.actions["AButtonClick"].performed -= AbilityButtonClicked;
        _playerInput.actions["SButtonClick"].performed -= AbilityButtonClicked;

        _playerInput.actions["QButtonClick"].performed -= AbilityButtonClicked;
        _playerInput.actions["WButtonClick"].performed -= AbilityButtonClicked;
        _playerInput.actions["EButtonClick"].performed -= AbilityButtonClicked;
        _playerInput.actions["RButtonClick"].performed -= AbilityButtonClicked;

        _playerInput.actions["Back"].performed -= BackClick;

        _playerInput.actions["CancelEverything"].performed -= CancelEverything;
    }

    public bool IsInputAllowed()
    {
        return AllowInput;
    }

    public void SetInputAllowed(bool isAllowed)
    {
        AllowInput = isAllowed;
    }

    void LeftMouseClick(InputAction.CallbackContext ctx)
    {
        if (_battleCharacterController.CharacterState == CharacterState.SelectingFaceDir)
            return;

        if (!AllowInput) // TODO: ||EventSystem.current.IsPointerOverGameObject() << throws an error;
            return;

        Vector3 mousePos = Mouse.current.position.ReadValue();
        mousePos.z = 0; // select distance = 1 unit(s) from the camera
        Vector3Int tilePos = _tilemap.WorldToCell(_cam.ScreenToWorldPoint(mousePos));
        if (!TileManager.Tiles.TryGetValue(tilePos, out _tile))
            return;

        Vector3 pos = _tile.GetMiddleOfTile();

        _movePointController.Move(pos);
    }

    void Move(InputAction.CallbackContext ctx)
    {
        Vector2 direction = ctx.ReadValue<Vector2>();

        // Selecting face direction with arrows
        if (_battleCharacterController.CharacterState == CharacterState.SelectingFaceDir)
        {
            if (direction == Vector2.up)
                _battleCharacterController.SelectedCharacter.GetComponent<FaceDirectionUI>().SimulateUpButtonClicked();
            if (direction == Vector2.left)
                _battleCharacterController.SelectedCharacter.GetComponent<FaceDirectionUI>().SimulateLeftButtonClicked();
            if (direction == Vector2.right)
                _battleCharacterController.SelectedCharacter.GetComponent<FaceDirectionUI>().SimulateRightButtonClicked();
            if (direction == Vector2.down)
                _battleCharacterController.SelectedCharacter.GetComponent<FaceDirectionUI>().SimulateDownButtonClicked();

            return;
        }

        if (!AllowInput)
            return;

        // TODO: this is wrong, but it works.
        // with only normalize, if you press both arrows at the same time you will get (0.7, 0.7) vector        
        direction.Normalize();
        Vector2 vectorX = new Vector2(direction.x, 0).normalized;
        Vector2 vectorY = new Vector2(0, direction.y).normalized;

        _movePointController.Move(new Vector3(transform.position.x + vectorX.x, transform.position.y + vectorY.y, transform.position.z));
    }
    void SelectClick(InputAction.CallbackContext ctx)
    {
        _movePointController.HandleSelectClick();
    }

    void BackClick(InputAction.CallbackContext ctx)
    {
        if (!AllowInput)
            return;

        _battleCharacterController.Back();
        _movePointController.UpdateDisplayInformation();
    }

    void SelectNextCharacter(InputAction.CallbackContext ctx)
    {
        if (TurnManager.BattleState == BattleState.Deployment)
            _battleDeploymentController.SelectNextCharacter();
    }

    void SelectPreviousCharacter(InputAction.CallbackContext ctx)
    {
        if (TurnManager.BattleState == BattleState.Deployment)
            _battleDeploymentController.SelectPreviousCharacter();
    }

    void CancelEverything(InputAction.CallbackContext ctx)
    {
        AllowInput = true;
        _battleCharacterController.Back();
        _battleCharacterController.UnselectCharacter();
    }

    void OpenInventoryClicked(InputAction.CallbackContext ctx)
    {
        _characterUI.SimulateOpenInventoryClicked();
    }

    void AbilityButtonClicked(InputAction.CallbackContext ctx)
    {
        _characterUI.SimulateAbilityButtonClicked(ctx);
    }
}
