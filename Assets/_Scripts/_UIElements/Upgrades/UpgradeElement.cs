using System.Collections.Generic;




using DG.Tweening;
using UnityEngine.UIElements;

namespace Lis
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

        GameManager _gameManager;

        public Upgrade Upgrade;

        List<VisualElement> _stars = new();

        Label _title;
        GoldElement _price;
        VisualElement _fill;

        string tweenId = "fill";

        IVisualElementScheduledItem _purchaseScheduler;

        protected VisualElement _tooltipElement;

        public UpgradeElement(Upgrade upgrade)
        {
            _gameManager = GameManager.Instance;
            var ss = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.UpgradeStyles);
            if (ss != null)
                styleSheets.Add(ss);

            Upgrade = upgrade;
            upgrade.OnLevelChanged += OnUpgradeLevelChanged;
            AddToClassList(_ussMain);
            AddToClassList(_ussCommonButtonBasic);

            tweenId = "fill" + Upgrade.name;

            AddStars();
            AddIcon();
            AddTitle();
            AddPrice();
            AddFill();

            if (Upgrade.IsMaxLevel()) AddToClassList(_ussFullyUnlocked);

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
            if (Upgrade.IsMaxLevel()) return;
            if (_gameManager.Gold < Upgrade.GetNextLevel().Cost) return;

            // TODO: play sound

            _purchaseScheduler = schedule.Execute(Purchase).StartingIn(1500);
            DOTween.Kill(tweenId);
            DOTween.To(x => _fill.style.height = Length.Percent(x), _fill.style.height.value.value, 90, 1.5f)
                .SetEase(Ease.InOutSine)
                .SetId(tweenId);
        }

        void OnPointerUp(PointerUpEvent evt)
        {
            if (_purchaseScheduler != null) _purchaseScheduler.Pause();
            DOTween.Kill(tweenId);
            DOTween.To(x => _fill.style.height = Length.Percent(x), _fill.style.height.value.value, 0, 0.5f)
                .SetEase(Ease.InOutSine)
                .OnComplete(() => _fill.style.height = Length.Percent(0))
                .SetId(tweenId);
        }

        void AddStars()
        {
            VisualElement starsContainer = new();
            starsContainer.style.flexDirection = FlexDirection.Row;
            Add(starsContainer);

            for (int i = 0; i < Upgrade.Levels.Count; i++)
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
                if (i <= Upgrade.CurrentLevel)
                    _stars[i].AddToClassList(_ussStarPurchased);
            }
        }

        void AddIcon()
        {
            Label icon = new();
            icon.AddToClassList(_ussIcon);
            icon.style.backgroundImage = new StyleBackground(Upgrade.Icon);
            Add(icon);
        }

        void AddTitle()
        {
            _title = new(Helpers.ParseScriptableObjectName(Upgrade.name));
            _title.AddToClassList(_ussTitle);
            Add(_title);
        }

        void AddPrice()
        {
            if (Upgrade.IsMaxLevel()) return;
            _price = new(Upgrade.GetNextLevel().Cost);
            Add(_price);
        }

        void UpdatePrice()
        {
            if (Upgrade.GetNextLevel() != null)
            {
                if (_price == null)
                {
                    AddPrice();
                    return;
                }
                _price.ChangeAmount(Upgrade.GetNextLevel().Cost);
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
            if (Upgrade.IsMaxLevel()) return;

            _gameManager.ChangeGoldValue(-Upgrade.GetNextLevel().Cost);
            Upgrade.Purchased();
            if (Upgrade.IsMaxLevel()) AddToClassList(_ussFullyUnlocked);

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
            _tooltip = new(this, new UpgradeElementTooltip(Upgrade));
            base.DisplayTooltip();
        }

        protected override void HideTooltip()
        {
            base.HideTooltip();
            _tooltipElement = null;
        }
    }
}
