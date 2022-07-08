using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class BattleInputController : Singleton<BattleInputController>
{
    // global utilities
    GameManager _gameManager;
    Camera _cam;
    CharacterUI _characterUI;
    BattleUI _battleUI;

    // input system
    PlayerInput _playerInput;

    // tilemap
    Tilemap _tilemap;
    WorldTile _tile;


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

        // TODO: Supposedly, this is an expensive call
        _gameManager = GameManager.Instance;
        _cam = Camera.main;
        _characterUI = CharacterUI.Instance;

        _playerInput = _gameManager.GetComponent<PlayerInput>();

        _tilemap = BattleManager.Instance.GetComponent<TileManager>().Tilemap;

        _movePointController = MovePointController.Instance;
        _battleUI = BattleUI.Instance;

        _battleCharacterController = GetComponent<BattleCharacterController>();
        _battleDeploymentController = GetComponent<BattleDeploymentController>();
        _oscilateScale = GetComponentInChildren<OscilateScale>();
    }

    void OnEnable()
    {
        // inputs
        if (_gameManager == null)
            _gameManager = GameManager.Instance;
        _playerInput = _gameManager.GetComponent<PlayerInput>();

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
        _playerInput.actions["DetailsClick"].performed += DetailsClick;

        _playerInput.actions["OpenInventoryClick"].performed += OpenInventoryClicked;

        _playerInput.actions["BasicAttackClick"].performed += AbilityButtonClicked;
        _playerInput.actions["BasicDefenseClick"].performed += AbilityButtonClicked;

        _playerInput.actions["FirstAbilityClick"].performed += AbilityButtonClicked;
        _playerInput.actions["SecondAbilityClick"].performed += AbilityButtonClicked;
        _playerInput.actions["ThirdAbilityClick"].performed += AbilityButtonClicked;
        _playerInput.actions["FourthAbilityClick"].performed += AbilityButtonClicked;

        _playerInput.actions["BackClick"].performed += BackClick;
    }

    void UnsubscribeInputActions()
    {
        // char placement specific for now
        _playerInput.actions["SelectNextCharacter"].performed -= SelectNextCharacter;
        _playerInput.actions["SelectPreviousCharacter"].performed -= SelectPreviousCharacter;

        _playerInput.actions["LeftMouseClick"].performed -= LeftMouseClick;
        _playerInput.actions["ArrowMovement"].performed -= Move;

        _playerInput.actions["SelectClick"].performed -= SelectClick;
        _playerInput.actions["DetailsClick"].performed -= DetailsClick;

        _playerInput.actions["OpenInventoryClick"].performed -= OpenInventoryClicked;

        _playerInput.actions["BasicAttackClick"].performed -= AbilityButtonClicked;
        _playerInput.actions["BasicDefenseClick"].performed -= AbilityButtonClicked;

        _playerInput.actions["FirstAbilityClick"].performed -= AbilityButtonClicked;
        _playerInput.actions["SecondAbilityClick"].performed -= AbilityButtonClicked;
        _playerInput.actions["ThirdAbilityClick"].performed -= AbilityButtonClicked;
        _playerInput.actions["FourthAbilityClick"].performed -= AbilityButtonClicked;

        _playerInput.actions["BackClick"].performed -= BackClick;
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
        if (IsPointerOverUI(mousePos))
            return;

        mousePos.z = 0; // select distance = 1 unit(s) from the camera
        Vector3Int tilePos = _tilemap.WorldToCell(_cam.ScreenToWorldPoint(mousePos));
        if (!TileManager.Tiles.TryGetValue(tilePos, out _tile))
            return;

        Vector3 pos = _tile.GetMiddleOfTile();

        _movePointController.Move(pos);
    }

    //https://answers.unity.com/questions/1881324/ui-toolkit-prevent-click-through-visual-element.html
    bool IsPointerOverUI(Vector2 screenPos)
    {
        Vector2 pointerUiPos = new Vector2 { x = screenPos.x, y = Screen.height - screenPos.y };
        List<VisualElement> picked = new List<VisualElement>();
        if (_battleUI == null)
            return false;
        _battleUI.Root.panel.PickAll(pointerUiPos, picked);
        foreach (var ve in picked)
            if (ve != null)
            {
                Color32 bcol = ve.resolvedStyle.backgroundColor;
                if (bcol.a != 0 && ve.enabledInHierarchy)
                    return true;
            }
        return false;
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
        if (!AllowInput)
            return;
        _movePointController.HandleSelectClick();
    }

    void DetailsClick(InputAction.CallbackContext ctx)
    {
        if (_battleUI.CharacterScreen != null)
        {
            _battleUI.HideCharacterScreen();
            return;
        }

        // this is niche: on the ocassion when there are 2 characters on the tile (troops deployment), 
        // I want to show the second character; 
        List<CharacterStats> statsList = new();

        Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, 0.2f);
        foreach (Collider2D c in cols)
            if (c.TryGetComponent(out CharacterStats stats))
                statsList.Add(stats);

        _battleUI.ShowCharacterScreen(statsList[statsList.Count - 1].Character);

    }

    void BackClick(InputAction.CallbackContext ctx)
    {
        if (!AllowInput)
            return;

        if (_battleUI.CharacterScreen != null)
            _battleUI.HideCharacterScreen();

        _battleCharacterController.Back();
        //_movePointController.UpdateDisplayInformation();
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
