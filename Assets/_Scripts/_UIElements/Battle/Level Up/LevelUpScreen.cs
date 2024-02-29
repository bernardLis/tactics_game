using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Lis.Core;
using Lis.Core.Utilities;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

namespace Lis
{
    public class LevelUpScreen : FullScreenElement
    {
        const string _ussClassName = "level-up-screen__";
        const string _ussMain = _ussClassName + "main";
        const string _ussLevelUpLabel = _ussClassName + "level-up-label";
        const string _ussFallingElement = _ussClassName + "falling-element";
        const string _ussRewardContainer = _ussClassName + "reward-container";

        readonly AudioManager _audioManager;
        readonly BattleHeroManager _battleHeroManager;

        VisualElement _rewardContainer;
        Label _title;
        readonly List<float> _leftPositions = new();
        int _rewardElementWidth;
        int _rewardElementHeight;
        
        readonly List<RewardElement> _allRewardElements = new();

        VisualElement _rerollContainer;
        Label _rerollsLeft;
        RerollButton _rerollButton;

        readonly int _numberOfRewards;

        public event Action OnRewardSelected;

        public LevelUpScreen()
        {
            _audioManager = _gameManager.GetComponent<AudioManager>();
            StyleSheet ss = _gameManager.GetComponent<AddressableManager>()
                .GetStyleSheetByName(StyleSheetType.LevelUpScreenStyles);
            if (ss != null) styleSheets.Add(ss);

            _numberOfRewards = _gameManager.UpgradeBoard.GetUpgradeByName("Reward Count").GetCurrentLevel().Value;
            _battleHeroManager = _battleManager.GetComponent<BattleHeroManager>();

            _content.AddToClassList(_ussMain);

            MakeItRain();
            PlayLevelUpAnimation();
            AddElements();
            DisableNavigation();
        }

        void MakeItRain()
        {
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
        }

