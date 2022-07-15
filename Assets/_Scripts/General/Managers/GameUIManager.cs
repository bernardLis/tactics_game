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

    UIDocument _uiDocument;

    MenuScreen _menuScreen;
    ViewTroopsScreen _viewTroopsScreen;

    void Start()
    {
        _gameManager = GameManager.Instance;
        _playerInput = _gameManager.GetComponent<PlayerInput>();
        UnsubscribeInputActions();
        SubscribeInputActions();

        _uiDocument = GetComponent<UIDocument>();
    }

    void SubscribeInputActions()
    {
        EnableMenuButton();
        _playerInput.actions["ViewTroopsClick"].performed += ShowTroopsScreen;

    }

    public void UnsubscribeInputActions()
    {
        DisableMenuButton();
        _playerInput.actions["ViewTroopsClick"].performed -= ShowTroopsScreen;
    }

    public void EnableMenuButton()
    {
        if (_menuScreen != null)
            return;

        _playerInput.actions["OpenMenuClick"].performed += ToggleMenu;
    }

    public void DisableMenuButton()
    {
        _playerInput.actions["OpenMenuClick"].performed -= ToggleMenu;
    }

    void ToggleMenu(InputAction.CallbackContext ctx)
    {
        if (SceneManager.GetActiveScene().name == "Main Menu")
            return;

        _menuScreen = new MenuScreen(_uiDocument.rootVisualElement);
        _menuScreen.OnClose += MenuScreenClosed;
        _playerInput.actions["OpenMenuClick"].performed -= ToggleMenu;

    }

    void MenuScreenClosed()
    {
        _menuScreen.OnClose -= MenuScreenClosed;

        _menuScreen = null;
        _playerInput.actions["OpenMenuClick"].performed += ToggleMenu;
    }

    public void ShowTroopsScreen(InputAction.CallbackContext ctx)
    {
        ShowTroopsScreenNoContext();
    }

    public void ShowTroopsScreenNoContext()
    {
        if (SceneManager.GetActiveScene().name == "Main Menu")
            return;

        if (_viewTroopsScreen != null)
            _viewTroopsScreen.Hide();

        _viewTroopsScreen = new ViewTroopsScreen(_gameManager.PlayerTroops, _uiDocument.rootVisualElement);
        _viewTroopsScreen.AddToClassList("menuScreen");
        _viewTroopsScreen.OnClose += TroopsScreenClosed;
    }

    void TroopsScreenClosed()
    {
        _viewTroopsScreen.OnClose -= TroopsScreenClosed;
        _viewTroopsScreen = null;
    }
}
