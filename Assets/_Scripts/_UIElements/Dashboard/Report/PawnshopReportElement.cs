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


    const string _ussClassName = "pawnshop";
    const string _ussSellButton = _ussClassName + "__sell-button";

    public PawnshopReportElement(VisualElement parent, Report report) : base(parent, report)
    {
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

        _reportContents.Add(_sellSlot);
        _draggableItems.AddSellSlot(_sellSlot);

        _goldElement = new(0);
        _reportContents.Add(_goldElement);

        _sellButton = new("Sell", _ussSellButton, Sell);
        _sellButton.AddToClassList(_ussCommonTextPrimary);
        _sellButton.SetEnabled(false);
        _reportContents.Add(_sellButton);

    }

    void BlockReportPickup(PointerDownEvent e) { e.StopImmediatePropagation(); }

    void OnSellItemAdded(ItemElement el)
    {
        _goldElement.ChangeAmount(el.Item.GetSellValue());
        el.RegisterCallback<PointerDownEvent>(BlockReportPickup, TrickleDown.NoTrickleDown);

        _sellButton.SetEnabled(true);
    }

    void OnSellItemRemoved(ItemElement el)
    {
        _goldElement.ChangeAmount(0);
        el.UnregisterCallback<PointerDownEvent>(BlockReportPickup, TrickleDown.NoTrickleDown);

        _sellButton.SetEnabled(false);
    }

    void Sell()
    {
        Item soldItem = _sellSlot.ItemElement.Item;
        _gameManager.ChangeGoldValue(soldItem.GetSellValue());
        _gameManager.RemoveItemFromPouch(soldItem);

        DismissReport();
    }

}
