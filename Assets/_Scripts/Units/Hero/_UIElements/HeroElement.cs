using System.Collections.Generic;
using Lis.Core;
using Lis.Units.Creature;
using Lis.Units.Hero.Ability;
using Lis.Units.Hero.Tablets;
using UnityEngine;
using UnityEngine.UIElements;

namespace Lis.Units.Hero
{
    public class HeroElement : VisualElement
    {
        const string _ussCommonTextPrimary = "common__text-primary";

        const string _ussClassName = "hero-element__";
        const string _ussMain = _ussClassName + "main";

        const string _ussBIcon = _ussClassName + "b-icon";
        const string _ussQIcon = _ussClassName + "q-icon";

        const string _ussTeamContainer = _ussClassName + "team-container";
        const string _ussInfoContainer = _ussClassName + "info-container";
        const string _ussStatContainer = _ussClassName + "stat-container";
        const string _ussTabletContainer = _ussClassName + "tablet-container";
        const string _ussSlot = _ussClassName + "slot";

        readonly GameManager _gameManager;

        readonly Hero _hero;

        VisualElement _topContainer;
        VisualElement _teamContainer;
        VisualElement _buttonGuideContainer;

        VisualElement _resourcesContainer;
        TroopsCountElement _troopsCounter;

        readonly VisualElement _heroInfoContainer;
        ResourceBarElement _expBar;
        Label _levelLabel;

        readonly bool _isAdvancedView;
        VisualElement _advancedViewContainer;

        public HeroElement(Hero hero, bool isAdvanced = false)
        {
            _gameManager = GameManager.Instance;
            StyleSheet ss = _gameManager.GetComponent<AddressableManager>()
                .GetStyleSheetByName(StyleSheetType.HeroElementStyles);
            if (ss != null) styleSheets.Add(ss);
            AddToClassList(_ussCommonTextPrimary);

            _hero = hero;
            hero.OnLevelUp += OnHeroLevelUp;
            AddToClassList(_ussMain);

            CreateTopContainer();

            _heroInfoContainer = new();
            _heroInfoContainer.AddToClassList(_ussInfoContainer);
            Add(_heroInfoContainer);

            _isAdvancedView = isAdvanced;

            HandleButtonGuide();
            HandleResources();
            HandleTeam();
            HandleAbilities();
            HandleExpBar();

            if (!isAdvanced) return;
            HandleAdvancedView();
        }

        void CreateTopContainer()
        {
            _topContainer = new();
            _topContainer.style.flexDirection = FlexDirection.Row;
            _topContainer.style.justifyContent = Justify.SpaceBetween;
            _topContainer.style.paddingTop = 12;
            _topContainer.style.paddingBottom = 12;
            Add(_topContainer);

            _teamContainer = new();
            _teamContainer.AddToClassList(_ussTeamContainer);
            _topContainer.Add(_teamContainer);

            _resourcesContainer = new();
            _teamContainer.Add(_resourcesContainer);

            _buttonGuideContainer = new();
            _topContainer.Add(_buttonGuideContainer);
        }

        void OnHeroLevelUp()
        {
            _levelLabel.text = $"Level {_hero.Level.Value}";
        }

        void HandleButtonGuide()
        {
            if (_isAdvancedView) return;

            VisualElement containerQ = new();
            containerQ.style.flexDirection = FlexDirection.Row;
            Label qIcon = new();
            qIcon.AddToClassList(_ussQIcon);
            Label qLabel = new("Throw Friend Ball");
            containerQ.Add(qIcon);
            containerQ.Add(qLabel);

            VisualElement containerB = new();
            containerB.style.flexDirection = FlexDirection.Row;
            Label bIcon = new();
            bIcon.AddToClassList(_ussBIcon);
            Label bLabel = new("Recall Troops");
            containerB.Add(bIcon);
            containerB.Add(bLabel);

            _buttonGuideContainer.Add(containerQ);
            _buttonGuideContainer.Add(containerB);
        }

        void HandleResources()
        {
            AddFriendBallCountElement();
            AddTroopsCountElement();
        }

        void AddFriendBallCountElement()
        {
            VisualElement container = new();
            container.style.flexDirection = FlexDirection.Row;

            Label icon = new();
            icon.style.backgroundImage = new(_gameManager.GameDatabase.FriendBallIcon);
            icon.style.width = 25;
            icon.style.height = 25;

            Label friendBallCountLabel = new($"{_hero.NumberOfFriendBalls}");
            friendBallCountLabel.AddToClassList(_ussCommonTextPrimary);
            _hero.OnFriendBallCountChanged +=
                () => friendBallCountLabel.text = $"{_hero.NumberOfFriendBalls}";

            container.Add(icon);
            container.Add(friendBallCountLabel);
            _resourcesContainer.Add(container);
        }

