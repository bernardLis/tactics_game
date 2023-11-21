using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using DG.Tweening;

public class GlobalUpgradeElement : ElementWithTooltip
{
    const string _ussCommonButtonBasic = "common__button-basic";
    const string _ussClassName = "global-upgrade__";
    const string _ussMain = _ussClassName + "main";
    const string _ussTitle = _ussClassName + "title";
    const string _ussIcon = _ussClassName + "icon";
    const string _ussStar = _ussClassName + "star";
    const string _ussStarPurchased = _ussClassName + "star-purchased";

    GameManager _gameManager;

    public GlobalUpgrade GlobalUpgrade;

    List<VisualElement> _stars = new();

    Label _title;
    GoldElement _price;
    VisualElement _fill;

    IVisualElementScheduledItem _purchaseScheduler;

    public GlobalUpgradeElement(GlobalUpgrade globalUpgrade)
    {
        _gameManager = GameManager.Instance;
        var ss = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.GlobalUpgradeStyles);
        if (ss != null)
            styleSheets.Add(ss);

        GlobalUpgrade = globalUpgrade;
        AddToClassList(_ussMain);
        AddToClassList(_ussCommonButtonBasic);

        AddStars();
        AddIcon();
        AddTitle();
        AddPrice();
        AddFill();

        RegisterCallback<PointerDownEvent>(OnPointerDown);
        RegisterCallback<PointerUpEvent>(OnPointerUp);
    }

    void OnPointerDown(PointerDownEvent evt)
    {
        if (GlobalUpgrade.IsMaxLevel()) return;

        // play sound

        // if the player holds the button for a certain amount of time, purchase the upgrade
        // start filling in the element with color
        // if the player releases the button, cancel the purchase

        _purchaseScheduler = schedule.Execute(Purchase).StartingIn(2000);
        DOTween.Kill("fill");
        DOTween.To(x => _fill.style.height = Length.Percent(x), _fill.style.height.value.value, 84, 2f)
                .SetEase(Ease.InOutSine)
                .SetId("fill");
    }

    void OnPointerUp(PointerUpEvent evt)
    {
        _purchaseScheduler.Pause();
        DOTween.Kill("fill");
        DOTween.To(x => _fill.style.height = Length.Percent(x), _fill.style.height.value.value, 0, 0.5f)
                .SetEase(Ease.InOutSine)
                .OnComplete(() => _fill.style.height = Length.Percent(0))
                .SetId("fill");

        // if (_fill != null) _fill.style.height = Length.Percent(0);
    }

    void AddStars()
    {
        VisualElement starsContainer = new();
        starsContainer.style.flexDirection = FlexDirection.Row;
        Add(starsContainer);

        for (int i = 0; i < GlobalUpgrade.Levels.Count; i++)
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
            if (i < GlobalUpgrade.CurrentLevel)
                _stars[i].AddToClassList(_ussStarPurchased);
    }

    void AddIcon()
    {
        Label icon = new();
        icon.AddToClassList(_ussIcon);
        icon.style.backgroundImage = new StyleBackground(GlobalUpgrade.Icon);
        Add(icon);
    }

    void AddTitle()
    {
        _title = new(Helpers.ParseScriptableObjectName(GlobalUpgrade.name));
        _title.AddToClassList(_ussTitle);
        Add(_title);
    }

    void AddPrice()
    {
        if (GlobalUpgrade.IsMaxLevel()) return;
        _price = new(GlobalUpgrade.GetNextLevel().Cost);
        Add(_price);
    }

    void AddFill()
    {
        _fill = new();
        // HERE: styles
        _fill.style.position = Position.Absolute;
        _fill.style.width = Length.Percent(82);
        _fill.style.height = Length.Percent(0);
        _fill.style.backgroundColor = new StyleColor(Color.yellow);
        _fill.style.opacity = 0.5f;
        _fill.style.bottom = Length.Percent(10);
        Add(_fill);
    }

    void Purchase()
    {
        _gameManager.ChangeGoldValue(GlobalUpgrade.GetNextLevel().Cost);
        GlobalUpgrade.Purchased();

        _price.ChangeAmount(GlobalUpgrade.GetNextLevel().Cost);
        UpdateStars();
        DisplayTooltip();
    }

    protected override void DisplayTooltip()
    {
        _tooltip = new(this, new Label(GlobalUpgrade.GetNextLevel().Description));
        base.DisplayTooltip();
    }
}
