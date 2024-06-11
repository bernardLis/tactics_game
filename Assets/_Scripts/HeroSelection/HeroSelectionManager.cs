using System.Collections.Generic;
using Lis.Core;
using Lis.Core.Utilities;
using Lis.HeroSelection._UIElements;
using Lis.Units.Hero;
using UnityEngine;
using UnityEngine.UIElements;

namespace Lis.HeroSelection
{
    public class HeroSelectionManager : MonoBehaviour
    {
        private const string _ussCommonButton = "common__button";
        private const string _ussMainMenuArrowButton = "hero-selection__arrow-button";

        [SerializeField] private StyleSheet[] _themeStyleSheets;

        [SerializeField] private Sound _music;
        private readonly List<Hero> _heroInstances = new();
        private readonly List<HeroDisplayer> _heroPrefabs = new();
        private MyButton _backButton;
        private int _currentIndex;

        private GameManager _gameManager;

        private VisualElement _heroInfoContainer;
        private MyButton _nextButton;
        private MyButton _playButton;
        private MyButton _previousButton;

        private void Start()
        {
            _gameManager = GameManager.Instance;
            AudioManager.Instance.PlayMusic(_music);
            SetUpUI();
            InstantiateHeroes();

            _currentIndex = 0;
            ShowHero(_currentIndex);
        }

        private void InstantiateHeroes()
        {
            foreach (Hero hero in _gameManager.UnitDatabase.Heroes)
            {
                Hero instance = Instantiate(hero);
                instance.InitializeHero();
                _heroInstances.Add(instance);

                HeroDisplayer h =
                    Instantiate(hero.SelectorPrefab, transform).GetComponent<HeroDisplayer>();
                h.transform.localPosition = Vector3.up * 6.93f;
                h.Initialize();
                _heroPrefabs.Add(h);
            }
        }

        private void SetUpUI()
        {
            VisualElement root = GetComponent<UIDocument>().rootVisualElement;

            _heroInfoContainer = root.Q<VisualElement>("heroInfoContainer");

            VisualElement arrowContainer = root.Q<VisualElement>("arrowContainer");
            _previousButton = new("<", _ussCommonButton, ShowPreviousHero);
            _nextButton = new(">", _ussCommonButton, ShowNextHero);
            _previousButton.style.width = 150;
            _nextButton.style.width = 150;

            arrowContainer.Add(_previousButton);
            arrowContainer.Add(_nextButton);

            VisualElement playButtonContainer = root.Q<VisualElement>("playButtonContainer");
            _playButton = new("Play", _ussCommonButton, Play);
            playButtonContainer.Add(_playButton);

            VisualElement backButtonContainer = root.Q<VisualElement>("backButtonContainer");
            _backButton = new("Back", _ussCommonButton, Back);
            backButtonContainer.Add(_backButton);
        }

        private void ShowNextHero()
        {
            _heroPrefabs[_currentIndex].Hide();
            _currentIndex++;
            if (_currentIndex >= _heroPrefabs.Count)
                _currentIndex = 0;
            ShowHero(_currentIndex);
        }

        private void ShowPreviousHero()
        {
            _heroPrefabs[_currentIndex].Hide();
            _currentIndex--;
            if (_currentIndex < 0)
                _currentIndex = _heroPrefabs.Count - 1;
            ShowHero(_currentIndex);
        }

        private void ShowHero(int index)
        {
            _previousButton.SetEnabled(false);
            _nextButton.SetEnabled(false);

            _heroInfoContainer.Clear();
            _heroInfoContainer.Add(new HeroInfoElement(_heroInstances[index]));
            _heroPrefabs[index].Show();
            _gameManager.SetHero(_heroInstances[index]);

            SetButtonColors(_heroInstances[index].Nature);
            Invoke(nameof(EnableArrows), 0.5f);
        }

        private void SetButtonColors(Nature n)
        {
            Color c = n.Color.Primary;
            if (n.NatureName == NatureName.Earth) c = n.Color.Secondary;
            _previousButton.style.backgroundColor = c;
            _nextButton.style.backgroundColor = c;
            _playButton.style.backgroundColor = c;
            _backButton.style.backgroundColor = c;
        }

        private void EnableArrows()
        {
            _previousButton.SetEnabled(true);
            _nextButton.SetEnabled(true);
        }

        private void Play()
        {
            _gameManager.StartGame();
        }

        private void Back()
        {
            _gameManager.LoadScene(Scenes.MainMenu);
        }
    }
}