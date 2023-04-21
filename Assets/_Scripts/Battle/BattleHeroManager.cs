using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BattleHeroManager : MonoBehaviour
{
    GameManager _gameManager;
    VisualElement _root;

    Hero _hero;
    void Start()
    {
        _gameManager = GameManager.Instance;
        _root = GetComponent<UIDocument>().rootVisualElement;

        _hero = _gameManager.PlayerHero;
        _hero.BattleInitialize();

        AddHeroCard();
    }

    void AddHeroCard()
    {
        VisualElement bottomPanel = _root.Q<VisualElement>("bottomPanel");

        HeroStatsCard card = new(_hero);
        bottomPanel.Add(card);
    }

}
