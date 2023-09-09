using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] Sound _mainMenuTheme;
    [SerializeField] GameObject _gameManagerPrefab;

    GameManager _gameManager;

    VisualElement _root;
    MyButton _continueButton;
    MyButton _startNewGameButton;
    MyButton _settingsButton;
    MyButton _quitButton;

    VisualElement _menuContainer;
    VisualElement _settingsContainer;

    const string _ussCommonMenuButton = "common__menu-button";


    void Awake()
    {
        // TODO: is this a good idea? When game manager was in the scene there was a bug when you were coming back to main menu.
        _gameManager = GameManager.Instance;
        if (_gameManager == null)
            _gameManager = Instantiate(_gameManagerPrefab).GetComponent<GameManager>();
    }

    void Start()
    {
        _root = GetComponent<UIDocument>().rootVisualElement;
        _continueButton = new MyButton("Play", _ussCommonMenuButton, Continue);
        _settingsButton = new MyButton("Settings", _ussCommonMenuButton, Settings);
        _quitButton = new MyButton("Quit", _ussCommonMenuButton, ConfirmQuit);

        _menuContainer = _root.Q<VisualElement>("menuContainer");
        _menuContainer.Add(_continueButton);
        _menuContainer.Add(_startNewGameButton);
        _menuContainer.Add(_settingsButton);
        _menuContainer.Add(_quitButton);

        _settingsContainer = _root.Q<VisualElement>("settingsContainer");

        AudioManager.Instance.PlayMusic(_mainMenuTheme);
    }

    void Continue()
    {
        _gameManager.Play();
    }

    void Settings() { new SettingsScreen(); }

    void ConfirmQuit()
    {
        ConfirmPopUp pop = new();
        pop.Initialize(_root, Quit);
    }

    void Quit() { Application.Quit(); }

}
