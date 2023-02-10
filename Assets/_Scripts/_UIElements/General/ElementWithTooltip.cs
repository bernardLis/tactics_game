using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;
using System.Threading.Tasks;
using DG.Tweening;

public abstract class ElementWithTooltip : VisualElement
{
    protected VisualElement _tooltipContainer;
    protected TooltipElement _tooltip;
    TooltipElement _setTooltipElement;

    bool _blockTooltip;
    bool _isPointerOn;
    bool _isPointerDown;
    bool _isTooltipDisplayed;

    string tooltipTweenId = "tooltipTweenId";

    IVisualElementScheduledItem _tooltipDisplayScheduler;

    public ElementWithTooltip() { RegisterTooltipCallbacks(); }

    protected void RegisterTooltipCallbacks()
    {
        RegisterCallback<PointerDownEvent>((evt) => OnPointerDown());
        RegisterCallback<PointerUpEvent>((evt) => OnPointerUp());

        RegisterCallback<MouseEnterEvent>((evt) => DisplayTooltip());
        RegisterCallback<MouseLeaveEvent>((evt) => OnMouseLeave());

        // on destroy https://forum.unity.com/threads/callback-for-destroy-dispose.856948/
        //  RegisterCallback<DetachFromPanelEvent>((evt) => HideTooltip()); maybe it is unnecessary? :)
    }

    public void AddTooltip(TooltipElement tooltip) { _setTooltipElement = tooltip; }

    public void BlockTooltip()
    {
        _blockTooltip = true;
        HideTooltip();
    }

    void OnPointerDown() { _isPointerDown = true; }
    void OnPointerUp() { _isPointerDown = false; }

    protected virtual void DisplayTooltip()
    {
        // for objects that set tooltip on object creation
        if (_setTooltipElement != null)
            _tooltip = _setTooltipElement;

        _isPointerOn = true;
        if (_tooltipDisplayScheduler != null)
            _tooltipDisplayScheduler.Pause();

        _tooltipDisplayScheduler = schedule.Execute(ShowTooltip).StartingIn(500);
    }

    void ShowTooltip()
    {
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

        _isTooltipDisplayed = true;
        var root = panel.visualTree;

        _tooltipContainer = root.Q<VisualElement>("tooltipContainer");
        if (_tooltipContainer == null)
            return;
        _tooltipContainer.style.display = DisplayStyle.Flex;
        _tooltipContainer.Clear();
        _tooltipContainer.Add(_tooltip);
        _tooltipContainer.BringToFront();

        _tooltip.style.opacity = 0;
        DOTween.Kill(tooltipTweenId);
        DOTween.To(x => _tooltip.style.opacity = x, 0, 1, 0.3f)
                .SetId(tooltipTweenId);
    }

    protected void OnMouseLeave()
    {
        if (_isTooltipDisplayed)
            HideTooltip();
        _isPointerOn = false;
        _isPointerDown = false; // reset otherwise you need to click on it to display tooltip again.
    }

    protected void HideTooltip()
    {
        DOTween.Kill(tooltipTweenId);
        if (_tooltip == null)
            return;

        DOTween.To(x => _tooltip.style.opacity = x, 1, 0, 0.3f)
                .SetId(tooltipTweenId).OnComplete(() =>
                {
                    _tooltipContainer.style.display = DisplayStyle.None;
                    _isTooltipDisplayed = false;
                    _tooltip = null;
                });
    }
}
