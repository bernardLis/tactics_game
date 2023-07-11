using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using DG.Tweening;

public class BattleHeroManager : MonoBehaviour
{
    BattleManager _battleManager;
    VisualElement _root;

    Hero _hero;
    public void Initialize(Hero hero)
    {
        _battleManager = BattleManager.Instance;
        _root = GetComponent<UIDocument>().rootVisualElement;

        _hero = hero;
        hero.BattleInitialize();
        hero.OnLevelUp += OnHeroLevelUp;

        AddHeroCard();
    }

    void AddHeroCard()
    {
        VisualElement bottomPanel = _root.Q<VisualElement>("bottomPanel");

        HeroCardStats card = new(_hero);
        bottomPanel.Insert(0, card);
        card.style.opacity = 0;

        DOTween.To(x => card.style.opacity = x, 0, 1, 0.5f);
    }

    void OnHeroLevelUp()
    {
        _battleManager.PauseGame();
        BattleResultRewardElement rewardElement = new();
        _root.Add(rewardElement);

        rewardElement.OnRewardSelected += () =>
        {
            _battleManager.ResumeGame();
            _root.Remove(rewardElement);
        };
    }

}
