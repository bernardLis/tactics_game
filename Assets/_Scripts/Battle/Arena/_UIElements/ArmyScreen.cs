using System.Collections.Generic;
using Lis.Battle.Fight;
using Lis.Core;
using Lis.Units;
using UnityEngine;
using UnityEngine.UIElements;

namespace Lis.Battle.Arena
{
    public class ArmyScreen : FullScreenElement
    {
        readonly FightManager _fightManager;
        readonly HeroManager _heroManager;

        readonly VisualElement _mainContainer;
        readonly UnitCardFactory _unitCardFactory;
        ScrollView _enemyArmyScrollView;

        bool _isCardView;
        ScrollView _playerArmyScrollView;

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
            _enemyArmyScrollView.Add(
                new Label($"Number of waves: {_fightManager.CurrentFight.ChosenOption.NumberOfWaves}"));

            List<VisualElement> aggregated =
                new(AggregateEnemiesIcons(_fightManager.CurrentFight.ChosenOption.ArmyPerWave));
            foreach (VisualElement el in aggregated)
                _enemyArmyScrollView.Add(el);
        }

        List<VisualElement> AggregateEnemiesIcons(Dictionary<string, int> army)
        {
            List<VisualElement> result = new();
            foreach (var item in army)
            {
                VisualElement container = new();
                container.style.flexDirection = FlexDirection.Row;
                result.Add(container);

                Label count = new(item.Value + "x");
                container.Add(count);

                Unit unit = GameManager.UnitDatabase.GetUnitById(item.Key);
                Unit instance = ScriptableObject.Instantiate(unit);
                instance.InitializeBattle(1);
                UnitIcon icon = new(instance);
                container.Add(icon);
            }

            return result;
        }


        void AddPlayerArmyCards()
        {
            foreach (Unit u in _heroManager.Hero.Army)
                _playerArmyScrollView.Add(_unitCardFactory.CreateUnitCard(u));
        }

        void AddEnemyArmyCards()
        {
            if (_fightManager.CurrentFight.ChosenOption == null) return;
            _enemyArmyScrollView.Add(
                new Label($"Number of waves: {_fightManager.CurrentFight.ChosenOption.NumberOfWaves}"));

            List<VisualElement> aggregated =
                new(AggregateEnemiesCards(_fightManager.CurrentFight.ChosenOption.ArmyPerWave));
            foreach (VisualElement el in aggregated)
                _enemyArmyScrollView.Add(el);
        }

        List<VisualElement> AggregateEnemiesCards(Dictionary<string, int> army)
        {
            List<VisualElement> result = new();
            foreach (var item in army)
            {
                VisualElement container = new();
                container.style.flexDirection = FlexDirection.Row;
                result.Add(container);

                Label count = new(item.Value + "x");
                container.Add(count);

                Unit unit = GameManager.UnitDatabase.GetUnitById(item.Key);
                Unit instance = ScriptableObject.Instantiate(unit);
                instance.InitializeBattle(1);
                UnitCard card = new(instance);
                container.Add(card);
            }

            return result;
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