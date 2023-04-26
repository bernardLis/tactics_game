using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BattleInputManager : MonoBehaviour
{
    GameManager _gameManager;
    PlayerInput _playerInput;


    public event Action OnContinueClicked;
    public event Action OnEnterClicked;
    void Start()
    {
        _gameManager = GameManager.Instance;
        _playerInput = _gameManager.GetComponent<PlayerInput>();
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
        _playerInput.actions["Continue"].performed += evt => OnContinueClicked?.Invoke();
        _playerInput.actions["Enter"].performed += evt => OnEnterClicked?.Invoke();


    }

    void UnsubscribeInputActions()
    {
        _playerInput.actions["Continue"].performed -= evt => OnContinueClicked?.Invoke();
        _playerInput.actions["Enter"].performed -= evt => OnEnterClicked?.Invoke();

    }
}
