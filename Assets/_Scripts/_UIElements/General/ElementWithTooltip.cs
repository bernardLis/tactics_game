using UnityEngine;
using UnityEngine.UIElements;
using System.Threading.Tasks;

public abstract class ElementWithTooltip : VisualElement
{
    protected TooltipElement _tooltip;

    bool _blockTooltip;
    bool _isPointerOn;
    bool _isPointerDown;

    public ElementWithTooltip()
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

    public void BlockTooltip()
    {
        _blockTooltip = true;
        HideTooltip();
    }

    void OnPointerDown() { _isPointerDown = true; }
    void OnPointerUp() { _isPointerDown = false; }

    protected async virtual void DisplayTooltip()
    {
        _isPointerOn = true;

        if (_blockTooltip)
            return;
        if (_isPointerDown)
            return;
        if (panel == null)
            return;

        await Task.Delay(300); // tooltip delay
        if (!_isPointerOn)
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
        _isPointerOn = false;
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
