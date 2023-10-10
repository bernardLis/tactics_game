using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using DG.Tweening;

public class BattleHeroManager : MonoBehaviour
{
    [SerializeField] EntitySpawner _spawnerPrefab;

    VisualElement _root;

    public BattleHero BattleHero;

    public Hero Hero { get; private set; }
    public void Initialize(Hero hero)
    {
        _root = GetComponent<UIDocument>().rootVisualElement;

        Hero = hero;
        hero.InitializeBattle(0);
        hero.OnLevelUpReady += OnHeroLevelUp;

        BattleHero.InitializeEntity(hero);
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
