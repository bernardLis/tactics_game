using UnityEngine;
using UnityEngine.UIElements;

public abstract class VisualWithTooltip : VisualElement
{
    protected TooltipVisual _tooltip;

    bool _isPointerDown;

    public VisualWithTooltip()
    {
        RegisterTooltipCallbacks();
    }

    protected void RegisterTooltipCallbacks()
    {
        RegisterCallback<PointerDownEvent>((evt) => OnPointerDown());
        RegisterCallback<PointerUpEvent>((evt) => OnPointerUp());

        RegisterCallback<MouseEnterEvent>((evt) => DisplayTooltip());
        RegisterCallback<MouseMoveEvent>((evt) => UpdateTooltipPosition());
        RegisterCallback<MouseLeaveEvent>((evt) => OnMouseLeave());

        // on destroy https://forum.unity.com/threads/callback-for-destroy-dispose.856948/
        RegisterCallback<DetachFromPanelEvent>((evt) => HideTooltip());
    }

    void OnPointerDown() { _isPointerDown = true; }
    void OnPointerUp() { _isPointerDown = false; }

    protected virtual void DisplayTooltip()
    {
        if (_isPointerDown)
            return;
        if (panel == null)
            return;
        var root = panel.visualTree;
        root.Add(_tooltip);
    }

    protected void UpdateTooltipPosition()
    {
        if (_tooltip == null)
            return;
        _tooltip.UpdatePosition(this);
    }

    protected void OnMouseLeave()
    {
        HideTooltip();
        _isPointerDown = false; // reset otherwise you need to click on it to display tooltip again.
    }

    protected void HideTooltip()
    {
        if (_tooltip == null)
            return;

        var root = panel.visualTree;
        _tooltip.SetEnabled(false);
        if (_tooltip.parent == root)
            root.Remove(_tooltip);
        _tooltip = null;
    }

}
