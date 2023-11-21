using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;

public class MainMenu : Singleton<MainMenu>
{
    const string _ussCommonMenuButton = "common__menu-button";

    [SerializeField] Sound _mainMenuTheme;
    [SerializeField] GameObject _gameManagerPrefab;

    GameManager _gameManager;
    GlobalUpgradeManager _globalUpgradeManager;

    public VisualElement Root;
    MyButton _playButton;
    MyButton _settingsButton;
    MyButton _quitButton;

    VisualElement _menuContainer;
    VisualElement _settingsContainer;


    protected override void Awake()
    {
        base.Awake();
        // TODO: is this a good idea? When game manager was in the scene there was a bug when you were coming back to main menu.
        _gameManager = GameManager.Instance;
        if (_gameManager == null)
            _gameManager = Instantiate(_gameManagerPrefab).GetComponent<GameManager>();

        _globalUpgradeManager = GetComponent<GlobalUpgradeManager>();

        Root = GetComponent<UIDocument>().rootVisualElement;
    }

    void Start()
    {
        _playButton = new MyButton("Play", _ussCommonMenuButton, ShowGlobalUpgradesMenu);
        _settingsButton = new MyButton("Settings", _ussCommonMenuButton, Settings);
        _quitButton = new MyButton("Quit", _ussCommonMenuButton, ConfirmQuit);

        _menuContainer = Root.Q<VisualElement>("menuContainer");
        _menuContainer.Add(_playButton);
        _menuContainer.Add(_settingsButton);
        _menuContainer.Add(_quitButton);

        _settingsContainer = Root.Q<VisualElement>("settingsContainer");

        AudioManager.Instance.PlayMusic(_mainMenuTheme);
    }

    void ShowGlobalUpgradesMenu()
    {
        _globalUpgradeManager.ShowGlobalUpgradesMenu();
        // _gameManager.Play();
    }

    void Settings() { new SettingsScreen(); }

    void ConfirmQuit()
    {
        ConfirmPopUp pop = new();
        pop.Initialize(Root, Quit);
    }

    void Quit() { Application.Quit(); }

}
