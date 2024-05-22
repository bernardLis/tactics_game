﻿using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Lis.Core;
using Lis.Core.Utilities;
using Lis.Units.Hero.Rewards;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

namespace Lis.Battle.Arena
{
    public class RewardScreen : FullScreenElement
    {
        protected const string USSClassName = "reward-screen__";
        const string _ussMain = USSClassName + "main";
        const string _ussLevelUpAnimationContainer = USSClassName + "level-up-animation-container";
        const string _ussFallingElement = USSClassName + "falling-element";
        const string _ussRewardContainer = USSClassName + "reward-container";
        const string _ussRerollContainer = USSClassName + "reroll-container";

        protected readonly AudioManager AudioManager;
        protected readonly HeroManager HeroManager;

        protected VisualElement RewardContainer;
        Label _titleLabel;
        protected readonly List<float> LeftPositions = new();
        protected int RewardElementWidth;
        protected int RewardElementHeight;

        protected readonly List<RewardElement> AllRewardElements = new();

        VisualElement _rerollContainer;
        protected Label RerollsLeft;
        protected RerollButton RerollButton;

        protected string Title;

        protected readonly int NumberOfRewards;

        public event Action OnRewardSelected;

        protected RewardScreen()
        {
            AudioManager = GameManager.GetComponent<AudioManager>();
            StyleSheet ss = GameManager.GetComponent<AddressableManager>()
                .GetStyleSheetByName(StyleSheetType.RewardScreenStyles);
            if (ss != null) styleSheets.Add(ss);

            NumberOfRewards = GameManager.UpgradeBoard.GetUpgradeByName("Reward Count").GetCurrentLevel().Value;
            HeroManager = BattleManager.GetComponent<HeroManager>();

            Content.AddToClassList(_ussMain);

            DisableNavigation();
        }

        public virtual void Initialize()
        {
            AddTitle();
            AddRewardContainer();
            AddRerollButton();
            schedule.Execute(PopulateRewards).StartingIn(600);
        }

        protected void AddTitle()
        {
            _titleLabel = new(Title);
            _titleLabel.style.fontSize = 48;
            _titleLabel.style.opacity = 0;
            Content.Add(_titleLabel);

            DOTween.To(x => _titleLabel.style.opacity = x, 0, 1, 0.5f)
                .SetUpdate(true);
            VisualElement spacer = new();
            spacer.AddToClassList(USSCommonHorizontalSpacer);
            Content.Add(spacer);
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

        void AddRerollButton()
        {
            _rerollContainer = new();
            _rerollContainer.AddToClassList(_ussRerollContainer);
            Content.Add(_rerollContainer);

            if (HeroManager.RewardRerollsAvailable <= 0) return;

            RerollButton = new(callback: RerollReward);
            _rerollContainer.Add(RerollButton);

            RerollsLeft = new($"Rerolls left: {HeroManager.RewardRerollsAvailable}");
            _rerollContainer.Add(RerollsLeft);

            DOTween.To(x => _rerollContainer.style.opacity = x, 0, 1, 0.5f)
                .SetDelay(1f)
                .SetUpdate(true);
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
            if (!reward.CreateRandom(HeroManager.Hero, AllRewardElements)) return null;
            reward.OnRewardSelected += RewardSelected;
            RewardElementTablet element = new(reward);
            return element;
        }

        protected RewardElement CreateRewardCardAbility()
        {
            RewardAbility reward = ScriptableObject.CreateInstance<RewardAbility>();
            if (!reward.CreateRandom(HeroManager.Hero, AllRewardElements)) return null;
            reward.OnRewardSelected += RewardSelected;
            RewardElementAbility element = new(reward);
            return element;
        }

        RewardElement CreateRewardCardGold()
        {
            RewardGold reward = ScriptableObject.CreateInstance<RewardGold>();
            reward.CreateRandom(HeroManager.Hero, AllRewardElements);
            reward.OnRewardSelected += RewardSelected;
            RewardElementGold element = new(reward);
            return element;
        }

        protected RewardElement CreateRewardCardCreature()
        {
            RewardCreature reward = ScriptableObject.CreateInstance<RewardCreature>();
            reward.CreateRandom(HeroManager.Hero, AllRewardElements);
            reward.OnRewardSelected += RewardSelected;
            RewardElementCreature element = new(reward);
            return element;
        }

        protected RewardElement CreateRewardCardPawn()
        {
            RewardPawn reward = ScriptableObject.CreateInstance<RewardPawn>();
            reward.CreateRandom(HeroManager.Hero, AllRewardElements);
            reward.OnRewardSelected += RewardSelected;
            RewardElementPawn element = new(reward);
            return element;
        }


        protected virtual void RewardSelected(Reward reward)
        {
            AudioManager.PlayUI("Reward Chosen");

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
            if (HeroManager.RewardRerollsAvailable <= 0)
            {
                Helpers.DisplayTextOnElement(BattleManager.Root, RerollButton, "Not More Rerolls!", Color.red);
                return;
            }

            HeroManager.RewardRerollsAvailable--;
            RerollsLeft.text = $"Rerolls left: {HeroManager.RewardRerollsAvailable}";
            AudioManager.PlayUI("Dice Roll");

            PopulateRewards();

            if (HeroManager.RewardRerollsAvailable <= 0)
                RerollButton.SetEnabled(false);
        }

        /* EFFECTS */
        protected void MakeItRain()
        {
            List<Sprite> sprites = GameManager.UnitDatabase.CreatureIcons.ToList();
            for (int i = 0; i < 100; i++)
            {
                VisualElement el = new();
                el.style.left = Random.Range(0, Screen.width);
                el.style.backgroundImage = new(sprites[Random.Range(0, sprites.Count)]);
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
            container.AddToClassList(_ussLevelUpAnimationContainer);

            Label label = new("Fight Won!");
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