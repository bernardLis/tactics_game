using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BattleInputManager : MonoBehaviour
{
    GameManager _gameManager;
    PlayerInput _playerInput;

    MenuScreen _menuScreen;

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
        _playerInput.actions["ToggleMenu"].performed += OpenMenu;
    }

    void UnsubscribeInputActions()
    {
        _playerInput.actions["Continue"].performed -= evt => OnContinueClicked?.Invoke();
        _playerInput.actions["Enter"].performed -= evt => OnEnterClicked?.Invoke();
        _playerInput.actions["ToggleMenu"].performed -= OpenMenu;
    }


    public void OpenMenu(InputAction.CallbackContext ctx)
    {
        if (_gameManager.OpenFullScreens.Count > 0) return;
        if (_menuScreen != null) return;

        _menuScreen = new MenuScreen();
        _menuScreen.OnHide += MenuScreenClosed;
    }

    void MenuScreenClosed()
    {
        _menuScreen = null;
    }

}
