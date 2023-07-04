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
    PlayerInput _playerInput;
    BattleAbilityManager _abilityManager;

    VisualElement _root;

    [SerializeField] Ability _grabAbility; // for visual purposes
    AbilityButton _grabButton;

    [SerializeField] Texture2D _cursorGrabbingEnabled;
    [SerializeField] Texture2D _cursorGrabbed;

    public bool IsGrabbingEnabled { get; private set; }

    GameObject _grabbedObject;
    float _objectYPosition;
    int _floorLayerMask;

    bool _wasInitialized;

    public void Initialize()
    {
        if (_wasInitialized) return;
        _wasInitialized = true;

        _gameManager = GameManager.Instance;
        _audioManager = AudioManager.Instance;
        _playerInput = _gameManager.GetComponent<PlayerInput>();
        _abilityManager = GetComponent<BattleAbilityManager>();
        _floorLayerMask = LayerMask.GetMask("Floor");

        _root = GetComponent<UIDocument>().rootVisualElement;
        VisualElement container = _root.Q(className: _ussAbilityContainer);
        _grabButton = new(_grabAbility, "g");
        _grabButton.RegisterCallback<PointerDownEvent>(evt => ToggleGrabbing());
        container.Add(_grabButton);
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
        if (this == null) return;
        if (_grabButton.IsOnCooldown)
        {
            Helpers.DisplayTextOnElement(_root, _grabButton, "On cooldown!", Color.red);
            return;
        }
        Cursor.SetCursor(_cursorGrabbingEnabled, Vector2.zero, CursorMode.Auto);
        _grabButton.Highlight();

        IsGrabbingEnabled = true;
    }

    void DisableGrabbing()
    {
        if (this == null) return;
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
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
    }

    void UnsubscribeInputActions()
    {
        _playerInput.actions["LeftMouseClick"].canceled -= OnPointerUp;
        _playerInput.actions["RightMouseClick"].performed -= evt => DisableGrabbing();
        _playerInput.actions["EnableGrabbing"].performed -= evt => ToggleGrabbing();
    }

    public void TryGrabbing(BattleEntity entity)
    {
        if (!IsGrabbingAllowed()) return;
        _objectYPosition = entity.transform.position.y;
        _audioManager.PlaySFX("Grab", entity.transform.position);
        Cursor.SetCursor(_cursorGrabbed, Vector2.zero, CursorMode.Auto);
        _grabbedObject = entity.gameObject;
        entity.Grabbed();

        StartCoroutine(UpdateGrabbedObjectPosition());
    }

    public void TryGrabbing(GameObject obj)
    {
        if (!IsGrabbingAllowed()) return;
        _objectYPosition = 4f;
        _audioManager.PlaySFX("Grab", obj.transform.position);
        Cursor.SetCursor(_cursorGrabbed, Vector2.zero, CursorMode.Auto);
        _grabbedObject = obj;

        StartCoroutine(UpdateGrabbedObjectPosition());
    }

    IEnumerator UpdateGrabbedObjectPosition()
    {
        while (_grabbedObject != null)
        {
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 1000f, _floorLayerMask))
            {
                Vector3 pos = new Vector3(hit.point.x, _objectYPosition, hit.point.z);
                _grabbedObject.transform.position = pos;
            }
            yield return new WaitForFixedUpdate();
        }
    }

    public bool IsGrabbingAllowed()
    {
        if (!_wasInitialized) return false;
        if (_abilityManager.IsAbilitySelected) return false;
        if (_grabbedObject != null) return false;
        if (!IsGrabbingEnabled) return false;

        return true;
    }

    public void OnPointerUp(InputAction.CallbackContext context)
    {
        if (!_wasInitialized) return;
        if (this == null) return;
        if (_abilityManager.IsAbilitySelected) return;
        if (_grabbedObject == null) return;

        _audioManager.PlaySFX("Grab", _grabbedObject.transform.position);
        _grabAbility.StartCooldown();
        DisableGrabbing();

        if (_grabbedObject.TryGetComponent(out BattleEntity entity))
            entity.Released();
        if (_grabbedObject.TryGetComponent(out BattleGrabbableObstacle obstacle))
            obstacle.Released();

        _grabbedObject = null;
        StopAllCoroutines();
    }

    public void CancelGrabbing()
    {
        if (_grabbedObject == null) DisableGrabbing();
        OnPointerUp(default);
    }
}
