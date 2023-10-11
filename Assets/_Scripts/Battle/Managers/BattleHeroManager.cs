using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using DG.Tweening;
using Cinemachine;

public class BattleHeroManager : MonoBehaviour
{
    const string _ussCommonTextPrimary = "common__text-primary";

    VisualElement _root;
    VisualElement _bottomPanel;
    VisualElement _heroInfoContainer;
    ResourceBarElement _expBar;
    Label _levelLabel;
    List<AbilityButton> _abilityButtons = new();

    public BattleHero BattleHero;
    public Hero Hero { get; private set; }
    public void Initialize(Hero hero)
    {
        _root = GetComponent<UIDocument>().rootVisualElement;
        _bottomPanel = _root.Q<VisualElement>("bottomPanel");
        _heroInfoContainer = new();
        _heroInfoContainer.style.flexDirection = FlexDirection.Row;
        _heroInfoContainer.style.width = Length.Percent(100);
        _heroInfoContainer.style.justifyContent = Justify.SpaceBetween;
        _bottomPanel.Add(_heroInfoContainer);

        Hero = hero;
        hero.InitializeBattle(0);
        hero.OnLevelUp += OnHeroLevelUp;

        BattleHero.InitializeEntity(hero);

        HandleExpBar();
        HandleAbilityIcons();
        HandleHeroStatIcons();
    }

    void OnHeroLevelUp()
    {
        BattleRewardElement rewardElement = new();

        rewardElement.OnHide += () =>
        {
            Hero.AddExp(Hero.LeftoverExp);
        };

        _levelLabel.text = $"Level {Hero.Level.Value}";
    }

    void HandleExpBar()
    {
        Color c = GameManager.Instance.GameDatabase.GetColorByName("Experience").Color;
        _expBar = new(c, "Experience", Hero.Experience, Hero.ExpForNextLevel);

        _levelLabel = new Label($"Level {Hero.Level.Value}");
        _levelLabel.style.position = Position.Absolute;
        _levelLabel.AddToClassList(_ussCommonTextPrimary);
        _expBar.Add(_levelLabel);

        _bottomPanel.Add(_expBar);
    }

    void HandleAbilityIcons()
    {
        VisualElement container = new();
        container.style.flexDirection = FlexDirection.Row;
        _heroInfoContainer.Add(container);

        foreach (Ability a in Hero.Abilities)
        {
            AbilityButton abilityIcon = new(a);
            container.Add(abilityIcon);
        }

        Hero.OnAbilityAdded += (Ability a) =>
        {
            AbilityButton abilityIcon = new(a);
            _abilityButtons.Add(abilityIcon);
            container.Add(abilityIcon);
        };
        
        Hero.OnAbilityRemoved += (Ability removedAbility) =>
        {
            foreach (AbilityButton button in _abilityButtons)
                if (button.Ability == removedAbility)
                    container.Remove(button);
        };
    }

    void HandleHeroStatIcons()
    {
        VisualElement container = new();
        container.style.flexDirection = FlexDirection.Row;
        _heroInfoContainer.Add(container);

        Label a = new("ASD 15");
        Label b = new("ASD 15");
        Label c = new("ASD 15");
        Label d = new("ASD 15");

        container.Add(a);
        container.Add(b);
        container.Add(c);
        container.Add(d);
    }
}
