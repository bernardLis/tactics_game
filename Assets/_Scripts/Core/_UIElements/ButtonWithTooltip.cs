using System;
using UnityEngine.UIElements;

namespace Lis.Core
{
    public class ButtonWithTooltip : ElementWithTooltip
    {
        readonly string _tooltipText;

        public ButtonWithTooltip(string className, string tooltipText, Action callback)
        {
            _tooltipText = tooltipText;
            MyButton button = new(null, className, callback);
            Add(button);
        }

        protected override void DisplayTooltip()
        {
            _tooltip = new(this, new Label(_tooltipText));
            base.DisplayTooltip();
        }
    }
}