using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public class RewardContainer : VisualElement
{
    const string _ussCommonTextPrimary = "common__text-primary";
    const string _ussCommonMenuButton = "common__menu-button";

    const string _ussClassName = "reward-container__";
    const string _ussMain = _ussClassName + "main";
    const string _ussRerollButton = _ussClassName + "reroll-button";
    const string _ussRerollIcon = _ussClassName + "reroll-icon";
    const string _ussRerollIconHover = _ussClassName + "reroll-icon-hover";


    GameManager _gameManager;

    VisualElement _rewardContainer;

    List<RewardCard> _allRewardCards = new();
    List<RewardCard> _selectedRewardCards = new();

    MyButton _rerollButton;
    GoldElement _rerollCost;

    public event Action OnRewardSelected;
    public RewardContainer()
    {
        _gameManager = GameManager.Instance;
        var commonStyles = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.CommonStyles);
        if (commonStyles != null)
            styleSheets.Add(commonStyles);
        var ss = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.RewardContainerStyles);
        if (ss != null)
            styleSheets.Add(ss);

        AddToClassList(_ussMain);

        Label title = new Label("Choose your reward:");
        title.style.fontSize = 32;
        Add(title);

        _rewardContainer = new();
        _rewardContainer.style.flexDirection = FlexDirection.Row;
        Add(_rewardContainer);
        PopulateRewards();

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

    void RerollReward()
    {
        // TODO: something smarter about the cost...
        if (_gameManager.Gold < 200)
        {
            Helpers.DisplayTextOnElement(BattleManager.Instance.GetComponent<UIDocument>().rootVisualElement,
                _rerollCost, "Not enough gold!", Color.red);
            return;
        }

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
        _rerollButton.SetEnabled(false);
        foreach (RewardCard card in _selectedRewardCards)
        {
            if (card.Reward != reward) card.DisableCard();

            card.DisableClicks();
        }

        OnRewardSelected?.Invoke();
    }
}
