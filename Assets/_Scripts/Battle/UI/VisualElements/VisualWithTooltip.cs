using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class VisualWithTooltip : VisualElement
{
    protected TooltipVisual _tooltip;

    protected void RegisterTooltipCallbacks()
    {
        RegisterCallback<MouseEnterEvent>((evt) => DisplayTooltip());
        RegisterCallback<MouseMoveEvent>((evt) => UpdateTooltipPosition());
        RegisterCallback<MouseLeaveEvent>((evt) => HideTooltip());
    }

    protected virtual void DisplayTooltip()
    {
        var root = panel.visualTree;
        root.Add(_tooltip);
    }
    protected void UpdateTooltipPosition()
    {
        if (_tooltip == null)
            return;
        _tooltip.UpdatePosition(this);
    }

    protected void HideTooltip()
    {
        if (_tooltip == null)
            return;
        var root = panel.visualTree;
        root.Remove(_tooltip);
    }

}
