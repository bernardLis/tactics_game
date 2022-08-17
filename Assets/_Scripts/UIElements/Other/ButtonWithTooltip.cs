using UnityEngine.UIElements;
using System;

public class ButtonWithTooltip : VisualWithTooltip
{
    string _tooltipText;

    public ButtonWithTooltip(string className, string tooltipText, Action callback) : base()
    {
        _tooltipText = tooltipText;
        Button button = new Button();
        button.AddToClassList(className);
        button.clicked += callback;
        Add(button);
    }

    protected override void DisplayTooltip()
    {
        _tooltip = new(this, new Label(_tooltipText));
        base.DisplayTooltip();
    }

}
