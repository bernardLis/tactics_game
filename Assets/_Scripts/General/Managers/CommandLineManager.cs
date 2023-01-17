using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class CommandLineManager : MonoBehaviour
{
    GameManager _gameManager;
    PlayerInput _playerInput;

    VisualElement _commandLineContainer;
    TextField _commandTextField;
    Button _submitCommandButton;

    bool _isOpen;

    void Start()
    {
        _gameManager = GetComponent<GameManager>();
        _playerInput = GetComponent<PlayerInput>();

        VisualElement root = GetComponent<UIDocument>().rootVisualElement;

        _commandLineContainer = root.Q<VisualElement>("commandLineContainer");
        _commandTextField = root.Q<TextField>("commandLineTextField");
        _submitCommandButton = root.Q<Button>("commandLineButton");

        _submitCommandButton.clickable.clicked += SubmitCommand;
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
        if (_playerInput == null)
            return;
        UnsubscribeInputActions();
    }

    void SubscribeInputActions()
    {
        _playerInput.actions["ToggleCommandLine"].performed += ToggleCommandLine;
    }

    void UnsubscribeInputActions()
    {
        _playerInput.actions["ToggleCommandLine"].performed -= ToggleCommandLine;
    }

    void ToggleCommandLine(InputAction.CallbackContext ctx)
    {
        if (_isOpen)
            _commandLineContainer.style.display = DisplayStyle.None;
        else
            _commandLineContainer.style.display = DisplayStyle.Flex;

        _isOpen = !_isOpen;
    }

    void SubmitCommand()
    {
        if (_commandTextField.text.ToLower() == "gold")
            _gameManager.ChangeGoldValue(10000);
        if (_commandTextField.text.ToLower() == "takegold")
            _gameManager.ChangeGoldValue(-5000);
        if (_commandTextField.text.ToLower() == "spice")
            _gameManager.ChangeSpiceValue(1000);
        if (_commandTextField.text.ToLower() == "takespice")
            _gameManager.ChangeSpiceValue(-500);
        if (_commandTextField.text.ToLower() == "levelup")
            for (int i = _gameManager.PlayerTroops.Count - 1; i >= 0; i--)
                _gameManager.PlayerTroops[i].LevelUp();
    }
}