        void PlayLevelUpAnimation()
        {
            VisualElement container = new();
            container.style.position = Position.Absolute;
            container.style.width = Length.Percent(80);
            container.style.height = Length.Percent(100);

            Label label = new("Level Up!");
            label.AddToClassList(_ussLevelUpLabel);
            container.Add(label);
            DOTween.To(x => label.style.fontSize = x, 22, 84, 0.5f).SetEase(Ease.OutBack).SetUpdate(true);

            AnimationElement anim = new(_gameManager.GameDatabase.LevelUpAnimationSprites,
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

        void AddElements()
        {
            _title = new("Choose a reward:");
            _title.style.fontSize = 48;
            _title.style.opacity = 0;
            _content.Add(_title);
            DOTween.To(x => _title.style.opacity = x, 0, 1, 0.5f)
                .SetUpdate(true);
            VisualElement spacer = new();
            spacer.AddToClassList(_ussCommonHorizontalSpacer);
            _content.Add(spacer);

            AddRewardContainer();
            AddRerollButton();
            AddHeroElement();
            schedule.Execute(PopulateRewards).StartingIn(600);
        }

        void AddRewardContainer()
        {
            _rewardContainer = new();
            _rewardContainer.AddToClassList(_ussRewardContainer);
            _content.Add(_rewardContainer);

            List<RewardElement> hiddenCards = new();
            for (int i = 0; i < _numberOfRewards; i++)
            {
                RewardElement card = CreateRewardCardGold();
                card.style.width = Length.Percent(100 / _numberOfRewards);
                card.style.height = Length.Percent(100);
                hiddenCards.Add(card);
                card.style.visibility = Visibility.Hidden;
                _rewardContainer.Add(card);
            }

            // styles need a frame to resolve...
            schedule.Execute(() =>
            {
                _rewardElementWidth = Mathf.RoundToInt(hiddenCards[0].layout.width);
                _rewardElementHeight = Mathf.RoundToInt(hiddenCards[0].layout.height);

                foreach (RewardElement el in hiddenCards)
                    _leftPositions.Add(el.layout.x);
            }).StartingIn(100);
        }

        void AddRerollButton()
        {
            _rerollContainer = new();
            _rerollContainer.style.opacity = 0;
            _content.Add(_rerollContainer);

            if (_battleHeroManager.RewardRerollsAvailable <= 0) return;

            _rerollButton = new(callback: RerollReward);
            _rerollContainer.Add(_rerollButton);

            _rerollsLeft = new($"Rerolls left: {_battleHeroManager.RewardRerollsAvailable}");
            _rerollContainer.Add(_rerollsLeft);

            DOTween.To(x => _rerollContainer.style.opacity = x, 0, 1, 0.5f)
                .SetDelay(1f)
                .SetUpdate(true);
        }

        void AddHeroElement()
        {
            Add(new HeroElement(_battleHeroManager.Hero, true));
        }

        void RunCardShow()
        {
            _rerollButton.SetEnabled(false);
            schedule.Execute(() => _rerollButton.SetEnabled(true)).StartingIn(_numberOfRewards * 200);
            for (int i = 0; i < _numberOfRewards; i++)
            {
                RewardElement card = _allRewardElements[i];
                _rewardContainer.Add(card);

                card.style.position = Position.Absolute;
                card.style.left = Screen.width;
                card.style.width = _rewardElementWidth;
                card.style.height = _rewardElementHeight;

                _audioManager.PlayUIDelayed("Paper", 0.2f + i * 0.3f);
                float endLeft = _leftPositions[i];
                DOTween.To(x => card.style.left = x, Screen.width, endLeft, 0.5f)
                    .SetEase(Ease.InFlash)
                    .SetDelay(i * 0.2f)
                    .SetUpdate(true);
            }
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

            if (_battleHeroManager.RewardRerollsAvailable <= 0)
                _rerollButton.SetEnabled(false);
        }

        void PopulateRewards()
        {
            _rewardContainer.Clear();
            CreateRewardCards();
            RunCardShow();
        }

        void CreateRewardCards()
        {
            _allRewardElements.Clear();
            for (int i = 0; i < _numberOfRewards; i++)
            {
                // try giving player ability or stat
                float v = Random.value;
                RewardElement card = v > 0.5f ? CreateRewardCardAbility() : CreateRewardTablet();

                if (card == null && v > 0.5f) card = CreateRewardTablet();
                if (card == null && v <= 0.5f) card = CreateRewardCardAbility();

                // if it is not possible give them gold
                card ??= CreateRewardCardGold();

                _allRewardElements.Add(card);
            }
        }

        RewardElement CreateRewardTablet()
        {
            RewardTablet reward = ScriptableObject.CreateInstance<RewardTablet>();
            if (!reward.CreateRandom(_battleHeroManager.Hero, _allRewardElements)) return null;
            reward.OnRewardSelected += RewardSelected;
            RewardElementTablet element = new(reward);
            return element;
        }

        RewardElement CreateRewardCardAbility()
        {
            RewardAbility reward = ScriptableObject.CreateInstance<RewardAbility>();
            if (!reward.CreateRandom(_battleHeroManager.Hero, _allRewardElements)) return null;
            reward.OnRewardSelected += RewardSelected;
            RewardElementAbility element = new(reward);
            return element;
        }

        RewardElement CreateRewardCardGold()
        {
            RewardGold reward = ScriptableObject.CreateInstance<RewardGold>();
            reward.CreateRandom(_battleHeroManager.Hero, _allRewardElements);
            reward.OnRewardSelected += RewardSelected;
            RewardElementGold element = new(reward);
            return element;
        }

        void RewardSelected(Reward reward)
        {
            _audioManager.PlayUI("Reward Chosen");

            _rerollContainer.style.display = DisplayStyle.None;
            DOTween.To(x => _rerollContainer.style.opacity = x, 1, 0, 0.5f)
                .SetUpdate(true);

            foreach (RewardElement element in _allRewardElements)
            {
                if (element.Reward != reward) element.DisableCard();

                element.DisableClicks();
            }

            OnRewardSelected?.Invoke();

            AddContinueButton();
        }
    }
}