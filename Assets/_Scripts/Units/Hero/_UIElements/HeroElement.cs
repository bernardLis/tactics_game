using Lis.Core;
using Lis.Units.Hero.Ability;
using UnityEngine;
using UnityEngine.UIElements;

namespace Lis.Units.Hero
{
    public class HeroElement : VisualElement
    {
        const string _ussCommonTextPrimary = "common__text-primary";

        const string _ussClassName = "hero-element__";
        const string _ussMain = _ussClassName + "main";

        const string _ussInfoContainer = _ussClassName + "info-container";

        readonly Hero _hero;

        VisualElement _resourcesContainer;
        TroopsCountElement _troopsCounter;

        readonly VisualElement _heroInfoContainer;
        ResourceBarElement _expBar;
        Label _levelLabel;

        VisualElement _advancedViewContainer;

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

        void OnHeroLevelUp()
        {
            _levelLabel.text = $"Level {_hero.Level.Value}";
        }

        void HandleAbilities()
        {
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
    }
}