using UnityEngine;
using UnityEngine.UIElements;
using System.Threading.Tasks;
using DG.Tweening;

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

    public void AddTooltip(TooltipElement tooltip) { _tooltip = tooltip; }

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
        if (_tooltip == null)
            return;

        var root = panel.visualTree;
        _tooltipContainer = root.Q<VisualElement>("tooltipContainer");
        _tooltipContainer.Add(_tooltip);

        _tooltip.style.opacity = 0;
        await DOTween.To(x => _tooltip.style.opacity = x, 0, 1, 0.3f).AsyncWaitForCompletion();
    }

    protected void OnMouseLeave()
    {
        HideTooltip();
        _isPointerOn = false;
        _isPointerDown = false; // reset otherwise you need to click on it to display tooltip again.
    }

    protected async void HideTooltip()
    {
        if (_tooltip == null)
            return;
        if (_tooltipContainer == null)
            return;

        await DOTween.To(x => _tooltip.style.opacity = x, 1, 0, 0.3f).AsyncWaitForCompletion();

        _tooltipContainer.Clear();
        _tooltip = null;
    }

}
