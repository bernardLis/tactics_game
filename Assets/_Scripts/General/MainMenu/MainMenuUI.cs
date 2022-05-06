using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

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
    VisualElement _settingsContainer;


    void Start()
    {
        _gameManager = GameManager.Instance;

        _root = GetComponent<UIDocument>().rootVisualElement;
        _continueButton = _root.Q<Button>("continueButton");
        ResolveContinueButton();
        _startNewGameButton = _root.Q<Button>("startNewGameButton");
        _loadGameButton = _root.Q<Button>("loadGameButton");
        ResolveLoadGameButton();
        _settingsButton = _root.Q<Button>("settingsButton");
        _quitButton = _root.Q<Button>("quitButton");

        _menuContainer = _root.Q<VisualElement>("menuContainer");
        _newGameScreen = _root.Q<VisualElement>("newGameContainer");
        _settingsContainer = _root.Q<VisualElement>("settingsContainer");

        _continueButton.clickable.clicked += Continue;
        _startNewGameButton.clickable.clicked += StartNewGame;
        _loadGameButton.clickable.clicked += LoadGame;
        _settingsButton.clickable.clicked += Settings;
        _quitButton.clickable.clicked += ConfirmQuit;
    }

    void ResolveContinueButton()
    {
        string lastSave = PlayerPrefs.GetString("lastSave");
        if (lastSave == null || !FileManager.FileExists(lastSave))
        {
            _continueButton.SetEnabled(false);
            _continueButton.style.backgroundColor = Color.gray;
        }
    }

    void Continue()
    {
        string lastSave = PlayerPrefs.GetString("lastSave");

        if (lastSave == null)
            return;
        if (!FileManager.FileExists(lastSave))
            return;

        _gameManager.StartGameFromSave(lastSave);
    }

    void StartNewGame()
    {
        // TODO: I could make a transition with dotween
        _menuContainer.style.display = DisplayStyle.None;
        _newGameScreen.style.display = DisplayStyle.Flex;
    }


    void ResolveLoadGameButton()
    {
        string[] saveFiles = FileManager.LoadALlSaveFiles();
        if (saveFiles.Length != 0)
            return;

        _loadGameButton.SetEnabled(false);
        _loadGameButton.style.backgroundColor = Color.gray;
    }

    void LoadGame()
    {
        string[] saveFiles = FileManager.LoadALlSaveFiles();
        new LoadGameScreenVisual(_root, saveFiles);
    }

    void Settings()
    {
        new SettingsScreen(_root);
    }

    void ConfirmQuit()
    {
        ConfirmPopUp pop = new ConfirmPopUp();
        pop.Initialize(_root, Quit);
    }

    void Quit()
    {
        Application.Quit();
    }

}
