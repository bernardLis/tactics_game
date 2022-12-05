using UnityEngine;
using UnityEngine.UIElements;

public class StatVisual : VisualWithTooltip
{
    public Label Icon;
    public Label Value;

    Stat _stat;

    string _tooltipText;

    // when there are no Stats => stats won't be interacted with
    public StatVisual(Sprite icon, int value, string tooltipText) : base()
    {
        BaseStatVisual(icon);
        _tooltipText = tooltipText;
        Value.text = value.ToString();
    }

    // when there are Stats
    public StatVisual(Sprite icon, Stat stat) : base()
    {
        BaseStatVisual(icon);

        _stat = stat;
        _stat.OnModifierAdded += OnModiferAdded;
        _stat.OnModifierRemoved += OnModifierRemoved;

        RegisterCallback<DetachFromPanelEvent>(OnPanelDetached);

        _tooltipText = _stat.Type.ToString();

        HandleStatValue();
    }

    void OnPanelDetached(DetachFromPanelEvent evt)
    {
        _stat.OnModifierAdded -= OnModiferAdded;
        _stat.OnModifierRemoved -= OnModifierRemoved;
    }

    void BaseStatVisual(Sprite icon)
    {
        style.flexDirection = FlexDirection.Row;

        Icon = new();
        Icon.AddToClassList("statIcon");
        Icon.style.backgroundImage = new StyleBackground(icon);
        Add(Icon);

        Value = new();
        Value.AddToClassList("statValue");
        Value.AddToClassList("textPrimary");
        Add(Value);
    }

    void OnModiferAdded(StatModifier modifier)
    {
        HandleStatValue();
    }

    void OnModifierRemoved(StatModifier modifier)
    {
        HandleStatValue();
    }

    void HandleStatValue()
    {
        Value.text = _stat.GetValue().ToString();

        Value.style.color = Color.white;
        if (_stat.GetValue() > _stat.BaseValue)
            Value.style.color = Color.green;
        if (_stat.GetValue() < _stat.BaseValue)
            Value.style.color = Color.red;
    }

    protected override void DisplayTooltip()
    {
        _tooltip = new(this, new Label(_tooltipText));
        base.DisplayTooltip();
    }

    public void OnValueChanged(int newValue)
    {
        Value.text = "" + newValue;
    }

}
