using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using DG.Tweening;
using Random = UnityEngine.Random;

public class BattleResultChoiceElement : VisualElement
{
    GameManager _gameManager;
    AudioManager _audioManager;

    List<BattleCard> _hiddenCards = new();

    List<BattleCard> _cards = new();

    VisualElement _cardContainer;
    RerollButton _rerollButton;

    List<BattleModifier> _selectedBattleModifiers = new();


    public event Action OnBattleSelected;
    public BattleResultChoiceElement()
    {
        _gameManager = GameManager.Instance;
        _audioManager = _gameManager.GetComponent<AudioManager>();

        style.alignItems = Align.Center;

        Label bn = new($"Battle #{GameManager.Instance.BattleNumber + 1}");
        bn.style.fontSize = 24;
        Add(bn);

        Label l = new("Choose your next battle: ");
        l.style.fontSize = 32;
        Add(l);

        _cardContainer = new();
        _cardContainer.style.flexDirection = FlexDirection.Row;
        Add(_cardContainer);

        AddPlaceholderCards();
        AddRerollButton();
        AddBattleModifiers();

        style.opacity = 0;
        DOTween.To(x => style.opacity = x, 0, 1, 0.5f)
            .OnComplete(() => ShowCards());

    }

    void AddPlaceholderCards()
    {
        // placeholder to make sure the cards are centered
        for (int i = 0; i < 3; i++)
        {
            BattleType type = (BattleType)UnityEngine.Random.Range(0, Enum.GetValues(typeof(BattleType)).Length);
            BattleCard card = new(type);
            _cardContainer.Add(card);
            card.style.visibility = Visibility.Hidden;
            _hiddenCards.Add(card);
        }
    }

    void AddRerollButton()
    {
        _rerollButton = new(callback: RerollCards);
        _rerollButton.style.opacity = 0;
        Add(_rerollButton);

        this.schedule.Execute(() => DOTween.To(x => _rerollButton.style.opacity = x, 0, 1, 0.5f))
                .StartingIn(2100);
    }

    void ShowCards()
    {
        _cards.Clear();
        for (int i = 0; i < _hiddenCards.Count; i++)
        {
            BattleType type = (BattleType)Random.Range(0, Enum.GetValues(typeof(BattleType)).Length);
            BattleCard card = new(type);
            foreach (BattleModifier bm in _selectedBattleModifiers)
                card.Battle.AddModifier(bm);

            _cards.Add(card);
            _cardContainer.Add(card);

            card.style.position = Position.Absolute;
            card.style.left = Screen.width;

            schedule.Execute(() => _audioManager.PlayUI("Paper")).StartingIn(200 + i * 300);

            float endLeft = i * (_hiddenCards[i].resolvedStyle.width +
                    _hiddenCards[i].resolvedStyle.marginLeft + _hiddenCards[i].resolvedStyle.right);

            DOTween.To(x => card.style.left = x, Screen.width, endLeft, 0.5f)
                .SetEase(Ease.InFlash)
                .SetDelay(i * 0.2f);
            card.OnCardSelected += OnCardSelected;
        }
    }

    void RerollCards()
    {
        // TODO: something smarter about the cost...
        if (_gameManager.Gold < 200)
        {
            Helpers.DisplayTextOnElement(BattleManager.Instance.GetComponent<UIDocument>().rootVisualElement,
                _rerollButton, "Not enough gold!", Color.red);
            return;
        }
        _audioManager.PlayUI("Dice Roll");

        _gameManager.ChangeGoldValue(-200);

        foreach (BattleCard c in _cards)
        {
            DOTween.To(x => c.style.opacity = x, 1, 0, 0.5f)
                    .OnComplete(() =>
                    {
                        c.style.display = DisplayStyle.None;
                        _cardContainer.Remove(c);
                    });
        }
        this.schedule.Execute(() => ShowCards()).StartingIn(500);
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

    void AddBattleModifiers()
    {
        Add(new Label("Choose battle modifiers: "));

        VisualElement container = new();
        Add(container);
        container.style.flexDirection = FlexDirection.Row;

        GameDatabase gdb = _gameManager.GameDatabase;
        List<BattleModifier> chosenModifiers = new();
        for (int i = 0; i < 3; i++)
        {
            BattleModifier modifier = gdb.GetRandomBattleModifier();
            while (chosenModifiers.Contains(modifier)) // TODO: risky?
                modifier = gdb.GetRandomBattleModifier();
            chosenModifiers.Add(modifier);

            BattleModifierElement element = new(modifier);
            element.RegisterCallback<PointerUpEvent>((e) =>
            {
                if (!element.enabledInHierarchy) return;
                if (_gameManager.Gold < modifier.Cost)
                {
                    Helpers.DisplayTextOnElement(BattleManager.Instance.Root,
                            element, "Not enough gold!", Color.red);
                    return;
                }
                if (_selectedBattleModifiers.Count >= 2)
                {
                    Helpers.DisplayTextOnElement(BattleManager.Instance.Root,
                            element, "You can only choose 2 modifiers!", Color.red);
                    return;
                }
                _gameManager.ChangeGoldValue(-modifier.Cost);
                AddModifierToAllBattles(modifier);
                element.SetEnabled(false);
            });
            container.Add(element);
        }
    }

    void AddModifierToAllBattles(BattleModifier modifier)
    {
        _selectedBattleModifiers.Add(modifier);
        foreach (BattleCard c in _cards)
            c.Battle.AddModifier(modifier);
    }
}
