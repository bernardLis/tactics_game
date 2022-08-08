using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] Sound _mainMenuTheme;

    GameManager _gameManager;

    VisualElement _root;
    Button _continueButton;
    Button _startNewGameButton;
    Button _globalUpgradeShopButton;
    Button _settingsButton;
    Button _quitButton;

    VisualElement _menuContainer;
    VisualElement _newGameScreen;
    VisualElement _shopContainer;
    VisualElement _settingsContainer;

    Label _menuObolsLabel;

    void Start()
    {
        _gameManager = GameManager.Instance;

        _root = GetComponent<UIDocument>().rootVisualElement;
        _continueButton = _root.Q<Button>("continueButton");
        ResolveContinueButton();
        _startNewGameButton = _root.Q<Button>("startNewGameButton");
        _globalUpgradeShopButton = _root.Q<Button>("globalUpgradeShopButton");
        _settingsButton = _root.Q<Button>("settingsButton");
        _quitButton = _root.Q<Button>("quitButton");

        _menuContainer = _root.Q<VisualElement>("menuContainer");
        _newGameScreen = _root.Q<VisualElement>("newGameContainer");
        _shopContainer = _root.Q<VisualElement>("shopContainer");
        _settingsContainer = _root.Q<VisualElement>("settingsContainer");

        _continueButton.clickable.clicked += Continue;
        _startNewGameButton.clickable.clicked += StartNewGame;
        _globalUpgradeShopButton.clickable.clicked += OpenShop;
        _settingsButton.clickable.clicked += Settings;
        _quitButton.clickable.clicked += ConfirmQuit;

        _menuObolsLabel = _root.Q<Label>("menuObolsLabel");
        _menuObolsLabel.text = $"Obols: {_gameManager.Obols}";

        AudioManager.Instance.PlayMusic(_mainMenuTheme);

        _gameManager.OnObolsChanged += OnObolsChanged;

        // HERE: to clean
        _root.Q<Button>("addObol").clickable.clicked += AddObol;
        _root.Q<Button>("removeObol").clickable.clicked += RemoveObol;
    }

    // HERE: to clean
    void AddObol()
    {
        _gameManager.ChangeObolValue(1);
    }
    void RemoveObol()
    {
        _gameManager.ChangeObolValue(-1);
    }

    void OnObolsChanged(int total)
    {
        _menuObolsLabel.text = $"Obols: {total}";
    }

    void ResolveContinueButton()
    {
        // TODO: continue only works if there is a run in progress;

        /*
        string lastSave = PlayerPrefs.GetString("lastSave");
        if (lastSave == null || !FileManager.FileExists(lastSave))
            _continueButton.SetEnabled(false);
        */
    }

    void Continue()
    {
        /*
        string lastSave = PlayerPrefs.GetString("lastSave");

        if (lastSave == null)
            return;
        if (!FileManager.FileExists(lastSave))
            return;
        */
        // _gameManager.StartGameFromSave(lastSave);
    }

    void StartNewGame()
    {
        // TODO: I could make a transition with dotween
        _menuContainer.style.display = DisplayStyle.None;
        _newGameScreen.style.display = DisplayStyle.Flex;
    }

    void OpenShop()
    {
        // TODO: I could make a transition with dotween
        _menuContainer.style.display = DisplayStyle.None;
        _shopContainer.style.display = DisplayStyle.Flex;
    }

    void Settings()
    {
        new SettingsScreen(_root, _root);
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
