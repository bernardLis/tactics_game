using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MainMenuUI : MonoBehaviour
{
    GameManager _gameManager;

    VisualElement _root;
    Button _continueButton;
    Button _startNewGameButton;
    Button _loadGameButton;
    Button _settingsButton;
    Button _quitButton;

    VisualElement _menuContainer;
    VisualElement _newGameScreen;


    void Start()
    {
        _gameManager = GameManager.Instance;

        _root = GetComponent<UIDocument>().rootVisualElement;
        _continueButton = _root.Q<Button>("continueButton");
        _startNewGameButton = _root.Q<Button>("startNewGameButton");
        _loadGameButton = _root.Q<Button>("loadGameButton");
        _settingsButton = _root.Q<Button>("settingsButton");
        _quitButton = _root.Q<Button>("quitButton");

        _menuContainer = _root.Q<VisualElement>("menuContainer");
        _newGameScreen = _root.Q<VisualElement>("newGameContainer");

        _continueButton.clickable.clicked += Continue;
        _startNewGameButton.clickable.clicked += StartNewGame;
        _loadGameButton.clickable.clicked += LoadGame;
        _settingsButton.clickable.clicked += Settings;
        _quitButton.clickable.clicked += Quit;
    }

    void Continue()
    {
        string lastSave = PlayerPrefs.GetString("lastSave");
        if (lastSave == null)
            _continueButton.style.backgroundColor = Color.gray;
        _gameManager.StartGameFromSave(lastSave);
    }

    void StartNewGame()
    {
        // TODO: I could make a transition with dotween
        _menuContainer.style.display = DisplayStyle.None;
        _newGameScreen.style.display = DisplayStyle.Flex;
    }

    void LoadGame()
    {
        string[] saveFiles = FileManager.LoadALlSaveFiles();
        new LoadGameScreenVisual(_root, saveFiles);
    }

    void Settings()
    {
        Debug.Log("settings click");
    }

    void Quit()
    {
        Debug.Log("quit click");
        Application.Quit();
    }
}
