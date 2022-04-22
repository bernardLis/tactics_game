using UnityEngine;
using UnityEngine.UIElements;

public abstract class VisualWithTooltip : VisualElement
{
    protected TooltipVisual _tooltip;

    public VisualWithTooltip()
    {
        RegisterTooltipCallbacks();
    }

    protected void RegisterTooltipCallbacks()
    {
        RegisterCallback<MouseEnterEvent>((evt) => DisplayTooltip());
        RegisterCallback<MouseMoveEvent>((evt) => UpdateTooltipPosition());
        RegisterCallback<MouseLeaveEvent>((evt) => HideTooltip());
        // on destroy https://forum.unity.com/threads/callback-for-destroy-dispose.856948/
        RegisterCallback<DetachFromPanelEvent>((evt) => HideTooltip());
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
        if (_tooltip.parent == null)
            return;
        var root = panel.visualTree;
        root.Remove(_tooltip);
    }

}
