using UnityEngine.UIElements;

public class TextWithTooltip : VisualWithTooltip
{

    string _tooltipText;
    Label _text;

    public TextWithTooltip(string text, string tooltipText) : base()
    {
        _tooltipText = tooltipText;
        _text = new Label(text);
        Add(_text);
    }

    public void UpdateText(string newText)
    {
        _text.text = newText;
    }

    protected override void DisplayTooltip()
    {
        Label t = new(_tooltipText);
        t.style.whiteSpace = WhiteSpace.Normal;
        _tooltip = new(this, t);
        base.DisplayTooltip();
    }

}
