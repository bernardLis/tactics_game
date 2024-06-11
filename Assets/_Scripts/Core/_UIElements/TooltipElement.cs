using UnityEngine;
using UnityEngine.UIElements;

namespace Lis.Core
{
    public class TooltipElement : VisualElement
    {
        const string _ussCommonTextPrimary = "common__text-primary";

        const string _ussClassName = "tooltip-element";
        const string _ussMain = _ussClassName + "__main";

        readonly VisualElement _parentElement;
        readonly VisualElement _tooltipElement;

        readonly int offsetX = 20;
        readonly int offsetY = 30;

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
        void OnPostVisualCreation()
        {
            // Make invisible so you don't see the size re-adjustment
            // (Non-visible objects still go through transforms in the layout engine)
            visible = false;
            schedule.Execute(WaitOneFrame);
        }

        void WaitOneFrame(TimerState obj)
        {
            // Because waiting once wasn't working
            schedule.Execute(AutoSize);
        }

        void AutoSize(TimerState obj)
        {
            // Do any measurements, size adjustments you need (NaNs not an issue now)
            MarkDirtyRepaint();
            visible = true;
            UpdatePosition(_parentElement);
        }
    }
}