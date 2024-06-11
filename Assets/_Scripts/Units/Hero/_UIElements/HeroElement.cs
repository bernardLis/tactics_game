using System.Collections.Generic;
using Lis.Core;
using Lis.Units.Hero.Ability;
using UnityEngine;
using UnityEngine.UIElements;

namespace Lis.Units.Hero
{
    public class HeroElement : VisualElement
    {
        private const string _ussCommonTextPrimary = "common__text-primary";

        private const string _ussClassName = "hero-element__";
        private const string _ussMain = _ussClassName + "main";

        private const string _ussInfoContainer = _ussClassName + "info-container";

        private readonly Hero _hero;

        private readonly VisualElement _heroInfoContainer;

        private readonly List<AbilityElement> _abilityIconsElements = new();

        private VisualElement _advancedViewContainer;
        private ResourceBarElement _expBar;
        private Label _levelLabel;

        private VisualElement _resourcesContainer;
        private TroopsCountElement _troopsCounter;

        public HeroElement(Hero hero)
        {
            GameManager gameManager = GameManager.Instance;
            StyleSheet ss = gameManager.GetComponent<AddressableManager>()
                .GetStyleSheetByName(StyleSheetType.HeroElementStyles);
            if (ss != null) styleSheets.Add(ss);
            AddToClassList(_ussCommonTextPrimary);

            _hero = hero;
            hero.OnLevelUp += OnHeroLevelUp;
            AddToClassList(_ussMain);

            _heroInfoContainer = new();
            _heroInfoContainer.AddToClassList(_ussInfoContainer);
            Add(_heroInfoContainer);

            HandleAbilities();
            HandleExpBar();
        }

        private void OnHeroLevelUp()
        {
            _levelLabel.text = $"Level {_hero.Level.Value}";
        }

        private void HandleAbilities()
        {
            VisualElement container = new();
            container.style.flexDirection = FlexDirection.Row;
            container.style.alignItems = Align.Center;
            _heroInfoContainer.Add(container);

            foreach (Ability.Ability ability in _hero.GetAllAbilities())
            {
                AbilityElement icon = new(ability, true);
                _abilityIconsElements.Add(icon);
                container.Add(icon);
            }

            _hero.OnAbilityAdded += a =>
            {
                AbilityElement icon = new(a, true);
                _abilityIconsElements.Add(icon);
                container.Add(icon);
            };

            _hero.OnAbilityRemoved += a =>
            {
                AbilityElement icon = _abilityIconsElements.Find(e => e.Ability == a);
                _abilityIconsElements.Remove(icon);
                icon.RemoveFromHierarchy();
            };
        }

        private void HandleExpBar()
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
    }
}