        void AddTroopsCountElement()
        {
            _troopsCounter = new("");
            _resourcesContainer.Add(_troopsCounter);
            _hero.OnTroopMemberAdded += (_) => UpdateTroopsCountElement();
            _hero.TroopsLimit.OnValueChanged += (_) => UpdateTroopsCountElement();

            UpdateTroopsCountElement();
        }

        void UpdateTroopsCountElement()
        {
            _troopsCounter.UpdateCountContainer($"{_hero.Troops.Count}/{_hero.TroopsLimit.Value}",
                Color.white);
        }

        void HandleTeam()
        {
            foreach (Creature.Creature c in _hero.Troops)
            {
                CreatureIcon icon = new(c);
                _teamContainer.Add(icon);
            }

            _hero.OnTroopMemberAdded += (c) =>
            {
                CreatureIcon icon = new(c);
                _teamContainer.Add(icon);
            };
        }

        void HandleAbilities()
        {
            if (_isAdvancedView) return;

            VisualElement container = new();
            container.style.flexDirection = FlexDirection.Row;
            container.style.alignItems = Align.Center;
            _heroInfoContainer.Add(container);

            foreach (Ability.Ability ability in _hero.GetAllAbilities())
            {
                AbilityElement icon = new(ability, true);
                container.Add(icon);
            }

            _hero.OnAbilityAdded += (a) =>
            {
                AbilityElement icon = new(a, true);
                container.Add(icon);
            };
        }

        void HandleExpBar()
        {
            Color c = GameManager.Instance.GameDatabase.GetColorByName("Experience").Primary;
            _expBar = new(c, "Experience", _hero.Experience, _hero.ExpForNextLevel);

            _levelLabel = new($"Level {_hero.Level.Value}");
            _levelLabel.style.position = Position.Absolute;
            _levelLabel.AddToClassList(_ussCommonTextPrimary);
            _levelLabel.style.fontSize = 46;

            _expBar.style.height = 50;
            _expBar.Add(_levelLabel);

            Add(_expBar);
        }

        void HandleAdvancedView()
        {
            style.backgroundColor = Color.black;
            style.bottom = 0;

            HandleAdvancedAbilities();
            HandleTablets();
            HandleStats();
        }

        void HandleAdvancedAbilities()
        {
            VisualElement container = new();
            container.style.flexDirection = FlexDirection.Row;
            container.style.alignItems = Align.Center;
            container.style.paddingBottom = 12;
            container.style.paddingTop = 12;
            _heroInfoContainer.Add(container);

            foreach (Ability.Ability ability in _hero.GetAllAbilities())
            {
                AbilityElement icon = new(ability, true);
                container.Add(icon);
            }

            List<VisualElement> slots = new();
            for (int i = 0; i < 5 - _hero.GetAllAbilities().Count; i++)
            {
                VisualElement slot = new();
                slot.AddToClassList(_ussSlot);
                container.Add(slot);
                slots.Add(slot);
            }

            if (_hero.AdvancedTablet == null)
                slots[^1].style.backgroundColor = Color.yellow;

            _hero.OnAbilityAdded += (a) =>
            {
                if (slots.Count > 0)
                {
                    container.Remove(slots[0]);
                    slots.RemoveAt(0);
                }

                AbilityElement icon = new(a, true);
                container.Insert(_hero.Abilities.Count - 1, icon);

                if (a.Nature.IsAdvanced && slots.Count > 0)
                    slots[^1].style.backgroundColor = Color.white;
            };
        }

        void HandleTablets()
        {
            VisualElement container = new();
            container.AddToClassList(_ussTabletContainer);
            container.pickingMode = PickingMode.Ignore;
            _heroInfoContainer.Add(container);

            foreach (Tablet t in _hero.Tablets)
            {
                TabletElement tabletElement = new(t, true);
                container.Add(tabletElement);
            }

            if (_hero.AdvancedTablet != null)
            {
                container.Add(new TabletElement(_hero.AdvancedTablet, true));
                return;
            }

            VisualElement slot = new();
            slot.AddToClassList(_ussSlot);
            slot.style.backgroundColor = Color.yellow;
            container.Add(slot);
            _hero.OnTabletAdvancedAdded += AdvancedTabletAdded;
            return;

            void AdvancedTabletAdded(TabletAdvanced tabletAdvanced)
            {
                container.Remove(slot);
                container.Add(new TabletElement(tabletAdvanced, true));
                _hero.OnTabletAdvancedAdded -= AdvancedTabletAdded;
            }
        }

        void HandleStats()
        {
            VisualElement statContainer = new();
            statContainer.AddToClassList(_ussStatContainer);
            _heroInfoContainer.Add(statContainer);

            foreach (Stat s in _hero.GetAllStats())
            {
                StatElement statElement = new(s);
                statContainer.Add(statElement);
            }
        }
    }
}