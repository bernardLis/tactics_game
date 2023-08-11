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
    const string _ussSpacer = _ussClassName + "spacer";

    GameManager _gameManager;
    AudioManager _audioManager;

    VisualElement _rewardContainer;
    Label _rewardTooltip;
    List<RewardCard> _hiddenCards = new();

    List<RewardCard> _allRewardCards = new();
    List<RewardCard> _selectedRewardCards = new();

    RerollButton _rerollButton;
    ContinueButton _continueButton;

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

        _rewardTooltip = new Label("Add stat point");
        _rewardTooltip.style.opacity = 0;
        _rewardTooltip.style.fontSize = 32;
        Add(_rewardTooltip);
        DOTween.To(x => _rewardTooltip.style.opacity = x, 0, 1, 0.5f).SetUpdate(true);

        AddHeroCard();

        VisualElement spacer = new();
        spacer.AddToClassList(_ussSpacer);
        Add(spacer);

        AddRewardContainer();
    }

    void AddHeroCard()
    {
        HeroCardExp card = new(_gameManager.PlayerHero);
        Add(card);
        card.style.opacity = 0;
        DOTween.To(x => card.style.opacity = x, 0, 1, 0.5f)
            .SetUpdate(true)
            .OnComplete(card.LeveledUp);

        card.OnPointAdded += () =>
        {
            DOTween.To(x => _rewardTooltip.style.opacity = x, 1, 0, 0.5f)
                .SetUpdate(true)
                .OnComplete(() =>
                {
                    _rewardTooltip.text = "Choose reward:";
                    DOTween.To(x => _rewardTooltip.style.opacity = x, 0, 1, 0.5f).SetUpdate(true);
                });

            RunCardShow();
        };
    }

    void AddRewardContainer()
    {
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
        _rerollButton.style.opacity = 0;
        _rerollButton.style.visibility = Visibility.Hidden;
        Add(_rerollButton);
    }

    void RunCardShow()
    {
        _rerollButton.style.visibility = Visibility.Visible;
        DOTween.To(x => _rerollButton.style.opacity = x, 0, 1, 0.5f)
            .SetDelay(0.5f)
            .SetUpdate(true);

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
        _allRewardCards.Add(CreateRewardCardObstacle());
        _allRewardCards.Add(CreateRewardCardTurret());

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
        RewardCreature reward = ScriptableObject.CreateInstance<RewardCreature>();
        reward.CreateRandom(_gameManager.PlayerHero);
        reward.OnRewardSelected += RewardSelected;
        RewardCard card = new RewardCardCreature(reward);

        if (_gameManager.PlayerHero.CreatureArmy.Count >= _gameManager.SelectedBattle.Spire.StoreyTroops.MaxTroopsTree.CurrentValue.Value)
        {
            card.DisableCard();
            card.Add(new Label("Your army is full!"));
        }

        return card;
    }

    RewardCard CreateRewardCardObstacle()
    {
        RewardObstacle reward = ScriptableObject.CreateInstance<RewardObstacle>();
        reward.CreateRandom(_gameManager.PlayerHero);
        reward.OnRewardSelected += RewardSelected;
        return new RewardCardObstacle(reward);
    }

    RewardCard CreateRewardCardTurret()
    {
        RewardTurret reward = ScriptableObject.CreateInstance<RewardTurret>();
        reward.CreateRandom(_gameManager.PlayerHero);
        reward.OnRewardSelected += RewardSelected;
        return new RewardCardTurret(reward);
    }

    void RewardSelected(Reward reward)
    {
        _audioManager.PlayUI("Reward Chosen");

        _rerollButton.SetEnabled(false);
        DOTween.To(x => _rerollButton.style.opacity = x, 1, 0, 0.5f).SetUpdate(true);

        foreach (RewardCard card in _selectedRewardCards)
        {
            if (card.Reward != reward) card.DisableCard();

            card.DisableClicks();
        }

        OnRewardSelected?.Invoke();

        _continueButton = new(callback: MoveAway);
        Add(_continueButton);
    }

    public void MoveAway()
    {
        style.position = Position.Absolute;
        DOTween.To(x => style.opacity = x, 1, 0, 0.5f)
            .SetUpdate(true)
            .OnComplete(() => OnContinueClicked?.Invoke());
    }
}
