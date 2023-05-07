using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

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
    }

}
