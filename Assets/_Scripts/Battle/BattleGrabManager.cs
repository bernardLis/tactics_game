using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class BattleGrabManager : Singleton<BattleGrabManager>
{
    GameManager _gameManager;
    PlayerInput _playerInput;
    BattleAbilityManager _abilityManager;

    BattleEntity _grabbedEntity;
    int _floorLayerMask;

    void Start()
    {
        _gameManager = GameManager.Instance;
        _playerInput = _gameManager.GetComponent<PlayerInput>();
        _abilityManager = GetComponent<BattleAbilityManager>();
        _floorLayerMask = LayerMask.GetMask("Floor");
    }

    void Update()
    {
        if (_abilityManager.IsAbilitySelected) return;
        if (_grabbedEntity == null) return;

        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 1000f, _floorLayerMask))
        {
            Vector3 pos = new Vector3(hit.point.x, 1f, hit.point.z);
            _grabbedEntity.transform.position = pos;
        }
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

    }

    void UnsubscribeInputActions()
    {
        _playerInput.actions["LeftMouseClick"].canceled -= OnPointerUp;
    }


    public void TryGrabbing(BattleEntity entity)
    {
        if (_abilityManager.IsAbilitySelected) return;
        if (_grabbedEntity != null) return;

        _grabbedEntity = entity;
        _grabbedEntity.StopRunEntityCoroutine();
    }


    public void OnPointerUp(InputAction.CallbackContext context)
    {
        if (this == null) return;
        if (_abilityManager.IsAbilitySelected) return;
        if (_grabbedEntity == null) return;

        _grabbedEntity.StartRunEntityCoroutine();
        _grabbedEntity = null;
    }




}
