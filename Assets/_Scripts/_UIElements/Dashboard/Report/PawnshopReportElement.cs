using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PawnshopReportElement : ReportElement
{
    DraggableItems _draggableItems;

    ItemSlot _sellSlot;
    GoldElement _goldElement;
    MyButton _sellButton;
    int _dayAdded;

    const string _ussClassName = "pawnshop";
    const string _ussSellButton = _ussClassName + "__sell-button";

    public PawnshopReportElement(VisualElement parent, Report report) : base(parent, report)
    {
        _dayAdded = _gameManager.Day;
        _draggableItems = _deskManager.GetComponent<DraggableItems>();

        var ss = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.PawnshopReportStyles);
        if (ss != null)
            styleSheets.Add(ss);

        AddHeader("Pawnshop", Color.magenta);

        Label instructions = new("Drag item to sell it");
        _reportContents.Add(instructions);

        _sellSlot = new();
        _sellSlot.OnItemAdded += OnSellItemAdded;
        _sellSlot.OnItemRemoved += OnSellItemRemoved;

        if (report.Item != null)
        {
            ItemElement el = new(report.Item);
            _draggableItems.AddDraggableItem(el);
            _sellSlot.AddItemNoDelegates(el);
            _sellSlot.ItemElement.RegisterCallback<PointerDownEvent>(BlockReportPickup, TrickleDown.NoTrickleDown);
        }

        _reportContents.Add(_sellSlot);
        _draggableItems.AddSlot(_sellSlot);

        _goldElement = new(0);
        _reportContents.Add(_goldElement);

        _sellButton = new("Sell", _ussSellButton, Sell);
        _sellButton.AddToClassList(_ussCommonTextPrimary);
        _sellButton.SetEnabled(false);
        _reportContents.Add(_sellButton);
    }

    protected override void OnDayPassed(int day)
    {
        if (_dayAdded == day)
            return;
        if (_sellSlot.ItemElement != null)
            Sell();
        DismissReport();
    }

    void BlockReportPickup(PointerDownEvent e) { e.StopImmediatePropagation(); }

    void OnSellItemAdded(ItemElement el)
    {
        _report.Item = el.Item;
        _goldElement.ChangeAmount(el.Item.GetSellValue());
        el.RegisterCallback<PointerDownEvent>(BlockReportPickup, TrickleDown.NoTrickleDown);

        _sellButton.SetEnabled(true);
    }

    void OnSellItemRemoved(ItemElement el)
    {
        _report.Item = null;
        _goldElement.ChangeAmount(0);
        el.UnregisterCallback<PointerDownEvent>(BlockReportPickup, TrickleDown.NoTrickleDown);

        _sellButton.SetEnabled(false);
    }

    void Sell()
    {
        Item soldItem = _sellSlot.ItemElement.Item;
        _gameManager.ChangeGoldValue(soldItem.GetSellValue());

        DismissReport();
    }
}
