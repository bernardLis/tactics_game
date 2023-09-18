using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using DG.Tweening;

public class BattleHeroManager : MonoBehaviour
{
    [SerializeField] EntitySpawner _spawnerPrefab;


    BattleManager _battleManager;
    BattleIntroManager _battleIntroManager;
    VisualElement _root;

    public BattleHero BattleHero;

    public Hero Hero { get; private set; }
    public void Initialize(Hero hero)
    {
        _battleManager = BattleManager.Instance;
        _root = GetComponent<UIDocument>().rootVisualElement;

        Hero = hero;
        hero.InitializeBattle(0);
        hero.OnLevelUpReady += OnHeroLevelUp;

        BattleHero.InitializeEntity(hero);

        _battleIntroManager = BattleIntroManager.Instance;
        if (_battleIntroManager != null)
            _battleIntroManager.OnIntroFinished += AddHeroCard;
        else
            AddHeroCard();
    }

    void AddHeroCard()
    {
        VisualElement bottomPanel = _root.Q<VisualElement>("bottomPanel");

        HeroCardStats card = new(Hero);
        bottomPanel.Insert(0, card);
        card.style.opacity = 0;

        DOTween.To(x => card.style.opacity = x, 0, 1, 0.5f);
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
