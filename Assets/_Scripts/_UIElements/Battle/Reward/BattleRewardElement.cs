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

    int _numberOfRewards = 3;

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

        //HERE: disable hero stats
        // _rewardTooltip = new Label("Add stat point");
        // _rewardTooltip.style.opacity = 0;
        // _rewardTooltip.style.fontSize = 32;
        // Add(_rewardTooltip);
        // DOTween.To(x => _rewardTooltip.style.opacity = x, 0, 1, 0.5f).SetUpdate(true);

        AddHeroCard();

        VisualElement spacer = new();
        spacer.AddToClassList(_ussSpacer);
        Add(spacer);

        AddRewardContainer();

        if (_gameManager.PlayerHero.Level.Value == 1)
            LevelOneShow();


    }

    void LevelOneShow()
    {
        _audioManager.PlayDialogue(_audioManager.GetSound("Level 1"));
        /*
        TextPrintingElement el = new("Power surges through you. It feels more like recollecting a memory than acquiring new power. Were you incredibly powerful 'before'?", 10f);
        BattleManager.Instance.Root.Add(el);
        el.style.opacity = 0;
        DOTween.To(x => el.style.opacity = x, 0, 1, 0.5f);
        el.OnFinishedPrinting += () =>
        {
            DOTween.To(x => el.style.opacity = x, 0, 1, 0.5f)
                    .OnComplete(() =>
                    {
                        BattleManager.Instance.Root.Remove(el);
                    });
        };
        */
    }

    void LevelOneClosedShow()
    {
        _audioManager.PlayDialogue(_audioManager.GetSound("On level 1 closed"));
        /*
        string s = "You have remembered how to use fireball. In game press 1 or the icon to summon a powerful ball of fire.";
        TextPrintingElement el = new(s, 8f);
        BattleManager.Instance.Root.Add(el);
        el.style.opacity = 0;
        DOTween.To(x => el.style.opacity = x, 0, 1, 0.5f);
        el.OnFinishedPrinting += () =>
        {
            DOTween.To(x => el.style.opacity = x, 0, 1, 0.5f)
                    .OnComplete(() =>
                    {
                        BattleManager.Instance.Root.Remove(el);
                    });
        };
        */
    }

    void AddHeroCard()
    {
        /* HERE: disable hero stats
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
    */

        HeroCardStats card = new(_gameManager.PlayerHero);
        Add(card);
        card.style.opacity = 0;

        DOTween.To(x => card.style.opacity = x, 0, 1, 0.5f)
            .SetUpdate(true)
            .OnComplete(() =>
            {
                _gameManager.PlayerHero.LevelUp();
                RunCardShow();
            });
    }

    void AddRewardContainer()
    {
        _rewardContainer = new();
        _rewardContainer.style.flexDirection = FlexDirection.Row;
        Add(_rewardContainer);

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
        Add(_rerollButton);
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

        // tutorial
        if (_gameManager.PlayerHero.Level.Value == 1) return;

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
        // Tutorial - forcing a specific reward
        if (_gameManager.PlayerHero.Level.Value == 2)
        {
            for (int i = 0; i < _numberOfRewards; i++)
                _allRewardCards.Add(CreateRewardCardAbility());
            return;
        }

        _allRewardCards.Clear();
        // _allRewardCards.Add(CreateRewardCardItem());
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

        if (_gameManager.PlayerHero.CreatureArmy.Count >= BattleSpire.Instance.Spire.StoreyTroops.MaxTroopsTree.CurrentValue.Value)
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
        if (_gameManager.PlayerHero.Level.Value == 2)
        {
            LevelOneClosedShow();
        }

        style.position = Position.Absolute;
        DOTween.To(x => style.opacity = x, 1, 0, 0.5f)
            .SetUpdate(true)
            .OnComplete(() => OnContinueClicked?.Invoke());
    }
}
