using UnityEngine;
using UnityEngine.UIElements;

namespace Lis.Core
{
    public class TooltipElement : VisualElement
    {
        private const string _ussCommonTextPrimary = "common__text-primary";

        private const string _ussClassName = "tooltip-element";
        private const string _ussMain = _ussClassName + "__main";

        private readonly VisualElement _parentElement;
        private readonly VisualElement _tooltipElement;

        private readonly int offsetX = 20;
        private readonly int offsetY = 30;

        public TooltipElement(VisualElement parent, VisualElement tooltipElement, bool disableTooltipStyle = false)
        {
            StyleSheet commonStyles = GameManager.Instance.GetComponent<AddressableManager>()
                .GetStyleSheetByName(StyleSheetType.CommonStyles);
            if (commonStyles != null)
                styleSheets.Add(commonStyles);
            StyleSheet ss = GameManager.Instance.GetComponent<AddressableManager>()
                .GetStyleSheetByName(StyleSheetType.TooltipElementStyles);
            if (ss != null)
                styleSheets.Add(ss);

            _parentElement = parent;
            _tooltipElement = tooltipElement;
            OnPostVisualCreation();

            style.position = Position.Absolute;

            if (!disableTooltipStyle)
            {
                AddToClassList(_ussCommonTextPrimary);
                AddToClassList(_ussMain);
            }

            Add(tooltipElement);
        }

        public void UpdatePosition(VisualElement element)
        {
            // TODO: inelegant solution, when hero card is tooltip element it does not resolve style for some reason. 
            float elWidth = resolvedStyle.width == 0 ? _tooltipElement.resolvedStyle.width : resolvedStyle.width;
            float elHeight = resolvedStyle.height == 0 ? _tooltipElement.resolvedStyle.height : resolvedStyle.height;
            float x = element.worldBound.xMin;
            float y = element.worldBound.yMin;

            if (x + offsetX + elWidth > Screen.width)
                style.left = x - elWidth + element.resolvedStyle.width;
            else
                style.left = x;

            if (y - elHeight + offsetY < 0)
                style.top = y + element.resolvedStyle.height;
            else
                style.top = y - elHeight;
        }

        //https://forum.unity.com/threads/how-to-get-the-actual-width-and-height-of-an-uielement.820266/
        private void OnPostVisualCreation()
        {
            // Make invisible so you don't see the size re-adjustment
            // (Non-visible objects still go through transforms in the layout engine)
            visible = false;
            schedule.Execute(WaitOneFrame);
        }

        private void WaitOneFrame(TimerState obj)
        {
            // Because waiting once wasn't working
            schedule.Execute(AutoSize);
        }

        private void AutoSize(TimerState obj)
        {
            // Do any measurements, size adjustments you need (NaNs not an issue now)
            MarkDirtyRepaint();
            visible = true;
            UpdatePosition(_parentElement);
        }
    }
}