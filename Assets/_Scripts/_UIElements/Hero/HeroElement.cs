using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
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

    GameManager _gameManager;

    Hero _hero;

    VisualElement _heroInfoContainer;
    ResourceBarElement _expBar;
    Label _levelLabel;
    List<AbilityElement> _abilityElements = new();

    bool _isAdvancedView;
    VisualElement _advancedViewContainer;

    public HeroElement(Hero hero, bool isAdvanced = false)
    {
        _gameManager = GameManager.Instance;
        var ss = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.HeroElementStyles);
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

        for (int i = 0; i < _hero.Abilities.Count; i++)
        {
            AbilityElement abilityIcon = new(_hero.Abilities[i], true);
            container.Add(abilityIcon);
            _abilityElements.Add(abilityIcon);
        }

        _hero.OnAbilityAdded += (Ability a) =>
        {
            AbilityElement abilityIcon = new(a, true);
            _abilityElements.Add(abilityIcon);
            container.Add(abilityIcon);
        };
    }

    void HandleAdvancedView()
    {
        _advancedViewContainer = new();
        _heroInfoContainer.Add(_advancedViewContainer);

        HandleAdvancedAbilities();
        HandleTablets();
        HandleStats();
    }

    void HandleAdvancedAbilities()
    {
        VisualElement container = new();
        container.style.flexDirection = FlexDirection.Row;
        container.style.alignItems = Align.Center;
        _advancedViewContainer.Add(container);

        List<VisualElement> abilitySlots = new();

        for (int i = 0; i < 6; i++)
        {
            VisualElement slot = new();
            if (i > 3) slot.AddToClassList(_ussAbilitySlotPremium); // 2 last ones are premium
            else slot.AddToClassList(_ussAbilitySlot);
            abilitySlots.Add(slot);
            container.Add(slot);
        }

        for (int i = 0; i < _hero.Abilities.Count; i++)
        {
            AbilityElement abilityIcon = new(_hero.Abilities[i], true);
            abilitySlots[i].Add(abilityIcon);
            _abilityElements.Add(abilityIcon);
        }

        _hero.OnAbilityAdded += (Ability a) =>
        {
            AbilityElement abilityIcon = new(a, true);
            _abilityElements.Add(abilityIcon);
            abilitySlots[_abilityElements.Count - 1].Add(abilityIcon);
        };
    }

    List<VisualElement> _tabletSlots = new();
    void HandleTablets()
    {
        VisualElement container = new();
        container.style.flexDirection = FlexDirection.Row;
        _advancedViewContainer.Add(container);

        _tabletSlots = new();
        for (int i = 0; i < 5; i++)
        {
            VisualElement slot = new();
            if (i > 3) slot.AddToClassList(_ussTabletSlotPremium);// 2 last ones are premium
            else slot.AddToClassList(_ussTabletSlot);
            _tabletSlots.Add(slot);
            container.Add(slot);
        }
        ShowTablets();
        foreach (Tablet t in _hero.Tablets)
            if (t.Level.Value == 0)
                t.OnLevelUp += ShowTablets;
    }

    void ShowTablets()
    {
        for (int i = 0; i < _hero.Tablets.Count; i++)
        {
            if (_hero.Tablets[i].Level.Value == 0) continue;
            if (_tabletSlots[i].childCount > 0) continue;

            TabletElement tabletElement = new(_hero.Tablets[i], true);
            _tabletSlots[i].Add(tabletElement);
        }

        if (_hero.AdvancedTablet != null)
        {
            TabletElement tabletElement = new(_hero.AdvancedTablet, true);
            _tabletSlots[_tabletSlots.Count - 1].Add(tabletElement);
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
