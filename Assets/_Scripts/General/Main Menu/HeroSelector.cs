using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Lis
{
    public class HeroSelector : MonoBehaviour
    {
        GameManager _gameManager;
        MainMenu _mainMenu;

        [SerializeField] GameObject _heroContainer;
        [SerializeField] Hero[] _heroes;
        readonly List<HeroBoxManager> _heroPrefabs = new();
        int _currentIndex;

        VisualElement _heroInfoContainer;
        MyButton _previousButton;
        MyButton _nextButton;

        void Start()
        {
            _gameManager = GameManager.Instance;
            _mainMenu = MainMenu.Instance;

            SetUpUI();
            InstantiateHeroes();

            _currentIndex = 0;
            SetHeroActive(_currentIndex);
        }

        void InstantiateHeroes()
        {
            Vector3 pos = new(1f, -0.07f, -7.5f);
            foreach (Hero hero in _heroes)
            {
                HeroBoxManager h =
                    Instantiate(hero.SelectorPrefab, pos, Quaternion.identity).GetComponent<HeroBoxManager>();

                h.gameObject.SetActive(false);
                _heroPrefabs.Add(h);
            }
        }

        void SetUpUI()
        {
            VisualElement root = _mainMenu.Root;

            _heroInfoContainer = root.Q<VisualElement>("heroInfoContainer");

            VisualElement arrowContainer = root.Q<VisualElement>("arrowContainer");
            _previousButton = new("<", null, ShowPreviousHero);
            _nextButton = new(">", null, ShowNextHero);
            arrowContainer.Add(_previousButton);
            arrowContainer.Add(_nextButton);
        }

        void ShowNextHero()
        {
            _heroPrefabs[_currentIndex].Hide();
            _currentIndex++;
            if (_currentIndex >= _heroPrefabs.Count)
                _currentIndex = 0;
            SetHeroActive(_currentIndex);
        }

        void ShowPreviousHero()
        {
            _heroPrefabs[_currentIndex].Hide();
            _currentIndex--;
            if (_currentIndex < 0)
                _currentIndex = _heroPrefabs.Count - 1;
            SetHeroActive(_currentIndex);
        }

        void SetHeroActive(int index)
        {
            _heroInfoContainer.Clear();
            //_heroInfoContainer.Add(new HeroInfo(_heroes[index]));
            _heroInfoContainer.Add(new Label($"{_heroes[index].EntityName}"));
            _heroPrefabs[index].Show();
            _gameManager.SelectedHero = _heroes[index];
        }
    }
}