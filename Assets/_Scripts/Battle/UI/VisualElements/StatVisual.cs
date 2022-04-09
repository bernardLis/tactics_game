using UnityEngine;
using UnityEngine.UIElements;

public class StatVisual : VisualWithTooltip
{
    public Label Icon;
    public Label Value;

    Stat _stat;

    string _tooltipText;

    public StatVisual(Sprite icon, int value, string tooltipText) : base()
    {
        BaseStatVisual(icon);
        _tooltipText = tooltipText;
        Value.text = value.ToString();
    }

    public StatVisual(Sprite icon, Stat stat) : base()
    {
        BaseStatVisual(icon);

        _stat = stat;
        _tooltipText = _stat.Type.ToString();

        Value.text = stat.GetValue().ToString();

        Value.style.color = Color.white;
        if (stat.GetValue() > stat.BaseValue)
            Value.style.color = Color.green;
        if (stat.GetValue() < stat.BaseValue)
            Value.style.color = Color.red;
    }

    void BaseStatVisual(Sprite icon)
    {
        style.flexDirection = FlexDirection.Row;

        Icon = new();
        Icon.AddToClassList("statIcon");
        Icon.style.backgroundImage = icon.texture;
        Add(Icon);

        Value = new();
        Value.AddToClassList("statValue");
        Add(Value);
    }

    protected override void DisplayTooltip()
    {
        _tooltip = new(this, _tooltipText);
        base.DisplayTooltip();
    }

}
