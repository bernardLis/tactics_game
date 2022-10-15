using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] Sound _mainMenuTheme;

    GameManager _gameManager;

    VisualElement _root;
    MyButton _continueButton;
    MyButton _startNewGameButton;
    MyButton _settingsButton;
    MyButton _quitButton;

    VisualElement _menuContainer;
    VisualElement _settingsContainer;

    void Start()
    {
        _gameManager = GameManager.Instance;

        _root = GetComponent<UIDocument>().rootVisualElement;
        _continueButton = new MyButton("Play", "menuButton", Continue);
        _settingsButton = new MyButton("Settings", "menuButton", Settings);
        _quitButton = new MyButton("Quit", "menuButton", ConfirmQuit);

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

    void Settings() { new SettingsScreen(_root, _root); }

    void ConfirmQuit()
    {
        ConfirmPopUp pop = new ConfirmPopUp();
        pop.Initialize(_root, Quit);
    }

    void Quit() { Application.Quit(); }

}
