using UnityEngine;
using UnityEngine.UIElements;
using System.Threading.Tasks;

public abstract class ElementWithTooltip : VisualElement
{
    protected VisualElement _tooltipContainer;
    protected TooltipElement _tooltip;

    bool _blockTooltip;
    bool _isPointerOn;
    bool _isPointerDown;

    public ElementWithTooltip() { RegisterTooltipCallbacks(); }

    protected void RegisterTooltipCallbacks()
    {
        RegisterCallback<PointerDownEvent>((evt) => OnPointerDown());
        RegisterCallback<PointerUpEvent>((evt) => OnPointerUp());

        RegisterCallback<MouseEnterEvent>((evt) => DisplayTooltip());
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
        await Task.Delay(500); // tooltip delay
        if (!_isPointerOn)
            return;
        if (_blockTooltip)
            return;
        if (_isPointerDown)
            return;
        if (panel == null)
            return;
        if (panel.visualTree == null)
            return;

        var root = panel.visualTree;
        _tooltipContainer = root.Q<VisualElement>("tooltipContainer");
        _tooltipContainer.Add(_tooltip);
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
        if (_tooltipContainer == null)
            return;

        _tooltipContainer.Clear();
        _tooltip = null;
    }

}
