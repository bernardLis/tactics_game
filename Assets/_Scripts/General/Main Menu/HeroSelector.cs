using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Lis
{
    public class HeroSelector : MonoBehaviour
    {
        const string _ussCommonMenuButton = "common__menu-button";
        const string _ussCommonButtonArrow = "common__button-arrow";

        const string _ussMainMenuArrowButton = "main-menu__arrow-button";

        GameManager _gameManager;
        MainMenu _mainMenu;

        [SerializeField] Hero[] _heroes;
        readonly List<HeroSelectorManager> _heroPrefabs = new();
        int _currentIndex;

        VisualElement _heroInfoContainer;
        MyButton _previousButton;
        MyButton _nextButton;

        void Start()
        {
            _mainMenu = MainMenu.Instance;
            _gameManager = GameManager.Instance;
            SetUpUI();
            InstantiateHeroes();

            _currentIndex = 0;
            ShowHero(_currentIndex);
        }

        void InstantiateHeroes()
        {
            // Vector3 pos = new(1f, -0.07f, -7.5f);
            foreach (Hero hero in _heroes)
            {
                HeroSelectorManager h =
                    Instantiate(hero.SelectorPrefab, transform).GetComponent<HeroSelectorManager>();
                h.transform.localPosition = Vector3.up * 6.93f;
                h.Initialize();
                _heroPrefabs.Add(h);
            }
        }

        void SetUpUI()
        {
            VisualElement root = _mainMenu.Root;

            _heroInfoContainer = root.Q<VisualElement>("heroInfoContainer");

            VisualElement arrowContainer = root.Q<VisualElement>("arrowContainer");
            _previousButton = new("<", _ussMainMenuArrowButton, ShowPreviousHero);
            _nextButton = new(">", _ussMainMenuArrowButton, ShowNextHero);
            arrowContainer.Add(_previousButton);
            arrowContainer.Add(_nextButton);
        }

        void ShowNextHero()
        {
            _heroPrefabs[_currentIndex].Hide();
            _currentIndex++;
            if (_currentIndex >= _heroPrefabs.Count)
                _currentIndex = 0;
            ShowHero(_currentIndex);
        }

        void ShowPreviousHero()
        {
            _heroPrefabs[_currentIndex].Hide();
            _currentIndex--;
            if (_currentIndex < 0)
                _currentIndex = _heroPrefabs.Count - 1;
            ShowHero(_currentIndex);
        }

        void ShowHero(int index)
        {
            _heroInfoContainer.Clear();
            _heroInfoContainer.Add(new HeroSelectorInfoElement(_heroes[index]));
            _heroPrefabs[index].Show();
            _gameManager.SelectedHero = _heroes[index];
        }
    }
}