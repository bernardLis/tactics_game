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
    MyButton _globalUpgradeShopButton;
    MyButton _settingsButton;
    MyButton _quitButton;

    VisualElement _menuContainer;
    VisualElement _shopContainer;
    VisualElement _settingsContainer;

    Label _menuObolsLabel;

    void Start()
    {
        _gameManager = GameManager.Instance;

        _root = GetComponent<UIDocument>().rootVisualElement;
        _continueButton = new MyButton("Continue", "menuButton", Continue);
        ResolveContinueButton();
        _startNewGameButton = new MyButton("New Run", "menuButton", StartNewGame);
        _globalUpgradeShopButton = new MyButton("Spend Obols", "menuButton", OpenShop);
        _settingsButton = new MyButton("Settings", "menuButton", Settings);
        _quitButton = new MyButton("Quit", "menuButton", ConfirmQuit);

        _menuContainer = _root.Q<VisualElement>("menuContainer");
        _menuContainer.Add(_continueButton);
        _menuContainer.Add(_startNewGameButton);
        _menuContainer.Add(_globalUpgradeShopButton);
        _menuContainer.Add(_settingsButton);
        _menuContainer.Add(_quitButton);

        _shopContainer = _root.Q<VisualElement>("shopContainer");
        _settingsContainer = _root.Q<VisualElement>("settingsContainer");

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
        if (!_gameManager.IsRunActive())
            _continueButton.SetEnabled(false);
    }

    void Continue()
    {
        _gameManager.ResumeLastRun();
    }

    void StartNewGame()
    {
        if (!_gameManager.IsRunActive())
        {
            _gameManager.StartNewRun();
            return;
        }

        ConfirmPopUp popUp = new ConfirmPopUp();
        popUp.Initialize(_root, () => _gameManager.StartNewRun(), "Are you sure? It will clear run that is in progress.");
    }

    void OpenShop()
    {
        // TODO: I could make a transition with dotween
        _menuContainer.style.display = DisplayStyle.None;
        _shopContainer.style.display = DisplayStyle.Flex;
        GetComponent<GlobalUpgradeShopManager>().SetupShop();
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
