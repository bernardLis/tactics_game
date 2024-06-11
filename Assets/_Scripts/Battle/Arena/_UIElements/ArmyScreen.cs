﻿using Lis.Battle.Fight;
using Lis.Core;
using Lis.Units;
using UnityEngine.UIElements;

namespace Lis.Battle.Arena
{
    public class ArmyScreen : FullScreenElement
    {
        private readonly FightManager _fightManager;
        private readonly HeroManager _heroManager;

        private readonly VisualElement _mainContainer;
        private readonly UnitCardFactory _unitCardFactory;
        private ScrollView _enemyArmyScrollView;

        private bool _isCardView;
        private ScrollView _playerArmyScrollView;

        public ArmyScreen()
        {
            _heroManager = HeroManager.Instance;
            _fightManager = FightManager.Instance;
            _unitCardFactory = UnitCardFactory.Instance;

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

        private void AddPlayerContainer()
        {
            VisualElement container = new();
            _mainContainer.Add(container);

            Label title = new("Player Army");
            container.Add(title);

            _playerArmyScrollView = new();
            container.Add(_playerArmyScrollView);
        }

        private void AddEnemyContainer()
        {
            VisualElement container = new();
            _mainContainer.Add(container);

            Label title = new("Enemy Army");
            container.Add(title);

            _enemyArmyScrollView = new();
            container.Add(_enemyArmyScrollView);
        }

        private void AddPlayerArmyIcons()
        {
            foreach (Unit u in _heroManager.Hero.Army)
            {
                UnitIcon icon = new(u);
                _playerArmyScrollView.Add(icon);
            }
        }

        private void AddEnemyArmyIcons()
        {
            if (_fightManager.CurrentFight.ChosenOption == null) return;
            foreach (Unit u in _fightManager.CurrentFight.ChosenOption.Army)
            {
                UnitIcon icon = new(u);
                _enemyArmyScrollView.Add(icon);
            }
        }

        private void AddPlayerArmyCards()
        {
            foreach (Unit u in _heroManager.Hero.Army)
                _playerArmyScrollView.Add(_unitCardFactory.CreateUnitCard(u));
        }

        private void AddEnemyArmyCards()
        {
            if (_fightManager.CurrentFight.ChosenOption == null) return;
            foreach (Unit u in _fightManager.CurrentFight.ChosenOption.Army)
                _enemyArmyScrollView.Add(_unitCardFactory.CreateUnitCard(u));
        }

        private void AddChangeViewButton()
        {
            MyButton changeViewButton = new("Change View", USSCommonButton, ChangeView);
            UtilityContainer.Add(changeViewButton);
        }

        private void ChangeView()
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