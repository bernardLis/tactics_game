using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;
using DG.Tweening;

public class BattleRewardElement : VisualElement
{
    const string _ussCommonTextPrimary = "common__text-primary";
    const string _ussCommonMenuButton = "common__menu-button";

    const string _ussClassName = "battle-reward__";
    const string _ussMain = _ussClassName + "main";
    const string _ussContinueButton = _ussClassName + "continue-button";


    GameManager _gameManager;
    AudioManager _audioManager;

    VisualElement _rewardContainer;
    Label _rewardTitle;
    List<RewardCard> _hiddenCards = new();

    List<RewardCard> _allRewardCards = new();
    List<RewardCard> _selectedRewardCards = new();

    RerollButton _rerollButton;

    public event Action OnRewardSelected;
    public event Action OnContinueClicked;

    public BattleRewardElement()
    {
        _gameManager = GameManager.Instance;
        _audioManager = _gameManager.GetComponent<AudioManager>();
        var commonStyles = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.CommonStyles);
        if (commonStyles != null)
            styleSheets.Add(commonStyles);
        var ss = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.BattleRewardStyles);
        if (ss != null)
            styleSheets.Add(ss);

        AddToClassList(_ussCommonTextPrimary);
        AddToClassList(_ussMain);

        AddHeroCard();
        AddRewardContainer();
    }

    void AddHeroCard()
    {
        Label title = new Label("Add stat point");
        Add(title);
        title.style.fontSize = 32;
        title.style.opacity = 0;
        DOTween.To(x => title.style.opacity = x, 0, 1, 0.5f).SetUpdate(true);

        HeroCardExp card = new(_gameManager.PlayerHero);
        Add(card);
        card.style.opacity = 0;
        DOTween.To(x => card.style.opacity = x, 0, 1, 0.5f)
            .SetUpdate(true)
            .OnComplete(card.LeveledUp);

        card.OnPointAdded += () =>
        {
            title.style.visibility = Visibility.Hidden;
            RunCardShow();
        };
    }

    void AddRewardContainer()
    {
        _rewardTitle = new Label("Choose your reward:");
        _rewardTitle.style.fontSize = 32;
        _rewardTitle.style.opacity = 0;
        Add(_rewardTitle);

        _rewardContainer = new();
        _rewardContainer.style.flexDirection = FlexDirection.Row;
        Add(_rewardContainer);

        _hiddenCards = new();
        for (int i = 0; i < 3; i++)
        {
            RewardCard card = CreateRewardCardGold();
            _hiddenCards.Add(card);
            card.style.visibility = Visibility.Hidden;
            _rewardContainer.Add(card);
        }

        _rerollButton = new(callback: RerollReward);
        Add(_rerollButton);
    }

    void RunCardShow()
    {
        DOTween.To(x => _rewardTitle.style.opacity = x, 0, 1, 0.5f).SetUpdate(true);

        schedule.Execute(() =>
        {
            CreateRewardCards();
            for (int i = 0; i < 3; i++)
            {
                RewardCard card = _allRewardCards[Random.Range(0, _allRewardCards.Count)];
                _allRewardCards.Remove(card);
                _selectedRewardCards.Add(card);
                _rewardContainer.Add(card);

                card.style.position = Position.Absolute;
                card.style.left = Screen.width;

                _audioManager.PlayUIDelayed("Paper", 0.2f + i * 0.3f);
                float endLeft = i * (_hiddenCards[i].resolvedStyle.width
                    + _hiddenCards[i].resolvedStyle.marginLeft + _hiddenCards[i].resolvedStyle.right);
                DOTween.To(x => card.style.left = x, Screen.width, endLeft, 0.5f)
                        .SetEase(Ease.InFlash)
                        .SetDelay(i * 0.2f).SetUpdate(true);
            }
        }).StartingIn(10);

    }

    void RerollReward()
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
        PopulateRewards();
    }

    void PopulateRewards()
    {
        _rewardContainer.Clear();
        _allRewardCards.Clear();
        CreateRewardCards();
        ChooseRewardCards();
    }

    void CreateRewardCards()
    {
        _allRewardCards.Add(CreateRewardCardItem());
        _allRewardCards.Add(CreateRewardCardAbility());
        _allRewardCards.Add(CreateRewardCardGold());
        _allRewardCards.Add(CreateRewardCardArmy());
    }

    void ChooseRewardCards()
    {
        for (int i = 0; i < 3; i++)
        {
            RewardCard card = _allRewardCards[Random.Range(0, _allRewardCards.Count)];
            _allRewardCards.Remove(card);
            _selectedRewardCards.Add(card);
            _rewardContainer.Add(card);
        }
    }

    RewardCard CreateRewardCardItem()
    {
        RewardItem reward = ScriptableObject.CreateInstance<RewardItem>();
        reward.CreateRandom(_gameManager.PlayerHero);
        reward.OnRewardSelected += RewardSelected;
        return new RewardCardItem(reward);
    }

    RewardCard CreateRewardCardAbility()
    {
        RewardAbility reward = ScriptableObject.CreateInstance<RewardAbility>();
        reward.CreateRandom(_gameManager.PlayerHero);
        reward.OnRewardSelected += RewardSelected;
        return new RewardCardAbility(reward);
    }

    RewardCard CreateRewardCardGold()
    {
        RewardGold reward = ScriptableObject.CreateInstance<RewardGold>();
        reward.CreateRandom(_gameManager.PlayerHero);
        reward.OnRewardSelected += RewardSelected;
        return new RewardCardGold(reward);
    }

    RewardCard CreateRewardCardArmy()
    {
        RewardArmy reward = ScriptableObject.CreateInstance<RewardArmy>();
        reward.CreateRandom(_gameManager.PlayerHero);
        reward.OnRewardSelected += RewardSelected;
        RewardCard card = new RewardCardArmy(reward);

        if (_gameManager.PlayerHero.CreatureArmy.Count >= _gameManager.SelectedBattle.Base.TroopsLimit.Value)
        {
            card.DisableCard();
            card.Add(new Label("Your army is full!"));
        }

        return card;
    }

    void RewardSelected(Reward reward)
    {
        _audioManager.PlayUI("Reward Chosen");

        _rerollButton.SetEnabled(false);
        foreach (RewardCard card in _selectedRewardCards)
        {
            if (card.Reward != reward) card.DisableCard();

            card.DisableClicks();
        }

        OnRewardSelected?.Invoke();


        MyButton continueButton = new("Continue", _ussContinueButton, MoveAway);
        Add(continueButton);
        continueButton.style.opacity = 0;
        DOTween.To(x => continueButton.style.opacity = x, 0, 1, 0.3f).SetUpdate(true);

    }

    public void MoveAway()
    {
        style.position = Position.Absolute;
        DOTween.To(x => style.opacity = x, 1, 0, 0.5f)
            .SetUpdate(true)
            .OnComplete(() => OnContinueClicked?.Invoke());
    }
}
