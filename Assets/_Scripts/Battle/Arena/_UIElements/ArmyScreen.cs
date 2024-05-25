using Lis.Battle.Fight;
using Lis.Core;
using Lis.Units;
using UnityEngine.UIElements;

namespace Lis.Battle.Arena
{
    public class ArmyScreen : FullScreenElement
    {
        readonly HeroManager _heroManager;
        readonly FightManager _fightManager;

        readonly VisualElement _mainContainer;
        ScrollView _playerArmyScrollView;
        ScrollView _enemyArmyScrollView;

        public ArmyScreen()
        {
            _heroManager = HeroManager.Instance;
            _fightManager = FightManager.Instance;

            _mainContainer = new();
            _mainContainer.style.flexDirection = FlexDirection.Row;
            _mainContainer.style.justifyContent = Justify.SpaceAround;
            Content.Add(_mainContainer);

            SetTitle("Army Screen");

            AddPlayerContainer();
            AddEnemyContainer();

            AddPlayerArmyIcons();
            AddEnemyArmyIcons();

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

        void AddEnemyContainer()
        {
            VisualElement container = new();
            _mainContainer.Add(container);

            Label title = new("Enemy Army");
            container.Add(title);

            _enemyArmyScrollView = new();
            container.Add(_enemyArmyScrollView);
        }

        void AddPlayerArmyIcons()
        {
            foreach (Unit u in _heroManager.Hero.Army)
            {
                UnitIcon icon = new(u);
                _playerArmyScrollView.Add(icon);
            }
        }

        void AddEnemyArmyIcons()
        {
            if (_fightManager.CurrentFight.ChosenOption == null) return;
            foreach (Unit u in _fightManager.CurrentFight.ChosenOption.Army)
            {
                UnitIcon icon = new(u);
                _enemyArmyScrollView.Add(icon);
            }
        }

        void AddPlayerArmyCards()
        {
            foreach (Unit u in _heroManager.Hero.Army)
            {
                UnitCard card = new(u);
                _playerArmyScrollView.Add(card);
            }
        }

        void AddEnemyArmyCards()
        {
            if (_fightManager.CurrentFight.ChosenOption == null) return;
            foreach (Unit u in _fightManager.CurrentFight.ChosenOption.Army)
            {
                UnitCard card = new(u);
                _enemyArmyScrollView.Add(card);
            }
        }

        bool _isCardView;

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
            {
                AddPlayerArmyCards();
                AddEnemyArmyCards();
            }
            else
            {
                AddPlayerArmyIcons();
                AddEnemyArmyIcons();
            }

            _isCardView = !_isCardView;
        }
    }
}