using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
public class HeroBattleElement : VisualElement
{
    const string _ussCommonTextPrimary = "common__text-primary";

    const string _ussClassName = "hero-battle__";
    const string _ussMain = _ussClassName + "main";
    const string _ussInfoContainer = _ussClassName + "info-container";
    const string _ussStatContainer = _ussClassName + "stat-container";

    GameManager _gameManager;

    Hero _hero;

    VisualElement _heroInfoContainer;
    ResourceBarElement _expBar;
    Label _levelLabel;
    List<AbilityButton> _abilityButtons = new();

    public HeroBattleElement(Hero hero)
    {
        _gameManager = GameManager.Instance;
        var common = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.CommonStyles);
        if (common != null)
            styleSheets.Add(common);
        var ss = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.HeroBattleStyles);
        if (ss != null)
            styleSheets.Add(ss);

        _hero = hero;
        hero.OnLevelUp += OnHeroLevelUp;
        AddToClassList(_ussMain);

        _heroInfoContainer = new();
        _heroInfoContainer.AddToClassList(_ussInfoContainer);
        Add(_heroInfoContainer);

        HandleExpBar();
        HandleAbilityIcons();
        HandleHeroStatIcons();
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

    void HandleAbilityIcons()
    {
        VisualElement container = new();
        container.style.flexDirection = FlexDirection.Row;
        _heroInfoContainer.Add(container);

        foreach (Ability a in _hero.Abilities)
        {
            AbilityButton abilityIcon = new(a);
            container.Add(abilityIcon);
        }

        _hero.OnAbilityAdded += (Ability a) =>
        {
            AbilityButton abilityIcon = new(a);
            _abilityButtons.Add(abilityIcon);
            container.Add(abilityIcon);
        };

        _hero.OnAbilityRemoved += (Ability removedAbility) =>
        {
            foreach (AbilityButton button in _abilityButtons)
                if (button.Ability == removedAbility)
                    container.Remove(button);
        };
    }

    void HandleHeroStatIcons()
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
