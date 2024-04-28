using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Lis.Core;
using Lis.Core.Utilities;
using Lis.Units.Hero;
using Lis.Units.Hero.Rewards;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

namespace Lis.Battle
{
    public class RewardScreen : FullScreenElement
    {
        const string _ussClassName = "reward-screen__";
        const string _ussMain = _ussClassName + "main";
        const string _ussLevelUpLabel = _ussClassName + "level-up-label";
        const string _ussFallingElement = _ussClassName + "falling-element";
        const string _ussRewardContainer = _ussClassName + "reward-container";
        const string _ussRerollContainer = _ussClassName + "reroll-container";

        readonly AudioManager _audioManager;
        readonly HeroManager _heroManager;

        protected VisualElement RewardContainer;
        protected Label TitleLabel;
        protected readonly List<float> LeftPositions = new();
        protected int RewardElementWidth;
        protected int RewardElementHeight;

        protected readonly List<RewardElement> AllRewardElements = new();

        VisualElement _rerollContainer;
        Label _rerollsLeft;
        RerollButton _rerollButton;

        protected string Title;

        protected readonly int NumberOfRewards;

        public event Action OnRewardSelected;

        protected RewardScreen()
        {
            _audioManager = GameManager.GetComponent<AudioManager>();
            StyleSheet ss = GameManager.GetComponent<AddressableManager>()
                .GetStyleSheetByName(StyleSheetType.RewardScreenStyles);
            if (ss != null) styleSheets.Add(ss);

            NumberOfRewards = GameManager.UpgradeBoard.GetUpgradeByName("Reward Count").GetCurrentLevel().Value;
            _heroManager = BattleManager.GetComponent<HeroManager>();

            Content.AddToClassList(_ussMain);

            DisableNavigation();
        }

        public virtual void Initialize()
        {
            AddTitle();
            AddRewardContainer();
            AddRerollButton();
            AddHeroElement();
            schedule.Execute(PopulateRewards).StartingIn(600);
        }

        protected void AddTitle()
        {
            TitleLabel = new(Title);
            TitleLabel.style.fontSize = 48;
            TitleLabel.style.opacity = 0;
            Content.Add(TitleLabel);

            DOTween.To(x => TitleLabel.style.opacity = x, 0, 1, 0.5f)
                .SetUpdate(true);
            VisualElement spacer = new();
            spacer.AddToClassList(USSCommonHorizontalSpacer);
            Content.Add(spacer);
        }

        protected void AddHeroGoldElement()
        {
            GoldElement goldElement = new(GameManager.Gold);
            GameManager.OnGoldChanged += goldElement.ChangeAmount;
            goldElement.style.position = Position.Absolute;
            goldElement.style.left = Length.Percent(10);
            goldElement.style.top = Length.Percent(3);
            Content.Add(goldElement);
        }


        void AddRewardContainer()
        {
            RewardContainer = new();
            RewardContainer.AddToClassList(_ussRewardContainer);
            Content.Add(RewardContainer);

            List<RewardElement> hiddenCards = new();
            for (int i = 0; i < NumberOfRewards; i++)
            {
                RewardElement card = CreateRewardCardGold();
                card.style.width = Length.Percent(100 / NumberOfRewards);
                card.style.height = Length.Percent(100);
                hiddenCards.Add(card);
                card.style.visibility = Visibility.Hidden;
                RewardContainer.Add(card);
            }

            // styles need a frame to resolve...
            schedule.Execute(() =>
            {
                RewardElementWidth = Mathf.RoundToInt(hiddenCards[0].layout.width);
                RewardElementHeight = Mathf.RoundToInt(hiddenCards[0].layout.height);

                foreach (RewardElement el in hiddenCards)
                    LeftPositions.Add(el.layout.x);
            }).StartingIn(100);
        }

        protected void AddRerollButton()
        {
            _rerollContainer = new();
            _rerollContainer.AddToClassList(_ussRerollContainer);
            Content.Add(_rerollContainer);

            if (_heroManager.RewardRerollsAvailable <= 0) return;

            _rerollButton = new(callback: RerollReward);
            _rerollContainer.Add(_rerollButton);

            _rerollsLeft = new($"Rerolls left: {_heroManager.RewardRerollsAvailable}");
            _rerollContainer.Add(_rerollsLeft);

            DOTween.To(x => _rerollContainer.style.opacity = x, 0, 1, 0.5f)
                .SetDelay(1f)
                .SetUpdate(true);
        }

