using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using DG.Tweening;

public class BattleChoiceContainer : VisualElement
{
    GameManager _gameManager;
    List<BattleCard> _cards = new();

    public event Action OnBattleSelected;
    public BattleChoiceContainer()
    {
        _gameManager = GameManager.Instance;

        style.alignItems = Align.Center;

        Label bn = new($"Battle #{GameManager.Instance.BattleNumber + 1}");
        bn.style.fontSize = 24;
        Add(bn);

        Label l = new("Choose your next opponent: ");
        l.style.fontSize = 32;
        Add(l);

        VisualElement cardContainer = new();
        cardContainer.style.flexDirection = FlexDirection.Row;
        Add(cardContainer);
        for (int i = 0; i < 3; i++)
        {
            BattleCard card = new();
            cardContainer.Add(card);
            card.style.visibility = Visibility.Hidden;
            _cards.Add(card);
            card.OnCardSelected += OnCardSelected;
        }

        style.opacity = 0;
        DOTween.To(x => style.opacity = x, 0, 1, 0.5f);

        // it would be cool if the cards were falling one by one into their places
    }

    void OnCardSelected(BattleCard selectedCard)
    {
        foreach (BattleCard c in _cards)
        {
            if (c != selectedCard) c.DisableCard();
            c.DisableClicks();
        }

        OnBattleSelected?.Invoke();
    }
}
