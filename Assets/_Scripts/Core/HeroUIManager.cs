using Lis.Camp.Building;
using Lis.Core;
using Lis.Units.Hero;
using UnityEngine;
using UnityEngine.UIElements;

namespace Lis.Arena
{
    public class HeroUIManager : MonoBehaviour
    {
        GameManager _gameManager;

        VisualElement _root;
        MyButton _viewArmyButton;

        MyButton _viewHeroButton;

        public void Start()
        {
            _gameManager = GameManager.Instance;
            _root = GetComponent<UIDocument>().rootVisualElement;

            AddHeroUI();
        }

        void AddHeroUI()
        {
            VisualElement container = new();
            container.style.flexDirection = FlexDirection.Row;
            container.style.position = Position.Absolute;
            container.style.alignItems = Align.Center;
            container.style.right = 0;
            container.style.top = 0;
            container.style.backgroundColor = new(new Color(0, 0, 0, 0.5f));
            _root.Add(container);

            GoldElement goldElement = new(_gameManager.Gold);
            _gameManager.OnGoldChanged += goldElement.ChangeAmount;
            container.Add(goldElement);

            _viewHeroButton = new("", "common__hero-button", ViewHero);
            container.Add(_viewHeroButton);

            _viewArmyButton = new("", "common__army-button", ViewArmy);
            container.Add(_viewArmyButton);
        }

        void ViewHero()
        {
            HeroScreen heroScreen = new(_gameManager.Campaign.Hero);
            heroScreen.Initialize();
            _viewHeroButton.SetEnabled(false);
            heroScreen.OnHide += () => _viewHeroButton.SetEnabled(true);
        }

        void ViewArmy()
        {
            ArmyScreen ass = new();
            _viewArmyButton.SetEnabled(false);
            ass.OnHide += () => _viewArmyButton.SetEnabled(true);
        }
    }
}