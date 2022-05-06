using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;


public class GameUIManager : MonoBehaviour
{
    PlayerInput _playerInput;

    UIDocument _uiDocument;

    bool isMenuOpen;

    void Start()
    {
        _playerInput = GameManager.Instance.GetComponent<PlayerInput>();
        UnsubscribeInputActions();
        SubscribeInputActions();

        _uiDocument = GetComponent<UIDocument>();
    }

    void SubscribeInputActions()
    {
        _playerInput.actions["OpenMenu"].performed += ShowMenu;
    }

    public void UnsubscribeInputActions()
    {
        _playerInput.actions["OpenMenu"].performed -= ShowMenu;
    }

    void ShowMenu(InputAction.CallbackContext ctx)
    {
        if (SceneManager.GetActiveScene().name == "Main Menu" || isMenuOpen)
            return;

        isMenuOpen = true;
        MenuScreenVisual menu = new MenuScreenVisual(_uiDocument.rootVisualElement);
    }

    public void SetIsMenuOpen(bool isIt)
    {
        isMenuOpen = isIt;
    }
}
