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

    [HideInInspector] public bool AllowInput { get; private set; }

    protected override void Awake() { base.Awake(); }

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
            SetInputAllowed(false);
        if (_state == BattleState.Lost)
            SetInputAllowed(false);
    }

    void OnDestroy() { TurnManager.OnBattleStateChanged -= TurnManager_OnBattleStateChanged; }

    void HandleDeployment() { SetInputAllowed(true); }

    void HandlePlayerTurn() { SetInputAllowed(true); }

    void HandleEnemyTurn() { SetInputAllowed(false); }

    void SubscribeInputActions()
    {
        // char placement specific for now
        _playerInput.actions["SelectNextCharacter"].performed += SelectNextCharacter;
        _playerInput.actions["SelectPreviousCharacter"].performed += SelectPreviousCharacter;

        _playerInput.actions["LeftMouseClick"].performed += LeftMouseClick;
        _playerInput.actions["ArrowMovement"].performed += Move;

        _playerInput.actions["SelectClick"].performed += SelectClick;
        _playerInput.actions["DetailsClick"].performed += DetailsClick;

        _playerInput.actions["BasicAttackClick"].performed += AbilityButtonClicked;
        _playerInput.actions["BasicDefenseClick"].performed += Defend;

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

        _playerInput.actions["BasicAttackClick"].performed -= AbilityButtonClicked;
        _playerInput.actions["BasicDefenseClick"].performed -= Defend;

        _playerInput.actions["FirstAbilityClick"].performed -= AbilityButtonClicked;
        _playerInput.actions["SecondAbilityClick"].performed -= AbilityButtonClicked;
        _playerInput.actions["ThirdAbilityClick"].performed -= AbilityButtonClicked;
        _playerInput.actions["FourthAbilityClick"].performed -= AbilityButtonClicked;

        _playerInput.actions["BackClick"].performed -= BackClick;
    }

    public bool IsInputAllowed() { return AllowInput; }

    public void SetInputAllowed(bool isAllowed) { AllowInput = isAllowed; }

    void LeftMouseClick(InputAction.CallbackContext ctx)
    {
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
        {
            if (ve != null)
            {
                Color32 bcol = ve.resolvedStyle.backgroundColor;
                if (bcol.a != 0 && ve.enabledInHierarchy)
                    return true;
            }
        }
        return false;
    }

    void Move(InputAction.CallbackContext ctx)
    {
        Vector2 direction = ctx.ReadValue<Vector2>();

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

        Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, 0.2f);
        foreach (Collider2D c in cols)
            if (c.TryGetComponent(out CharacterStats stats))
                _battleUI.ShowCharacterScreen(stats.Character);
    }

    void BackClick(InputAction.CallbackContext ctx)
    {
        if (!AllowInput)
            return;

        if (_battleUI.CharacterScreen != null)
            _battleUI.HideCharacterScreen();

        _battleCharacterController.Back();
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
        SetInputAllowed(true);
        _battleCharacterController.Back();
        _battleCharacterController.UnselectCharacter();
    }

    void AbilityButtonClicked(InputAction.CallbackContext ctx) { _characterUI.SimulateAbilityButtonClicked(ctx); }

    void Defend(InputAction.CallbackContext ctx)
    {
        if (!IsInputAllowed())
            return;

        if (!_battleCharacterController.CanSelectAbility())
            return;

        Debug.Log($"_battleCharacterController.SelectedCharacter {_battleCharacterController.SelectedCharacter}");
        Vector2 dir = Helpers.GetDirectionToClosestWithTag(_battleCharacterController.SelectedCharacter, Tags.Enemy);
        _battleCharacterController.SelectedCharacter.GetComponentInChildren<CharacterRendererManager>().Face(dir);
        _battleCharacterController.FinishCharacterTurn();
    }
}
