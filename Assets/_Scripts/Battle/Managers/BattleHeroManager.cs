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
    ResourceBarElement _expBar;
    Label _levelLabel;

    public BattleHero BattleHero;
    public Hero Hero { get; private set; }
    public void Initialize(Hero hero)
    {
        _root = GetComponent<UIDocument>().rootVisualElement;
        _bottomPanel = _root.Q<VisualElement>("bottomPanel");

        Hero = hero;
        hero.InitializeBattle(0);
        hero.OnLevelUp += OnHeroLevelUp;

        BattleHero.InitializeEntity(hero);

        AddExpBar();
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

    void AddExpBar()
    {
        Color c = GameManager.Instance.GameDatabase.GetColorByName("Experience").Color;
        _expBar = new(c, "Experience", Hero.Experience, Hero.ExpForNextLevel);

        _levelLabel = new Label($"Level {Hero.Level.Value}");
        _levelLabel.style.position = Position.Absolute;
        _levelLabel.AddToClassList(_ussCommonTextPrimary);
        _expBar.Add(_levelLabel);

        _bottomPanel.Add(_expBar);
    }
}
