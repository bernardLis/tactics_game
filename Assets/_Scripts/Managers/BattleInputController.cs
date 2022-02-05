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

        tilemap = GameManager.instance.GetComponent<TileManager>().tilemap;

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
        playerInput.actions["LeftMouseClick"].performed += ctx => LeftMouseClick();
        playerInput.actions["ArrowMovement"].performed += ctx => Move(ctx.ReadValue<Vector2>());

        playerInput.actions["SelectClick"].performed += ctx => SelectClick();

        playerInput.actions["AButtonClick"].performed += ctx => AButtonClickInput();
        playerInput.actions["SButtonClick"].performed += ctx => SButtonClickInput();
        playerInput.actions["DButtonClick"].performed += ctx => DButtonClickInput();

        playerInput.actions["QButtonClick"].performed += ctx => QButtonClickInput();
        playerInput.actions["WButtonClick"].performed += ctx => WButtonClickInput();
        playerInput.actions["EButtonClick"].performed += ctx => EButtonClickInput();
        playerInput.actions["RButtonClick"].performed += ctx => RButtonClickInput();

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

        playerInput.actions["AButtonClick"].performed -= ctx => AButtonClickInput();
        playerInput.actions["SButtonClick"].performed -= ctx => SButtonClickInput();
        playerInput.actions["DButtonClick"].performed -= ctx => DButtonClickInput();

        playerInput.actions["QButtonClick"].performed -= ctx => QButtonClickInput();
        playerInput.actions["WButtonClick"].performed -= ctx => WButtonClickInput();
        playerInput.actions["EButtonClick"].performed -= ctx => EButtonClickInput();
        playerInput.actions["RButtonClick"].performed -= ctx => RButtonClickInput();

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

    public void SetInputAllowed(bool _isAllowed)
    {
        if (_isAllowed)
            characterUI.EnableSkillButtons();
        if (!_isAllowed)
            characterUI.DisableSkillButtons();

        allowInput = _isAllowed;
    }

    void LeftMouseClick()
    {
        if (battleCharacterController.characterState == CharacterState.SelectingFaceDir)
            return;

        if (!allowInput) // TODO: ||EventSystem.current.IsPointerOverGameObject() << throws an error;
            return;

        Vector3 mousePos = Mouse.current.position.ReadValue();
        mousePos.z = 1; // select distance = 1 unit(s) from the camera
        Vector3Int tilePos = tilemap.WorldToCell(cam.ScreenToWorldPoint(mousePos));
        if (!TileManager.tiles.TryGetValue(tilePos, out _tile))
            return;

        Vector3 pos = _tile.GetMiddleOfTile();

        movePointController.Move(pos);
    }

    void Move(Vector2 _direction)
    {
        // Selecting face direction with arrows
        if (battleCharacterController.characterState == CharacterState.SelectingFaceDir)
        {
            if (_direction == Vector2.up)
                battleCharacterController.selectedCharacter.GetComponent<FaceDirectionUI>().SimulateUpButtonClicked();
            if (_direction == Vector2.left)
                battleCharacterController.selectedCharacter.GetComponent<FaceDirectionUI>().SimulateLeftButtonClicked();
            if (_direction == Vector2.right)
                battleCharacterController.selectedCharacter.GetComponent<FaceDirectionUI>().SimulateRightButtonClicked();
            if (_direction == Vector2.down)
                battleCharacterController.selectedCharacter.GetComponent<FaceDirectionUI>().SimulateDownButtonClicked();

            return;
        }

        if (!allowInput)
            return;

        // TODO: this is wrong, but it works.
        // with only normalize, if you press both arrows at the same time you will get (0.7, 0.7) vector        
        _direction.Normalize();
        Vector2 vectorX = new Vector2(_direction.x, 0).normalized;
        Vector2 vectorY = new Vector2(0, _direction.y).normalized;

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

        battleCharacterController.Back();
        movePointController.UpdateDisplayInformation();
    }

    void SelectNextCharacter()
    {
        if (TurnManager.battleState == BattleState.Deployment)
        {
            battleDeploymentController.SelectNextCharacter();
            return;
        }

    }

    void SelectPreviousCharacter()
    {
        if (TurnManager.battleState == BattleState.Deployment)
        {
            battleDeploymentController.SelectPreviousCharacter();
            return;
        }
    }

    void CancelEverything()
    {
        allowInput = true;
        battleCharacterController.Back();
        battleCharacterController.UnselectCharacter();
    }

    // when you click Q on keyboard I want to simulate clicking a button with mouse
    void AButtonClickInput()
    {
        characterUI.SimulateAButtonClicked();
    }
    void SButtonClickInput()
    {
        characterUI.SimulateSButtonClicked();
    }
    void DButtonClickInput()
    {
        characterUI.SimulateDButtonClicked();
    }


    void QButtonClickInput()
    {
        characterUI.SimulateQButtonClicked();
    }
    void WButtonClickInput()
    {
        characterUI.SimulateWButtonClicked();
    }
    void EButtonClickInput()
    {
        characterUI.SimulateEButtonClicked();
    }
    void RButtonClickInput()
    {
        characterUI.SimulateRButtonClicked();
    }


}
