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

    GameManager _gameManager;

    Hero _hero;

    VisualElement _heroInfoContainer;
    ResourceBarElement _expBar;
    Label _levelLabel;
    List<AbilityElement> _abilityButtons = new();

    VisualElement _advancedViewContainer;

    public HeroElement(Hero hero, bool isAdvanced = false)
    {
        _gameManager = GameManager.Instance;
        var common = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.CommonStyles);
        if (common != null) styleSheets.Add(common);
        var ss = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.HeroElementStyles);
        if (ss != null) styleSheets.Add(ss);

        _hero = hero;
        hero.OnLevelUp += OnHeroLevelUp;
        AddToClassList(_ussMain);

        _heroInfoContainer = new();
        _heroInfoContainer.AddToClassList(_ussInfoContainer);
        Add(_heroInfoContainer);

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
        Color c = GameManager.Instance.GameDatabase.GetColorByName("Experience").Color;
        _expBar = new(c, "Experience", _hero.Experience, _hero.ExpForNextLevel);

        _levelLabel = new Label($"Level {_hero.Level.Value}");
        _levelLabel.style.position = Position.Absolute;
        _levelLabel.AddToClassList(_ussCommonTextPrimary);
        _expBar.Add(_levelLabel);

        Add(_expBar);
    }

    void HandleAbilities()
    {
        VisualElement container = new();
        container.style.flexDirection = FlexDirection.Row;
        container.style.alignItems = Align.Center;
        _heroInfoContainer.Add(container);

        foreach (Ability a in _hero.Abilities)
        {
            AbilityElement abilityIcon = new(a, true);
            container.Add(abilityIcon);
        }

        _hero.OnAbilityAdded += (Ability a) =>
        {
            AbilityElement abilityIcon = new(a, true);
            _abilityButtons.Add(abilityIcon);
            container.Add(abilityIcon);
        };

        _hero.OnAbilityRemoved += (Ability removedAbility) =>
        {
            foreach (AbilityElement button in _abilityButtons)
                if (button.Ability == removedAbility)
                    container.Remove(button);
        };
    }

    void HandleAdvancedView()
    {
        _advancedViewContainer = new();
        _heroInfoContainer.Add(_advancedViewContainer);

        HandleTablets();
        HandleStats();
    }

    void HandleTablets()
    {
        VisualElement container = new();
        container.style.flexDirection = FlexDirection.Row;
        _advancedViewContainer.Add(container);

        foreach (Tablet t in _hero.Tablets)
        {
            TabletElement tabletElement = new(t, true);
            container.Add(tabletElement);
        }
    }

    void HandleStats()
    {
        VisualElement statContainer = new();
        statContainer.AddToClassList(_ussStatContainer);
        _advancedViewContainer.Add(statContainer);

        foreach (Stat s in _hero.GetAllStats())
        {
            StatElement statElement = new(s);
            statContainer.Add(statElement);
        }
    }

}
