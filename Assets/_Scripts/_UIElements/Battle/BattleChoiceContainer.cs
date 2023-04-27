using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using DG.Tweening;

public class BattleChoiceContainer : VisualElement
{
    List<BattleCard> _cards = new();

    public event Action OnBattleSelected;
    public BattleChoiceContainer()
    {
        style.alignItems = Align.Center;

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
            _cards.Add(card);
            card.OnCardSelected += OnCardSelected;
        }
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
