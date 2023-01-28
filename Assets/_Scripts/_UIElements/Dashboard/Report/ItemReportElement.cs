using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using DG.Tweening;

public class ItemReportElement : ReportElement
{
    ItemElement _itemElement;
    public ItemReportElement(VisualElement parent, Report report) : base(parent, report)
    {
        AddHeader("New Item", Color.cyan);

        _itemElement = new ItemElement(report.Item);
        ItemSlot slot = new(_itemElement);
        _reportContents.Add(slot);

        AddAcceptRejectButtons(AcceptItem, RejectItem);
    }

    void AcceptItem()
    {
        if (_gameManager.PlayerAbilityPouch.Count >= 5) // TODO: magic 5
        {
            Helpers.DisplayTextOnElement(_deskManager.Root, this, "No more space in pouch", Color.red);
            DOTween.Shake(() => _itemElement.transform.position, x => _itemElement.transform.position = x, 1f);
            return;
        }

        _deskManager.AddItemToEmptySlot(_itemElement);
        DismissReport();
    }

    void RejectItem()
    {
        DismissReport();
    }
}
