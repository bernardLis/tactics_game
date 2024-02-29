using System.Collections.Generic;
using DG.Tweening;
using Lis.Core;
using Lis.Core.Utilities;
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

        readonly GameManager _gameManager;

        readonly Upgrade _upgrade;

        readonly List<VisualElement> _stars = new();

        Label _title;
        GoldElement _price;
        VisualElement _fill;

        readonly string _tweenId = "fill";

        IVisualElementScheduledItem _purchaseScheduler;

        public UpgradeElement(Upgrade upgrade)
        {
            _gameManager = GameManager.Instance;
            var ss = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.UpgradeStyles);
            if (ss != null)
                styleSheets.Add(ss);

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

            RegisterCallback<PointerDownEvent>(OnPointerDown);
            RegisterCallback<PointerUpEvent>(OnPointerUp);
        }

        void OnUpgradeLevelChanged()
        {
            UpdateStars();
            UpdatePrice();
        }

        void OnPointerDown(PointerDownEvent evt)
        {
            if (_upgrade.IsMaxLevel()) return;
            if (_gameManager.Gold < _upgrade.GetNextLevel().Cost) return;

            // TODO: play sound

            _purchaseScheduler = schedule.Execute(Purchase).StartingIn(1000);
            DOTween.Kill(_tweenId);
            DOTween.To(x => _fill.style.height = Length.Percent(x), _fill.style.height.value.value, 90, 1f)
                .SetEase(Ease.InOutSine)
                .SetId(_tweenId);
        }

        void OnPointerUp(PointerUpEvent evt)
        {
            if (_purchaseScheduler != null) _purchaseScheduler.Pause();
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
            icon.style.backgroundImage = new StyleBackground(_upgrade.Icon);
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
            if (_upgrade.IsMaxLevel()) AddToClassList(_ussFullyUnlocked);

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