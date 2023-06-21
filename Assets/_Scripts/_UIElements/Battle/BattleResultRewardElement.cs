using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;
using DG.Tweening;

public class BattleResultRewardElement : VisualElement
{
    const string _ussCommonTextPrimary = "common__text-primary";
    const string _ussCommonMenuButton = "common__menu-button";

    const string _ussClassName = "battle-result-reward-element__";
    const string _ussMain = _ussClassName + "main";
    const string _ussRerollButton = _ussClassName + "reroll-button";
    const string _ussRerollIcon = _ussClassName + "reroll-icon";
    const string _ussRerollIconHover = _ussClassName + "reroll-icon-hover";


    GameManager _gameManager;
    AudioManager _audioManager;

    VisualElement _rewardContainer;

    List<RewardCard> _allRewardCards = new();
    List<RewardCard> _selectedRewardCards = new();

    MyButton _rerollButton;
    GoldElement _rerollCost;

    public event Action OnRewardSelected;
    public BattleResultRewardElement()
    {
        _gameManager = GameManager.Instance;
        _audioManager = _gameManager.GetComponent<AudioManager>();
        var commonStyles = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.CommonStyles);
        if (commonStyles != null)
            styleSheets.Add(commonStyles);
        var ss = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.BattleResultRewardStyles);
        if (ss != null)
            styleSheets.Add(ss);

        AddToClassList(_ussMain);

        Label title = new Label("Choose your reward:");
        title.style.fontSize = 32;
        Add(title);

        _rewardContainer = new();
        _rewardContainer.style.flexDirection = FlexDirection.Row;
        Add(_rewardContainer);

        RunCardShow();

        //PopulateRewards();

        _rerollButton = new("", _ussRerollButton, RerollReward);
        VisualElement rerollIcon = new();

        rerollIcon.AddToClassList(_ussRerollIcon);
        _rerollButton.Add(rerollIcon);

        _rerollButton.RegisterCallback<PointerEnterEvent>(evt => rerollIcon.AddToClassList(_ussRerollIconHover));
        _rerollButton.RegisterCallback<PointerLeaveEvent>(evt => rerollIcon.RemoveFromClassList(_ussRerollIconHover));

        _rerollCost = new(200);
        _rerollButton.Add(_rerollCost);
        Add(_rerollButton);
    }

    void RunCardShow()
    {
        List<RewardCard> hiddenCards = new();
        for (int i = 0; i < 3; i++)
        {
            RewardCard card = CreateRewardCardGold();
            hiddenCards.Add(card);
            card.style.visibility = Visibility.Hidden;
            _rewardContainer.Add(card);
        }

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
                float endLeft = i * (hiddenCards[i].resolvedStyle.width
                    + hiddenCards[i].resolvedStyle.marginLeft + hiddenCards[i].resolvedStyle.right);
                DOTween.To(x => card.style.left = x, Screen.width, endLeft, 0.5f)
                        .SetEase(Ease.InFlash)
                        .SetDelay(i * 0.2f);
            }
        }).StartingIn(10);

    }

    void RerollReward()
    {
        // TODO: something smarter about the cost...
        if (_gameManager.Gold < 200)
        {
            Helpers.DisplayTextOnElement(BattleManager.Instance.GetComponent<UIDocument>().rootVisualElement,
                _rerollCost, "Not enough gold!", Color.red);
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
        return new RewardCardArmy(reward);
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
    }

    public void MoveAway()
    {
        style.position = Position.Absolute;
        DOTween.To(x => style.opacity = x, 1, 0, 0.5f);
    }
}
