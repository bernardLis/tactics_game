using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;
using DG.Tweening;
using System.Linq;

public class BattleRewardElement : FullScreenElement
{
    const string _ussClassName = "battle-reward__";
    const string _ussMain = _ussClassName + "main";
    const string _ussLevelUpLabel = _ussClassName + "level-up-label";
    const string _ussFallingElement = _ussClassName + "falling-element";

    AudioManager _audioManager;
    BattleHeroManager _battleHeroManager;

    VisualElement _rewardContainer;
    Label _title;
    List<RewardCard> _hiddenCards = new();

    List<RewardCard> _allRewardCards = new();

    VisualElement _rerollContainer;
    Label _rerollsLeft;
    RerollButton _rerollButton;

    int _numberOfRewards = 2;

    public event Action OnRewardSelected;
    public BattleRewardElement()
    {
        // HERE: testing falling ui objects
        _gameManager = GameManager.Instance;
        List<Sprite> sprites = _gameManager.EntityDatabase.CreatureIcons.ToList();
        for (int i = 0; i < 100; i++)
        {
            VisualElement el = new();
            el.style.left = Random.Range(0, Screen.width);
            el.style.backgroundImage = new StyleBackground(sprites[Random.Range(0, sprites.Count)]);
            el.AddToClassList(_ussFallingElement);
            Add(el);
            float time = Random.Range(1f, 4f);
            DOTween.To(x => el.style.top = x, 0, Screen.height, time)
                    .SetEase(Ease.InOutSine)
                    .SetUpdate(true);
            DOTween.To(x => el.style.opacity = x, 1, 0, time)
                    .SetEase(Ease.InOutSine)
                    .SetUpdate(true)
                    .OnComplete(() => Remove(el));
        }

        _numberOfRewards = _gameManager.GlobalUpgradeBoard.RewardNumber.GetCurrentLevel().Value;

        _audioManager = _gameManager.GetComponent<AudioManager>();
        var ss = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.BattleRewardStyles);
        if (ss != null) styleSheets.Add(ss);

        _battleHeroManager = BattleManager.Instance.GetComponent<BattleHeroManager>();

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
        AddRerollButton();
        AddHeroElement();

        DisableNavigation();
    }

    void AddRewardContainer()
    {
        _rewardContainer = new();
        _rewardContainer.style.flexDirection = FlexDirection.Row;
        _rewardContainer.style.flexGrow = 1;
        _content.Add(_rewardContainer);

        _hiddenCards = new();
        for (int i = 0; i < _numberOfRewards; i++)
        {
            RewardCard card = CreateRewardCardGold();
            _hiddenCards.Add(card);
            card.style.visibility = Visibility.Hidden;
            _rewardContainer.Add(card);
        }
    }

    void AddRerollButton()
    {
        if (_battleHeroManager.RewardRerollsAvailable <= 0) return;
        _rerollContainer = new();
        _rerollContainer.style.opacity = 0;
        _content.Add(_rerollContainer);

        _rerollButton = new(callback: RerollReward);
        _rerollContainer.Add(_rerollButton);

        _rerollsLeft = new($"Rerolls left: {_battleHeroManager.RewardRerollsAvailable}");
        _rerollContainer.Add(_rerollsLeft);
    }

    void AddHeroElement()
    {
        _content.Add(new HeroBattleElement(_gameManager.PlayerHero));
    }

    void RunCardShow()
    {
        schedule.Execute(() =>
        {
            CreateRewardCards();
            for (int i = 0; i < _numberOfRewards; i++)
            {
                RewardCard card = _allRewardCards[i];
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
        }).StartingIn(100);

        if (_rerollContainer == null) return;
        DOTween.To(x => _rerollContainer.style.opacity = x, 0, 1, 0.5f)
            .SetDelay(0.5f)
            .SetUpdate(true);
    }

    void RerollReward()
    {
        if (_battleHeroManager.RewardRerollsAvailable <= 0)
        {
            Helpers.DisplayTextOnElement(BattleManager.Instance.GetComponent<UIDocument>().rootVisualElement,
                _rerollButton, "Not More Rerolls!", Color.red);
            return;
        }

        _battleHeroManager.RewardRerollsAvailable--;
        _rerollsLeft.text = $"Rerolls left: {_battleHeroManager.RewardRerollsAvailable}";
        _audioManager.PlayUI("Dice Roll");

        PopulateRewards();
        UpdateRerollButton();
    }

    void UpdateRerollButton()
    {
        if (_battleHeroManager.RewardRerollsAvailable <= 0)
        {
            _rerollButton.SetEnabled(false);
            return;
        }
        _rerollButton.SetEnabled(true);
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
        for (int i = 0; i < _numberOfRewards; i++)
        {
            // try giving player ability or stat
            RewardCard card;
            float v = Random.value;
            if (v > 0.5f) card = CreateRewardCardAbility();
            else card = CreateRewardCardHeroStat();

            if (card == null && v > 0.5f) card = CreateRewardCardHeroStat();
            if (card == null && v <= 0.5f) card = CreateRewardCardAbility();

            // if it is not possible give them gold
            if (card == null) card = CreateRewardCardGold();

            _allRewardCards.Add(card);
        }
    }

    void ChooseRewardCards()
    {
        for (int i = 0; i < _numberOfRewards; i++)
        {
            RewardCard card = _allRewardCards[Random.Range(0, _allRewardCards.Count)];
            _allRewardCards.Remove(card);
            _rewardContainer.Add(card);
        }
    }

    RewardCard CreateRewardCardHeroStat()
    {
        RewardHeroStat reward = ScriptableObject.CreateInstance<RewardHeroStat>();
        if (!reward.CreateRandom(_gameManager.PlayerHero, _allRewardCards)) return null;

        reward.OnRewardSelected += RewardSelected;
        RewardCardHeroStat card = new(reward);
        return card;
    }

    RewardCard CreateRewardCardAbility()
    {
        RewardAbility reward = ScriptableObject.CreateInstance<RewardAbility>();
        if (!reward.CreateRandom(_gameManager.PlayerHero, _allRewardCards)) return null;
        reward.OnRewardSelected += RewardSelected;
        RewardCardAbility card = new(reward);
        return card;
    }

    RewardCard CreateRewardCardGold()
    {
        RewardGold reward = ScriptableObject.CreateInstance<RewardGold>();
        reward.CreateRandom(_gameManager.PlayerHero, _allRewardCards);
        reward.OnRewardSelected += RewardSelected;
        RewardCardGold card = new(reward);
        return card;
    }

    void RewardSelected(Reward reward)
    {
        _audioManager.PlayUI("Reward Chosen");

        _rerollButton.SetEnabled(false);
        DOTween.To(x => _rerollButton.style.opacity = x, 1, 0, 0.5f).SetUpdate(true);

        foreach (RewardCard card in _allRewardCards)
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
