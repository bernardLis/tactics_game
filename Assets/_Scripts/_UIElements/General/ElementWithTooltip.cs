using DG.Tweening;
using UnityEngine.UIElements;

namespace Lis
{
    public abstract class ElementWithTooltip : VisualElement
    {
        protected VisualElement _tooltipContainer;
        protected TooltipElement _tooltip;
        TooltipElement _setTooltipElement;

        bool _blockTooltip;
        bool _isPointerOn;
        bool _isPointerDown;
        bool _isTooltipDisplayed;

        readonly string tooltipTweenId = "tooltipTweenId";

        IVisualElementScheduledItem _tooltipDisplayScheduler;

        public ElementWithTooltip() { RegisterTooltipCallbacks(); }

        protected void RegisterTooltipCallbacks()
        {
            RegisterCallback<PointerDownEvent>((evt) => OnPointerDown());
            RegisterCallback<PointerUpEvent>((evt) => OnPointerUp());

            RegisterCallback<MouseEnterEvent>((evt) => DisplayTooltip());
            RegisterCallback<MouseLeaveEvent>((evt) => OnMouseLeave());
        }

        public void SetTooltip(TooltipElement tooltip) { _setTooltipElement = tooltip; }

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
            if (!CanDisplayTooltip()) return;

            _isTooltipDisplayed = true;
            var root = panel.visualTree;

            _tooltipContainer = root.Q<VisualElement>("tooltipContainer");
            if (_tooltipContainer == null) return;

            _tooltipContainer.style.display = DisplayStyle.Flex;
            _tooltipContainer.Clear();
            _tooltipContainer.Add(_tooltip);
            _tooltipContainer.BringToFront();

            _tooltip.style.opacity = 0;
            DOTween.Kill(tooltipTweenId);
            DOTween.To(x => _tooltip.style.opacity = x, 0, 1, 0.3f)
                .SetUpdate(true)
                .SetId(tooltipTweenId);
        }

        bool CanDisplayTooltip()
        {
            if (!_isPointerOn) return false;
            if (_blockTooltip) return false;
            if (_isPointerDown) return false;
            if (panel == null) return false;
            if (panel.visualTree == null) return false;
            if (_tooltip == null) return false;

            return true;
        }

        protected void OnMouseLeave()
        {
            if (_isTooltipDisplayed)
                HideTooltip();
            _isPointerOn = false;
            _isPointerDown = false; // reset otherwise you need to click on it to display tooltip again.
        }

        protected virtual void HideTooltip()
        {
            if (_tooltipContainer == null) return;
            DOTween.Kill(tooltipTweenId);
            if (_tooltip == null) return;


            DOTween.To(x => _tooltip.style.opacity = x, 1, 0, 0.3f)
                .SetUpdate(true)
                .SetId(tooltipTweenId)
                .OnComplete(() =>
                {
                    _tooltipContainer.style.display = DisplayStyle.None;
                    _isTooltipDisplayed = false;
                    _tooltip = null;
                });
        }
    }
}
