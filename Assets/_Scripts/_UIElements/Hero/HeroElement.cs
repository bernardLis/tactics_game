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
        const string _ussAbilitySlot = _ussClassName + "ability-slot";
        const string _ussAbilitySlotPremium = _ussAbilitySlot + "-premium";
        const string _ussTabletSlot = _ussClassName + "tablet-slot";
        const string _ussTabletSlotPremium = _ussTabletSlot + "-premium";

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

            _hero.OnAbilityAdded += (Ability a) =>
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
                slot.AddToClassList(_ussAbilitySlot);
                container.Add(slot);
                slots.Add(slot);
            }

            _hero.OnAbilityAdded += (Ability a) =>
            {
                if (slots.Count > 0)
                {
                    container.Remove(slots[0]);
                    slots.RemoveAt(0);
                }

                AbilityElement abilityIcon = new(a, true);
                container.Insert(_hero.Abilities.Count - 1, abilityIcon);
            };
        }

        // List<VisualElement> _tabletSlots = new();
        void HandleTablets()
        {
            VisualElement container = new();
            container.style.flexDirection = FlexDirection.Row;
            // _advancedViewContainer.Add(container);
            _heroInfoContainer.Add(container);

            foreach (Tablet t in _hero.Tablets)
            {
                TabletElement tabletElement = new(t, true);
                container.Add(tabletElement);
            }

            // VisualElement container = new();
            // container.style.flexDirection = FlexDirection.Row;
            // _advancedViewContainer.Add(container);
            //
            // _tabletSlots = new();
            // for (int i = 0; i < 5; i++)
            // {
            //     VisualElement slot = new();
            //     if (i > 3) slot.AddToClassList(_ussTabletSlotPremium);// 2 last ones are premium
            //     else slot.AddToClassList(_ussTabletSlot);
            //     _tabletSlots.Add(slot);
            //     container.Add(slot);
            // }
            // ShowTablets();
            // foreach (Tablet t in _hero.Tablets)
            //     if (t.Level.Value == 0)
            //         t.OnLevelUp += ShowTablets;
        }

        // void ShowTablets()
        // {
        //     for (int i = 0; i < _hero.Tablets.Count; i++)
        //     {
        //         // if (_hero.Tablets[i].Level.Value == 0) continue;
        //         if (_tabletSlots[i].childCount > 0) continue;
        //
        //         TabletElement tabletElement = new(_hero.Tablets[i], true);
        //         _tabletSlots[i].Add(tabletElement);
        //     }
        //
        //     if (_hero.AdvancedTablet != null)
        //     {
        //         TabletElement tabletElement = new(_hero.AdvancedTablet, true);
        //         _tabletSlots[^1].Add(tabletElement);
        //     }
        // }

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