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

namespace Lis.Camp.Building
{
    public class RewardScreen : FullScreenElement
    {
        protected const string USSClassName = "reward-screen__";
        const string _ussMain = USSClassName + "main";
        const string _ussLevelUpAnimationContainer = USSClassName + "level-up-animation-container";
        const string _ussFallingElement = USSClassName + "falling-element";
        const string _ussRewardContainer = USSClassName + "reward-container";
        const string _ussRerollContainer = USSClassName + "reroll-container";

        protected readonly List<RewardElement> AllRewardElements = new();

        protected readonly AudioManager AudioManager;
        protected readonly List<float> LeftPositions = new();

        protected readonly int NumberOfRewards;
        protected Hero Hero;

        VisualElement _rerollContainer;
        Label _titleLabel;
        protected RerollButton RerollButton;
        protected Label RerollsLeft;

        protected VisualElement RewardContainer;
        protected int RewardElementHeight;
        protected int RewardElementWidth;

        protected RewardScreen()
        {
            AudioManager = GameManager.GetComponent<AudioManager>();
            StyleSheet ss = GameManager.GetComponent<AddressableManager>()
                .GetStyleSheetByName(StyleSheetType.RewardScreenStyles);
            if (ss != null) styleSheets.Add(ss);

            NumberOfRewards = GameManager.UpgradeBoard.GetUpgradeByName("Reward Count").GetCurrentLevel().Value;
            Hero = GameManager.Campaign.Hero;

            Content.AddToClassList(_ussMain);

            DisableNavigation();
        }

        public virtual void Initialize()
        {
            AddRewardContainer();
            AddRerollButton();
            schedule.Execute(PopulateRewards).StartingIn(600);
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
            UtilityContainer.Add(_rerollContainer);

            if (Hero.RewardRerolls <= 0) return;

            RerollButton = new(callback: RerollReward);
            _rerollContainer.Add(RerollButton);

            RerollsLeft = new($"Rerolls left: {Hero.RewardRerolls}");
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
        protected RewardElement CreateRewardCardAbility()
        {
            RewardAbility reward = ScriptableObject.CreateInstance<RewardAbility>();
            if (!reward.CreateRandom(Hero, null)) return null;
            reward.OnRewardSelected += RewardSelected;
            RewardElementAbility element = new(reward);
            return element;
        }

        RewardElement CreateRewardCardGold()
        {
            RewardGold reward = ScriptableObject.CreateInstance<RewardGold>();
            reward.CreateRandom(Hero, null);
            reward.OnRewardSelected += RewardSelected;
            RewardElementGold element = new(reward);
            return element;
        }

        protected RewardElement CreateRewardCardPawn()
        {
            RewardPawn reward = ScriptableObject.CreateInstance<RewardPawn>();
            reward.CreateRandom(Hero, null);
            reward.OnRewardSelected += RewardSelected;
            RewardElementPawn element = new(reward);
            return element;
        }

        protected RewardElement CreateRewardCardArmor()
        {
            RewardArmor reward = ScriptableObject.CreateInstance<RewardArmor>();
            reward.CreateRandom(Hero, null);
            reward.OnRewardSelected += RewardSelected;
            RewardElementArmor element = new(reward);
            return element;
        }

        protected virtual void RewardSelected(Reward reward)
        {
            AudioManager.CreateSound().WithSound(AudioManager.GetSound("Reward Chosen")).Play();

            _rerollContainer.style.display = DisplayStyle.None;
            DOTween.To(x => _rerollContainer.style.opacity = x, 1, 0, 0.5f)
                .SetUpdate(true);

            foreach (RewardElement element in AllRewardElements)
            {
                if (element.Reward != reward) element.DisableCard();
                element.DisableClicks();
            }

            AddContinueButton();
        }

        protected virtual void RerollReward()
        {
            if (Hero.RewardRerolls <= 0)
            {
                Helpers.DisplayTextOnElement(FightManager.Root, RerollButton, "Not More Rerolls!", Color.red);
                return;
            }

            Hero.RewardRerolls--;
            RerollsLeft.text = $"Rerolls left: {Hero.RewardRerolls}";
            AudioManager.CreateSound().WithSound(AudioManager.GetSound("Dice Roll")).Play();

            PopulateRewards();

            if (Hero.RewardRerolls <= 0)
                RerollButton.SetEnabled(false);
        }

        /* EFFECTS */
        protected void MakeItRain()
        {
            var sprites = GameManager.UnitDatabase.CreatureIcons.ToList();
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