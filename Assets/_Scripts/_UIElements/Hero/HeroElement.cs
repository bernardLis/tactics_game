using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Lis
{
    public class HeroElement : VisualElement
    {
        const string _ussCommonTextPrimary = "common__text-primary";

        const string _ussClassName = "hero-element__";
        const string _ussMain = _ussClassName + "main";
        const string _ussInfoContainer = _ussClassName + "info-container";
        const string _ussStatContainer = _ussClassName + "stat-container";
        const string _ussTabletContainer = _ussClassName + "tablet-container";
        const string _ussSlot = _ussClassName + "slot";

        readonly Hero _hero;

        readonly VisualElement _heroInfoContainer;
        ResourceBarElement _expBar;
        Label _levelLabel;

        readonly bool _isAdvancedView;
        VisualElement _advancedViewContainer;

        public HeroElement(Hero hero, bool isAdvanced = false)
        {
            GameManager gameManager = GameManager.Instance;
            StyleSheet ss = gameManager.GetComponent<AddressableManager>()
                .GetStyleSheetByName(StyleSheetType.HeroElementStyles);
            if (ss != null) styleSheets.Add(ss);

            _hero = hero;
            hero.OnLevelUp += OnHeroLevelUp;
            AddToClassList(_ussMain);

            _heroInfoContainer = new();
            _heroInfoContainer.AddToClassList(_ussInfoContainer);
            Add(_heroInfoContainer);

            _isAdvancedView = isAdvanced;

            HandleAbilities();
            HandleExpBar();

            if (!isAdvanced) return;
            HandleAdvancedView();
        }

        void OnHeroLevelUp()
        {
            _levelLabel.text = $"Level {_hero.Level.Value}";
        }

        void HandleExpBar()
        {
            Color c = GameManager.Instance.GameDatabase.GetColorByName("Experience").Primary;
            _expBar = new(c, "Experience", _hero.Experience, _hero.ExpForNextLevel);

            _levelLabel = new Label($"Level {_hero.Level.Value}");
            _levelLabel.style.position = Position.Absolute;
            _levelLabel.AddToClassList(_ussCommonTextPrimary);
            _expBar.Add(_levelLabel);

            Add(_expBar);
        }

        void HandleAbilities()
        {
            if (_isAdvancedView) return;

            VisualElement container = new();
            container.style.flexDirection = FlexDirection.Row;
            container.style.alignItems = Align.Center;
            _heroInfoContainer.Add(container);

            foreach (Ability ability in _hero.Abilities)
            {
                AbilityElement abilityIcon = new(ability, true);
                container.Add(abilityIcon);
            }

            _hero.OnAbilityAdded += (a) =>
            {
                AbilityElement abilityIcon = new(a, true);
                container.Add(abilityIcon);
            };
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
            _heroInfoContainer.Add(container);

            foreach (Ability ability in _hero.Abilities)
            {
                AbilityElement abilityIcon = new(ability, true);
                container.Add(abilityIcon);
            }

            List<VisualElement> slots = new();
            for (int i = 0; i < 5 - _hero.Abilities.Count; i++)
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

                AbilityElement abilityIcon = new(a, true);
                container.Insert(_hero.Abilities.Count - 1, abilityIcon);

                if (a.Element.IsAdvanced && slots.Count > 0)
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