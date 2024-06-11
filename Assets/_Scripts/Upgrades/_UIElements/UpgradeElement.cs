using System.Collections.Generic;
using DG.Tweening;
using Lis.Core;
using Lis.Core.Utilities;
using UnityEngine;
using UnityEngine.UIElements;

namespace Lis.Upgrades
{
    public class UpgradeElement : ElementWithTooltip
    {
        const string _ussCommonButtonBasic = "common__button-basic";
        const string _ussClassName = "upgrade__";
        const string _ussMain = _ussClassName + "main";
        const string _ussFullyUnlocked = _ussClassName + "fully-unlocked";

        const string _ussTitle = _ussClassName + "title";
        const string _ussIcon = _ussClassName + "icon";
        const string _ussStar = _ussClassName + "star";
        const string _ussStarPurchased = _ussClassName + "star-purchased";
        const string _ussFill = _ussClassName + "fill";
        readonly AudioManager _audioManager;

        readonly GameManager _gameManager;

        readonly List<VisualElement> _stars = new();

        readonly string _tweenId = "fill";

        readonly Upgrade _upgrade;
        VisualElement _fill;
        GoldElement _price;

        IVisualElementScheduledItem _purchaseScheduler;

        AudioSource _swooshAudioSource;

        Label _title;

        public UpgradeElement(Upgrade upgrade)
        {
            _gameManager = GameManager.Instance;
            _audioManager = AudioManager.Instance;
            StyleSheet ss = _gameManager.GetComponent<AddressableManager>()
                .GetStyleSheetByName(StyleSheetType.UpgradeStyles);
            if (ss != null) styleSheets.Add(ss);

            _upgrade = upgrade;
            upgrade.OnLevelChanged += OnUpgradeLevelChanged;
            AddToClassList(_ussMain);
            AddToClassList(_ussCommonButtonBasic);

            _tweenId = "fill" + _upgrade.name;

            AddStars();
            AddIcon();
            AddTitle();
            AddPrice();
            AddFill();

            if (_upgrade.IsMaxLevel()) AddToClassList(_ussFullyUnlocked);

            RegisterCallback<PointerEnterEvent>(OnPointerEnter);
            RegisterCallback<PointerDownEvent>(OnPointerDown);
            RegisterCallback<PointerUpEvent>(OnPointerUp);
        }

        void OnUpgradeLevelChanged()
        {
            UpdateStars();
            UpdatePrice();

            if (_upgrade.IsMaxLevel()) AddToClassList(_ussFullyUnlocked);
            else RemoveFromClassList(_ussFullyUnlocked);
        }

        void OnPointerEnter(PointerEnterEvent evt)
        {
            _audioManager.PlayUI("UI Click");
        }

        void OnPointerDown(PointerDownEvent evt)
        {
            if (_upgrade.IsMaxLevel())
            {
                Helpers.DisplayTextOnElement(parent, this, "Max Level", Color.red);
                _audioManager.PlayUI("Upgrade - Max Level");
                return;
            }

            if (_gameManager.Gold < _upgrade.GetNextLevel().Cost)
            {
                Helpers.DisplayTextOnElement(parent, this, "$$$", Color.red);
                _audioManager.PlayUI("Upgrade - Not Enough Gold");
                return;
            }

            _swooshAudioSource = _audioManager.PlayUI("Upgrade - Swoosh");

            _purchaseScheduler = schedule.Execute(Purchase).StartingIn(1000);
            DOTween.Kill(_tweenId);
            DOTween.To(x => _fill.style.height = Length.Percent(x), _fill.style.height.value.value, 90, 1f)
                .SetEase(Ease.InOutSine)
                .SetId(_tweenId);
        }

        void OnPointerUp(PointerUpEvent evt)
        {
            if (_purchaseScheduler != null) _purchaseScheduler.Pause();
            if (_swooshAudioSource != null)
            {
                _swooshAudioSource.Stop();
                _swooshAudioSource = null;
            }

            DOTween.Kill(_tweenId);
            DOTween.To(x => _fill.style.height = Length.Percent(x), _fill.style.height.value.value, 0, 0.3f)
                .SetEase(Ease.InOutSine)
                .OnComplete(() => _fill.style.height = Length.Percent(0))
                .SetId(_tweenId);
        }

        void AddStars()
        {
            VisualElement starsContainer = new();
            starsContainer.style.flexDirection = FlexDirection.Row;
            Add(starsContainer);

            for (int i = 0; i < _upgrade.Levels.Count; i++)
            {
                VisualElement star = new();
                star.AddToClassList(_ussStar);
                _stars.Add(star);
                starsContainer.Add(star);
            }

            UpdateStars();
        }

        void UpdateStars()
        {
            for (int i = 0; i < _stars.Count; i++)
            {
                _stars[i].RemoveFromClassList(_ussStarPurchased);
                if (i <= _upgrade.CurrentLevel)
                    _stars[i].AddToClassList(_ussStarPurchased);
            }
        }

        void AddIcon()
        {
            Label icon = new();
            icon.AddToClassList(_ussIcon);
            icon.style.backgroundImage = new(_upgrade.Icon);
            Add(icon);
        }

        void AddTitle()
        {
            _title = new(Helpers.ParseScriptableObjectName(_upgrade.name));
            _title.AddToClassList(_ussTitle);
            Add(_title);
        }

        void AddPrice()
        {
            if (_upgrade.IsMaxLevel()) return;
            _price = new(_upgrade.GetNextLevel().Cost);
            Add(_price);
        }

        void UpdatePrice()
        {
            if (_upgrade.GetNextLevel() != null)
            {
                if (_price == null)
                {
                    AddPrice();
                    return;
                }

                _price.ChangeAmount(_upgrade.GetNextLevel().Cost);
                return;
            }

            if (_price == null) return;
            _price.RemoveFromHierarchy();
            _price = null;
        }

        void AddFill()
        {
            _fill = new();
            _fill.AddToClassList(_ussFill);
            Add(_fill);
        }

        void Purchase()
        {
            if (_upgrade.IsMaxLevel()) return;

            _gameManager.ChangeGoldValue(-_upgrade.GetNextLevel().Cost);
            _upgrade.Purchased();

            if (_swooshAudioSource != null)
            {
                _swooshAudioSource.Stop();
                _swooshAudioSource = null;
            }

            _audioManager.PlayUI("Upgrade - Bought");
            DisplayTooltip();

            DOTween.To(x => _fill.style.opacity = x, _fill.style.opacity.value, 1, 0.1f)
                .SetLoops(2, LoopType.Yoyo)
                .OnComplete(() =>
                {
                    _fill.style.opacity = 0.5f;
                    _fill.style.height = Length.Percent(0);
                });
        }

        protected override void DisplayTooltip()
        {
            _tooltip = new(this, new UpgradeElementTooltip(_upgrade));
            base.DisplayTooltip();
        }
    }
}