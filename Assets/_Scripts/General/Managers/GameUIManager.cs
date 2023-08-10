using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;


public class GameUIManager : MonoBehaviour
{
    GameManager _gameManager;

    PlayerInput _playerInput;

    MenuScreen _menuScreen;

    void Start()
    {
        _gameManager = GameManager.Instance;
    }

    /* INPUT */
    void OnEnable()
    {
        if (_gameManager == null)
            _gameManager = GameManager.Instance;

        _playerInput = _gameManager.GetComponent<PlayerInput>();
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
        _playerInput.actions["ToggleMenu"].performed += OpenMenu;
    }

    public void UnsubscribeInputActions()
    {
        _playerInput.actions["ToggleMenu"].performed -= OpenMenu;
    }

    public void OpenMenu(InputAction.CallbackContext ctx)
    {
        if (SceneManager.GetActiveScene().name == Scenes.MainMenu) return;
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
