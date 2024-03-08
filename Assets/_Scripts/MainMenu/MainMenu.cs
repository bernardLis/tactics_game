using Lis.Core;
using Lis.Core.Utilities;
using Lis.Upgrades;
using UnityEngine;
using UnityEngine.UIElements;

namespace Lis.MainMenu
{
    public class MainMenu : Singleton<MainMenu>
    {
        const string _ussCommonMenuButton = "common__menu-button";

        [SerializeField] Sound _mainMenuTheme;
        [SerializeField] GameObject _gameManagerPrefab;

        GameManager _gameManager;
        UpgradeManager _globalUpgradeManager;

        public VisualElement Root;
        MyButton _playButton;
        MyButton _settingsButton;
        MyButton _quitButton;

        VisualElement _menuContainer;

        protected override void Awake()
        {
            base.Awake();
            // TODO: is this a good idea? When game manager was in the scene there was a bug when you were coming back to main menu.
            _gameManager = GameManager.Instance;
            if (_gameManager == null)
                _gameManager = Instantiate(_gameManagerPrefab).GetComponent<GameManager>();

            _globalUpgradeManager = GetComponent<UpgradeManager>();

            Root = GetComponent<UIDocument>().rootVisualElement;
        }

        void Start()
        {
            _playButton = new("Play", _ussCommonMenuButton, ShowGlobalUpgradesMenu);
            _settingsButton = new("Settings", _ussCommonMenuButton, Settings);
            _quitButton = new("Quit", _ussCommonMenuButton, ConfirmQuit);

            _menuContainer = Root.Q<VisualElement>("menuContainer");
            _menuContainer.Add(_playButton);
            _menuContainer.Add(_settingsButton);
            _menuContainer.Add(_quitButton);

            VisualElement statsContainer = Root.Q<VisualElement>("gameStatsContainer");
            GameStatsElement stats = new(_gameManager.GameStats);
            Debug.Log($"showing stats: {_gameManager.GameStats.GetInstanceID()}");
            statsContainer.Add(stats);

            AudioManager.Instance.PlayMusic(_mainMenuTheme);
        }

        void ShowGlobalUpgradesMenu()
        {
            _globalUpgradeManager.ShowUpgradeMenu();
        }

        void Settings()
        {
            new SettingsScreen();
        }

        void ConfirmQuit()
        {
            ConfirmPopUp pop = new();
            pop.Initialize(Root, Quit);
        }

        void Quit()
        {
            Application.Quit();
        }
    }
}