using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using Cursor = UnityEngine.Cursor;
using DG.Tweening;

public class BattleGrabManager : Singleton<BattleGrabManager>
{
    const string _ussClassName = "battle__";
    const string _ussAbilityContainer = _ussClassName + "ability-container";

    GameManager _gameManager;
    AudioManager _audioManager;
    CursorManager _cursorManager;
    PlayerInput _playerInput;
    BattleDeploymentManager _playerArmyDeployer;
    BattleTooltipManager _tooltipManager;

    VisualElement _root;

    [SerializeField] Ability _grabAbility; // for visual purposes
    AbilityButton _grabButton;

    public bool IsGrabbingEnabled { get; private set; }

    GameObject _grabbedObject;
    float _objectYPosition;
    int _floorLayerMask;

    bool _wasInitialized;

    void Start()
    {
        _playerArmyDeployer = GetComponent<BattleDeploymentManager>();
        _playerArmyDeployer.OnPlayerArmyDeployed += Initialize;
    }

    public void Initialize()
    {
        if (_wasInitialized) return;
        _wasInitialized = true;
        _playerArmyDeployer.OnPlayerArmyDeployed -= Initialize;

        _gameManager = GameManager.Instance;
        _audioManager = AudioManager.Instance;
        _cursorManager = CursorManager.Instance;
        _tooltipManager = BattleTooltipManager.Instance;
        _playerInput = _gameManager.GetComponent<PlayerInput>();
        _floorLayerMask = LayerMask.GetMask("Floor");
    }

    public void ToggleGrabbing()
    {
        if (_grabbedObject != null) return;

        if (IsGrabbingEnabled)
            DisableGrabbing();
        else
            EnableGrabbing();
    }

    public void EnableGrabbing()
    {
        if (BattleManager.BlockBattleInput) return;
        if (this == null) return;
        if (_grabButton == null) return;
        if (_grabButton.IsOnCooldown)
        {
            Helpers.DisplayTextOnElement(_root, _grabButton, "On cooldown!", Color.red);
            return;
        }
        _cursorManager.SetCursorByName("Grab");
        _grabButton.Highlight();

        IsGrabbingEnabled = true;
    }

    void DisableGrabbing()
    {
        if (this == null) return;
        if (_grabButton == null) return;

        _cursorManager.ClearCursor();
        _grabButton.ClearHighlight();

        IsGrabbingEnabled = false;
    }

    /* INPUT */
    void OnEnable()
    {
        if (_gameManager == null)
            _gameManager = GameManager.Instance;

        _playerInput = _gameManager.GetComponent<PlayerInput>();
        _playerInput.SwitchCurrentActionMap("Battle");
        UnsubscribeInputActions();
        SubscribeInputActions();
    }

    void OnDisable()
    {
        if (_playerInput == null) return;

        UnsubscribeInputActions();
    }

    void OnDestroy()
    {
        if (_playerInput == null) return;

        UnsubscribeInputActions();
    }

    void SubscribeInputActions()
    {
        _playerInput.actions["LeftMouseClick"].canceled += OnPointerUp;
        _playerInput.actions["RightMouseClick"].performed += evt => DisableGrabbing();
        _playerInput.actions["EnableGrabbing"].performed += evt => ToggleGrabbing();
        _playerInput.actions["Rotate"].performed += RotateObject;
    }

    void UnsubscribeInputActions()
    {
        _playerInput.actions["LeftMouseClick"].canceled -= OnPointerUp;
        _playerInput.actions["RightMouseClick"].performed -= evt => DisableGrabbing();
        _playerInput.actions["EnableGrabbing"].performed -= evt => ToggleGrabbing();
        _playerInput.actions["Rotate"].performed -= RotateObject;
    }

    // returns true if successful
    public bool TryGrabbing(GameObject obj, float yPosition = 0f)
    {
        if (!IsGrabbingAllowed()) return false;

        _objectYPosition = obj.transform.position.y;
        if (yPosition != 0f)
            _objectYPosition = yPosition;

        _audioManager.PlaySFX("Grab", obj.transform.position);
        _cursorManager.SetCursorByName("Hold");

        _grabbedObject = obj;
        if (_grabbedObject.TryGetComponent(out IGrabbable g))
            g.Grabbed();

        StartCoroutine(UpdateGrabbedObjectPosition());
        return true;
    }

    IEnumerator UpdateGrabbedObjectPosition()
    {
        while (_grabbedObject != null)
        {
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (Physics.Raycast(ray, out RaycastHit hit, 1000f, _floorLayerMask))
            {
                Vector3 pos = new(hit.point.x, _objectYPosition, hit.point.z);
                _grabbedObject.transform.position = pos;
            }
            yield return new WaitForFixedUpdate();
        }
    }

    void RotateObject(InputAction.CallbackContext context)
    {
        if (_grabbedObject == null) return;
        _grabbedObject.transform.Rotate(Vector3.up, 30f);
    }

    public bool IsGrabbingAllowed()
    {

        if (!_wasInitialized) return false;
        if (_grabbedObject != null) return false;
        if (!IsGrabbingEnabled) return false;

        return true;
    }

    public void OnPointerUp(InputAction.CallbackContext context)
    {
        if (!_wasInitialized) return;
        if (this == null) return;
        if (_grabbedObject == null) return;

        _audioManager.PlaySFX("Grab", _grabbedObject.transform.position);
        _grabAbility.StartCooldown();
        DisableGrabbing();

        if (_grabbedObject.TryGetComponent(out IGrabbable g))
            g.Released();

        _grabbedObject = null;
        _tooltipManager.HideKeyTooltipInfo();
        StopAllCoroutines();
    }

    public void CancelGrabbing()
    {
        if (_grabbedObject == null) DisableGrabbing();
        OnPointerUp(default);
    }
}
