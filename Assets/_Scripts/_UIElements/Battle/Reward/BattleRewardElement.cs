using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;
using DG.Tweening;

public class BattleRewardElement : FullScreenElement
{
    const string _ussClassName = "battle-reward__";
    const string _ussMain = _ussClassName + "main";
    const string _ussLevelUpLabel = _ussClassName + "level-up-label";

    AudioManager _audioManager;

    VisualElement _rewardContainer;
    Label _title;
    List<RewardCard> _hiddenCards = new();

    List<RewardCard> _allRewardCards = new();
    List<RewardCard> _selectedRewardCards = new();

    RerollButton _rerollButton;

    int _numberOfRewards = 3;

    public event Action OnRewardSelected;
    public BattleRewardElement()
    {
        _audioManager = _gameManager.GetComponent<AudioManager>();
        var ss = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.BattleRewardStyles);
        if (ss != null) styleSheets.Add(ss);

        _content.AddToClassList(_ussMain);

        PlayLevelUpAnimation();
        AddElements();
    }

    void PlayLevelUpAnimation()
    {
        VisualElement container = new();
        container.style.position = Position.Absolute;
        container.style.width = Length.Percent(100);
        container.style.height = Length.Percent(100);

        Label label = new("Level Up!");
        label.AddToClassList(_ussLevelUpLabel);
        container.Add(label);
        DOTween.To(x => label.style.fontSize = x, 22, 84, 0.5f).SetEase(Ease.OutBack).SetUpdate(true);

        AnimationElement anim = new(_gameManager.GameDatabase.LevelUpAnimationSprites, 50, false);
        container.Add(anim);
        anim.PlayAnimation();

        Add(container);
        anim.OnAnimationFinished += () =>
        {
            DOTween.To(x => container.style.opacity = x, 1, 0, 0.5f)
                    .SetUpdate(true)
                    .OnComplete(() => Remove(container));
        };
    }

    void AddElements()
    {
        _title = new("Choose a reward:");
        _title.style.fontSize = 48;
        _title.style.opacity = 0;
        _content.Add(_title);
        DOTween.To(x => _title.style.opacity = x, 0, 1, 0.5f)
            .SetUpdate(true)
            .OnComplete(() => RunCardShow());

        VisualElement spacer = new();
        spacer.AddToClassList(_ussCommonHorizontalSpacer);
        _content.Add(spacer);

        AddRewardContainer();

        DisableNavigation();
    }

    void AddRewardContainer()
    {
        _rewardContainer = new();
        _rewardContainer.style.flexDirection = FlexDirection.Row;
        _content.Add(_rewardContainer);

        _hiddenCards = new();
        for (int i = 0; i < _numberOfRewards; i++)
        {
            RewardCard card = CreateRewardCardGold();
            _hiddenCards.Add(card);
            card.style.visibility = Visibility.Hidden;
            _rewardContainer.Add(card);
        }

        _rerollButton = new(callback: RerollReward);
        _rerollButton.style.opacity = 0;
        _rerollButton.style.visibility = Visibility.Hidden;
        _content.Add(_rerollButton);
    }

    void RunCardShow()
    {
        schedule.Execute(() =>
        {
            CreateRewardCards();
            for (int i = 0; i < _numberOfRewards; i++)
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

        _rerollButton.style.visibility = Visibility.Visible;
        DOTween.To(x => _rerollButton.style.opacity = x, 0, 1, 0.5f)
            .SetDelay(0.5f)
            .SetUpdate(true);
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
        _allRewardCards.Clear();
        _allRewardCards.Add(CreateRewardCardHeroStat());
        _allRewardCards.Add(CreateRewardCardAbility());
        _allRewardCards.Add(CreateRewardCardGold());
        _allRewardCards.Add(CreateRewardCardArmy());
        _allRewardCards.Add(CreateRewardCardObstacle());
        _allRewardCards.Add(CreateRewardCardTurret());
    }

    void ChooseRewardCards()
    {
        for (int i = 0; i < _numberOfRewards; i++)
        {
            RewardCard card = _allRewardCards[Random.Range(0, _allRewardCards.Count)];
            _allRewardCards.Remove(card);
            _selectedRewardCards.Add(card);
            _rewardContainer.Add(card);
        }
    }

    RewardCard CreateRewardCardHeroStat()
    {
        RewardHeroStat reward = ScriptableObject.CreateInstance<RewardHeroStat>();
        reward.CreateRandom(_gameManager.PlayerHero);
        reward.OnRewardSelected += RewardSelected;
        RewardCardHeroStat card = new(reward);
        return card;
    }

    RewardCard CreateRewardCardAbility()
    {
        RewardAbility reward = ScriptableObject.CreateInstance<RewardAbility>();
        reward.CreateRandom(_gameManager.PlayerHero);
        reward.OnRewardSelected += RewardSelected;
        RewardCardAbility card = new(reward);
        return card;
    }

    RewardCard CreateRewardCardGold()
    {
        RewardGold reward = ScriptableObject.CreateInstance<RewardGold>();
        reward.CreateRandom(_gameManager.PlayerHero);
        reward.OnRewardSelected += RewardSelected;
        RewardCardGold card = new(reward);
        return card;
    }

    RewardCard CreateRewardCardArmy()
    {
        RewardCreature reward = ScriptableObject.CreateInstance<RewardCreature>();
        reward.CreateRandom(_gameManager.PlayerHero);
        reward.OnRewardSelected += RewardSelected;
        RewardCard card = new RewardCardCreature(reward);

        if (_gameManager.PlayerHero.CreatureArmy.Count >= 7)// HERE: troops limit
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

        AddContinueButton();
    }

    public override void Hide()
    {
        base.Hide();
    }
}
