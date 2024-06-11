using System;
using System.Collections;
using Lis.Core;
using Lis.Core.Utilities;
using Lis.MainMenu._UIElements;
using Lis.Upgrades;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

namespace Lis.MainMenu
{
    public class MainMenu : Singleton<MainMenu>
    {
        const string _ussCommonButton = "common__button";

        [SerializeField] Sound _mainMenuTheme;
        [SerializeField] GameObject _gameManagerPrefab;
        [SerializeField] StyleSheet[] _themeStyleSheets;
        [SerializeField] ParticleSystem _particleSystem;

        GameManager _gameManager;
        UpgradeManager _globalUpgradeManager;

        VisualElement _root;
        MyButton _playButton;
        MyButton _statsButton;
        MyButton _settingsButton;
        MyButton _quitButton;


        protected override void Awake()
        {
            base.Awake();
            // TODO: is this a good idea? When game manager was in the scene there was a bug when you were coming back to main menu.
            _gameManager = GameManager.Instance;
            if (_gameManager == null)
                _gameManager = Instantiate(_gameManagerPrefab).GetComponent<GameManager>();

            _globalUpgradeManager = GetComponent<UpgradeManager>();

            _root = GetComponent<UIDocument>().rootVisualElement;
            SetThemeStyleSheet();
        }

        void SetThemeStyleSheet()
        {
            StyleSheet chosen = _themeStyleSheets[Random.Range(0, _themeStyleSheets.Length)];
            _root.styleSheets.Add(chosen);
            _gameManager.Root.styleSheets.Add(chosen);
            StartCoroutine(SetParticleSystemColorCoroutine());
        }

        IEnumerator SetParticleSystemColorCoroutine()
        {
            yield return new WaitForSeconds(1f);
            Color c = _root.Q<Label>("gameTitle").resolvedStyle.color;

            // set particle system start color as the main color of the theme
            ParticleSystem.MainModule main = _particleSystem.main;
            main.startColor = c;
        }


        void Start()
        {
            //    _playButton = new("Play", _ussCommonButton, ShowGlobalUpgradesMenu);
            _playButton = new("Play", _ussCommonButton,
                () => GameManager.Instance.LoadScene(Scenes.HeroSelection));

            _statsButton = new("Stats", _ussCommonButton, ShowStatsScreen);
            _settingsButton = new("Settings", _ussCommonButton, Settings);
            _quitButton = new("Quit", _ussCommonButton, ConfirmQuit);

            VisualElement buttonContainer = _root.Q<VisualElement>("buttonContainer");
            buttonContainer.Add(_playButton);
            buttonContainer.Add(_statsButton);
            buttonContainer.Add(_settingsButton);
            buttonContainer.Add(_quitButton);

            AudioManager.Instance.PlayMusic(_mainMenuTheme);
        }

        void ShowGlobalUpgradesMenu()
        {
            _globalUpgradeManager.ShowUpgradeMenu();
        }

        void ShowStatsScreen()
        {
            StatsScreen statsScreen = new(_gameManager.GameStats);
            if (statsScreen == null) throw new ArgumentNullException(nameof(statsScreen));
        }

        void Settings()
        {
            SettingsScreen settingsScreen = new();
            if (settingsScreen == null) throw new ArgumentNullException(nameof(settingsScreen));
        }

        void ConfirmQuit()
        {
            ConfirmPopUp pop = new();
            pop.Initialize(_root, Quit);
        }

        void Quit()
        {
            Application.Quit();
        }
    }
}