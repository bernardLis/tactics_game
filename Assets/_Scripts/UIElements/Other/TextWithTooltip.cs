using UnityEngine.UIElements;

public class TextWithTooltip : VisualWithTooltip
{

    string _tooltipText;

    public TextWithTooltip(string text, string tooltipText) : base()
    {
        _tooltipText = tooltipText;
        Label txt = new Label(text);
        Add(txt);
    }

    protected override void DisplayTooltip()
    {
        _tooltip = new(this, _tooltipText);
        base.DisplayTooltip();
    }

}
