using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ShopReportElement : ReportElement
{
    Shop _shop;
    VisualElement _itemContainer;
    Label _durationLabel;

    GoldElement _rerollPriceGoldElement;

    const string ussCommonTextPrimary = "common__text-primary";

    const string _ussClassName = "shop";
    const string _ussItemContainer = _ussClassName + "__item-container";
    const string _ussItem = _ussClassName + "__item";
    const string _ussBottomPanel = _ussClassName + "__bottom-panel";
    const string _ussRerollContainer = _ussClassName + "__reroll-container";

    const string _ussRerollButton = _ussClassName + "__reroll-button";


    public ShopReportElement(VisualElement parent, Report report) : base(parent, report)
    {
        var ss = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.ShopReportStyles);
        if (ss != null)
            styleSheets.Add(ss);

        _shop = report.Shop;

        AddHeader("Shop", Color.red);

        _itemContainer = new();
        _itemContainer.AddToClassList(_ussItemContainer);
        _reportContents.Add(_itemContainer);
        ShowShopItems();

        VisualElement bottomPanel = new();
        bottomPanel.AddToClassList(_ussBottomPanel);
        bottomPanel.Add(CreateRerollButton());
        _reportContents.Add(bottomPanel);

        _durationLabel = new();
        _reportContents.Add(_durationLabel);
        UpdateDuration();
        _shop.OnDurationChanged += UpdateDuration;
    }

    void ShowShopItems()
    {
        _itemContainer.Clear();
        foreach (var item in _report.Shop.Items)
        {
            // so here I want 3 item slots that are filled with items 
            VisualElement container = new();
            container.AddToClassList(_ussItem);

            ItemElement itemVisual = new(item);
            ItemSlot itemSlot = new();
            itemSlot.AddItem(itemVisual);

            //https://docs.unity3d.com/2020.1/Documentation/Manual/UIE-Events-Handling.html
            // HERE: drag items
            //itemVisual.RegisterCallback<PointerDownEvent>(OnShopItemPointerDown);

            container.Add(itemSlot);
            container.Add(new GoldElement(item.Price));

            _itemContainer.Add(container);
        }
    }

    void UpdateDuration()
    {
        if (_durationLabel == null)
            return;
        _durationLabel.text = ($"Shop is leaving in {_shop.Duration} days");

        if (_shop.Duration <= 0)
            DismissReport(false);
    }

    VisualElement CreateRerollButton()
    {
        VisualElement shopRerollContainer = new();
        shopRerollContainer.AddToClassList(_ussRerollContainer);
        MyButton rerollButton = new MyButton("", _ussRerollButton, Reroll);
        _rerollPriceGoldElement = new(_shop.RerollCost);

        shopRerollContainer.Add(rerollButton);
        shopRerollContainer.Add(_rerollPriceGoldElement);

        return shopRerollContainer;
    }

    void Reroll()
    {
        if (_gameManager.Gold < _shop.RerollCost)
        {
            Helpers.DisplayTextOnElement(_deskManager.Root, _rerollPriceGoldElement, "Insufficient funds", Color.red);
            return;
        }
        _gameManager.ChangeGoldValue(-_shop.RerollCost);
        _shop.RerollItems();
        ShowShopItems();
    }

}
