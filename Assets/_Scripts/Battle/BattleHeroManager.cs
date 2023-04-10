using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BattleHeroManager : MonoBehaviour
{
    GameManager _gameManager;
    VisualElement _root;

    void Start()
    {
        _gameManager = GameManager.Instance;
        _root = GetComponent<UIDocument>().rootVisualElement;

        AddHeroCard();
    }

    void AddHeroCard()
    {
        VisualElement heroCardContainer = _root.Q<VisualElement>("HeroCardContainer");
        Hero hero = _gameManager.SelectedBattle.Hero;

        HeroStatsCard card = new(hero);
        heroCardContainer.Add(card);
    }

}
