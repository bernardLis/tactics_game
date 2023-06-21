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

    ScrollView _logContainer;
    static string myLog = "";

    bool _isOpen;

    Label _fpsLabel;
    float _deltaTime;

    [SerializeField] Creature _metalon;

    void Awake()
    {
        _gameManager = GetComponent<GameManager>();
        _playerInput = GetComponent<PlayerInput>();

        VisualElement root = GetComponent<UIDocument>().rootVisualElement;

        _commandLineContainer = root.Q<VisualElement>("commandLineContainer");
        _commandTextField = root.Q<TextField>("commandLineTextField");
        _submitCommandButton = root.Q<Button>("commandLineButton");
        _submitCommandButton.clickable.clicked += SubmitCommand;

        _logContainer = root.Q<ScrollView>("logContainer");

        _fpsLabel = root.Q<Label>("fpsLabel");
    }

    void Update()
    {
        _deltaTime += (Time.deltaTime - _deltaTime) * 0.1f;
        float fps = 1.0f / _deltaTime;
        _fpsLabel.text = $"{Mathf.Ceil(fps)}";
    }

    /* INPUT */
    void OnEnable()
    {
        if (_gameManager == null)
            _gameManager = GameManager.Instance;
        _playerInput = _gameManager.GetComponent<PlayerInput>();
        UnsubscribeInputActions();
        SubscribeInputActions();

        Application.logMessageReceived += Log;
    }

    void OnDisable()
    {
        if (_playerInput == null)
            return;
        UnsubscribeInputActions();

        Application.logMessageReceived -= Log;
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
        if (_commandTextField.text.ToLower() == "quit")
            Application.Quit();
        if (_commandTextField.text.ToLower() == "clearsave")
            _gameManager.ClearSaveData();

        if (_commandTextField.text.ToLower() == "warren")
            _gameManager.ChangeGoldValue(10000);
        if (_commandTextField.text.ToLower() == "takegold")
            _gameManager.ChangeGoldValue(-5000);
        if (_commandTextField.text.ToLower() == "diuna")
            _gameManager.ChangeSpiceValue(1000);
        if (_commandTextField.text.ToLower() == "takespice")
            _gameManager.ChangeSpiceValue(-500);
        if (_commandTextField.text.ToLower() == "levelup")
            _gameManager.PlayerHero.LevelUp();
    }

    public void Log(string logString, string stackTrace, LogType type)
    {
        if (_logContainer == null) return;
        _logContainer.Add(new Label(logString));

        myLog = logString + "\n" + myLog;
        if (myLog.Length > 5000)
            myLog = myLog.Substring(0, 4000);

        FileManager.WriteToFile("log", myLog);
    }

}
