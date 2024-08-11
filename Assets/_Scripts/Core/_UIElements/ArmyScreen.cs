using Lis.Arena;
using Lis.Core;
using Lis.Units;
using UnityEngine.UIElements;

namespace Lis.Camp.Building
{
    public class ArmyScreen : FullScreenElement
    {
        GameManager _gameManager;

        readonly VisualElement _mainContainer;
        readonly UnitCardFactory _unitCardFactory;
        ScrollView _enemyArmyScrollView;

        bool _isCardView;
        ScrollView _playerArmyScrollView;

        public ArmyScreen()
        {
            _gameManager = GameManager.Instance;
            _unitCardFactory = UnitCardFactory.Instance;

            _mainContainer = new();
            _mainContainer.style.flexDirection = FlexDirection.Row;
            _mainContainer.style.justifyContent = Justify.SpaceAround;
            Content.Add(_mainContainer);

            SetTitle("Army Screen");

            AddPlayerContainer();

            AddPlayerArmyIcons();

            AddChangeViewButton();
            AddContinueButton();
        }

        void AddPlayerContainer()
        {
            VisualElement container = new();
            _mainContainer.Add(container);

            Label title = new("Player Army");
            container.Add(title);

            _playerArmyScrollView = new();
            container.Add(_playerArmyScrollView);
        }

        void AddPlayerArmyIcons()
        {
            foreach (Unit u in _gameManager.Campaign.Hero.Army)
            {
                UnitIcon icon = new(u);
                _playerArmyScrollView.Add(icon);
            }
        }


        void AddPlayerArmyCards()
        {
            foreach (Unit u in _gameManager.Campaign.Hero.Army)
                _playerArmyScrollView.Add(_unitCardFactory.CreateUnitCard(u));
        }


        void AddChangeViewButton()
        {
            MyButton changeViewButton = new("Change View", USSCommonButton, ChangeView);
            UtilityContainer.Add(changeViewButton);
        }

        void ChangeView()
        {
            _playerArmyScrollView.Clear();
            _enemyArmyScrollView.Clear();

            if (!_isCardView)
                AddPlayerArmyCards();
            else
                AddPlayerArmyIcons();

            _isCardView = !_isCardView;
        }
    }
}