        protected void AddHeroElement()
        {
            Add(new HeroElement(_heroManager.Hero, true));
        }

        void PopulateRewards()
        {
            RewardContainer.Clear();
            CreateRewardCards();
        }

        protected virtual void CreateRewardCards()
        {
            AllRewardElements.Clear();
            for (int i = 0; i < NumberOfRewards; i++)
            {
                RewardElement el = ChooseRewardElement();
                el ??= CreateRewardCardGold(); // backup

                el.style.position = Position.Absolute;
                float endLeft = LeftPositions[i];
                el.style.left = endLeft;

                el.style.width = RewardElementWidth;
                el.style.height = RewardElementHeight;

                RewardContainer.Add(el);
                AllRewardElements.Add(el);
            }
        }

        protected virtual RewardElement ChooseRewardElement()
        {
            // meant to be overwritten
            return null;
        }

        /* REWARDS */
        protected RewardElement CreateRewardTablet()
        {
            RewardTablet reward = ScriptableObject.CreateInstance<RewardTablet>();
            if (!reward.CreateRandom(_heroManager.Hero, AllRewardElements)) return null;
            reward.OnRewardSelected += RewardSelected;
            RewardElementTablet element = new(reward);
            return element;
        }

        protected RewardElement CreateRewardCardAbility()
        {
            RewardAbility reward = ScriptableObject.CreateInstance<RewardAbility>();
            if (!reward.CreateRandom(_heroManager.Hero, AllRewardElements)) return null;
            reward.OnRewardSelected += RewardSelected;
            RewardElementAbility element = new(reward);
            return element;
        }

        RewardElement CreateRewardCardGold()
        {
            RewardGold reward = ScriptableObject.CreateInstance<RewardGold>();
            reward.CreateRandom(_heroManager.Hero, AllRewardElements);
            reward.OnRewardSelected += RewardSelected;
            RewardElementGold element = new(reward);
            return element;
        }

        protected RewardElement CreateRewardCardCreature()
        {
            RewardCreature reward = ScriptableObject.CreateInstance<RewardCreature>();
            reward.CreateRandom(_heroManager.Hero, AllRewardElements);
            reward.OnRewardSelected += RewardSelected;

            RewardElementCreature element = new(reward);
            AllRewardElements.Add(element);
            RewardContainer.Add(element);
            return element;
        }

        protected virtual void RewardSelected(Reward reward)
        {
            _audioManager.PlayUI("Reward Chosen");

            _rerollContainer.style.display = DisplayStyle.None;
            DOTween.To(x => _rerollContainer.style.opacity = x, 1, 0, 0.5f)
                .SetUpdate(true);

            foreach (RewardElement element in AllRewardElements)
            {
                if (element.Reward != reward) element.DisableCard();
                element.DisableClicks();
            }

            OnRewardSelected?.Invoke();

            AddContinueButton();
        }

        protected virtual void RerollReward()
        {
            if (_heroManager.RewardRerollsAvailable <= 0)
            {
                Helpers.DisplayTextOnElement(BattleManager.Root, _rerollButton, "Not More Rerolls!", Color.red);
                return;
            }

            _heroManager.RewardRerollsAvailable--;
            _rerollsLeft.text = $"Rerolls left: {_heroManager.RewardRerollsAvailable}";
            _audioManager.PlayUI("Dice Roll");

            PopulateRewards();

            if (_heroManager.RewardRerollsAvailable <= 0)
                _rerollButton.SetEnabled(false);
        }

        /* EFFECTS */
        protected void MakeItRain()
        {
            List<Sprite> sprites = GameManager.UnitDatabase.CreatureIcons.ToList();
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
        }

        protected void PlayLevelUpAnimation()
        {
            VisualElement container = new();
            container.style.position = Position.Absolute;
            container.style.width = Length.Percent(80);
            container.style.height = Length.Percent(100);

            Label label = new("Fight Won!");
            label.AddToClassList(_ussLevelUpLabel);
            container.Add(label);
            DOTween.To(x => label.style.fontSize = x, 22, 84, 0.5f).SetEase(Ease.OutBack).SetUpdate(true);

            AnimationElement anim = new(GameManager.GameDatabase.LevelUpAnimationSprites,
                50, false);
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
    }
}