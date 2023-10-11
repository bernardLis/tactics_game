using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using DG.Tweening;
using Cinemachine;

public class BattleHeroManager : MonoBehaviour
{
    VisualElement _root;
    VisualElement _bottomPanel;
    HeroBattleElement _heroBattleElement;

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

        _heroBattleElement = new(hero);
        _bottomPanel.Add(_heroBattleElement);
    }

    void OnHeroLevelUp()
    {
        BattleRewardElement rewardElement = new();

        rewardElement.OnHide += () =>
        {
            Hero.AddExp(Hero.LeftoverExp);
        };
    }

}
