using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using DG.Tweening;

public class BattleHeroManager : MonoBehaviour
{
    VisualElement _root;

    Hero _hero;
    public void Initialize(Hero hero)
    {
        _root = GetComponent<UIDocument>().rootVisualElement;

        _hero = hero;
        hero.BattleInitialize();

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

}
