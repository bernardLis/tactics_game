using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using DG.Tweening;

public class BattleChoiceContainer : VisualElement
{
    GameManager _gameManager;
    AudioManager _audioManager;

    List<BattleCard> _hiddenCards = new();

    List<BattleCard> _cards = new();

    VisualElement _cardContainer;

    public event Action OnBattleSelected;
    public BattleChoiceContainer()
    {
        _gameManager = GameManager.Instance;
        _audioManager = _gameManager.GetComponent<AudioManager>();

        style.alignItems = Align.Center;

        Label bn = new($"Battle #{GameManager.Instance.BattleNumber + 1}");
        bn.style.fontSize = 24;
        Add(bn);

        Label l = new("Choose your next opponent: ");
        l.style.fontSize = 32;
        Add(l);

        _cardContainer = new();
        _cardContainer.style.flexDirection = FlexDirection.Row;
        Add(_cardContainer);
        for (int i = 0; i < 3; i++)
        {
            BattleCard card = new();
            _cardContainer.Add(card);
            card.style.visibility = Visibility.Hidden;
            _hiddenCards.Add(card);
        }

        style.opacity = 0;
        DOTween.To(x => style.opacity = x, 0, 1, 0.5f)
            .OnComplete(() => ShowCards());
    }

    void ShowCards()
    {
        for (int i = 0; i < _hiddenCards.Count; i++)
        {
            BattleCard card = new();
            _cards.Add(card);
            _cardContainer.Add(card);

            card.style.position = Position.Absolute;
            card.style.left = Screen.width;

            // HERE: audio siup siup
            schedule.Execute(() => _audioManager.PlaySFX("Paper", Vector3.zero)).StartingIn(200 + i * 300);

            float endLeft = i * (_hiddenCards[i].resolvedStyle.width +
                    _hiddenCards[i].resolvedStyle.marginLeft + _hiddenCards[i].resolvedStyle.right);

            DOTween.To(x => card.style.left = x, Screen.width, endLeft, 0.5f)
                .SetEase(Ease.InFlash)
                .SetDelay(i * 0.2f);
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
