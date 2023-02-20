using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ShopReportElement : ReportElement
{
    Shop _shop;
    VisualElement _itemContainer;
    List<VisualElement> _shopItemContainers = new();
    List<ItemSlot> _itemSlots = new();

   // protected LineTimerElement _expiryTimer;

    GoldElement _rerollPriceGoldElement;

    int _numberOfItems = 2;

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

        _reportContents.style.backgroundImage = new StyleBackground(_gameManager.GameDatabase.ShopWoodSprite);

        AddHeader("Shop", Color.red);

        _itemContainer = new();
        _itemContainer.AddToClassList(_ussItemContainer);
        _reportContents.Add(_itemContainer);

        ShowShopItems();

        VisualElement bottomPanel = new();
        bottomPanel.AddToClassList(_ussBottomPanel);
        bottomPanel.Add(CreateRerollButton());
        _reportContents.Add(bottomPanel);

        float timeTotal = _shop.DateTimeExpired.GetTimeInSeconds() - _shop.DateTimeAdded.GetTimeInSeconds();
        float timeLeft = _shop.DateTimeExpired.GetTimeInSeconds() - _gameManager.GetCurrentTimeInSeconds();
        _expiryTimer = new(timeLeft, timeTotal, false, "Leaving in: ");
        _reportContents.Add(_expiryTimer);
        _expiryTimer.OnTimerFinished += () => DismissReport(false);

        _itemContainer.RegisterCallback<PointerDownEvent>(OnPointerDown, TrickleDown.NoTrickleDown);
    }

    // block report pickup
    void OnPointerDown(PointerDownEvent e) { e.StopImmediatePropagation(); }

    public void ItemBought(Item item) { _shop.ItemBought(item); }

    void ShowShopItems()
    {
        _itemContainer.Clear();
        _itemSlots.Clear();
        _shopItemContainers.Clear();

        for (int i = 0; i < _numberOfItems; i++)
        {
            ItemSlot slot = new();
            _itemSlots.Add(slot);

            VisualElement container = new();
            container.AddToClassList(_ussItem);
            container.Add(slot);
            _shopItemContainers.Add(container);
            _itemContainer.Add(container);
        }

        for (int i = 0; i < _report.Shop.Items.Count; i++)
        {
            if (i > _numberOfItems) // only so much space in the shop
                return;

            ItemElement itemElement = new(_report.Shop.Items[i], this);
            _itemSlots[i].AddItem(itemElement);
            _shopItemContainers[i].Add(new GoldElement(_report.Shop.Items[i].Price));

            DraggableItems draggables = _deskManager.GetComponent<DraggableItems>();
            if (draggables != null)
                draggables.AddDraggableItem(itemElement);
        }
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
