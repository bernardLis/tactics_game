using UnityEngine.UIElements;
using UnityEngine;

public class ModifierVisual : VisualWithTooltip
{
    public StatModifier Modifier;
    public Status Status;

    public ModifierVisual(StatModifier modifier) : base()
    {
        BaseModifierVisual();
        Modifier = modifier;
        style.backgroundImage = modifier.Icon.texture;
    }

    public ModifierVisual(Status status) : base()
    {
        BaseModifierVisual();
        Status = status;
        style.backgroundImage = status.Icon.texture;
        style.backgroundColor = status.DisplayColor;
    }

    void BaseModifierVisual()
    {
        AddToClassList("modifierIcon");
    }

    protected override void DisplayTooltip()
    {
        VisualElement container = new();
        Label statusName = new();
        statusName.AddToClassList("textPrimary");
        Label description = new();
        description.AddToClassList("textSecondary");
        container.Add(statusName);
        container.Add(description);

        if (Modifier != null)
        {
            statusName.text = Modifier.name;
            description.text = Modifier.GetDescription();
        }

        if (Status != null)
        {
            statusName.text = Status.name;
            description.text = Status.GetDescription();
        }

        _tooltip = new(this, container);
        base.DisplayTooltip();
    }

    public void RemoveSelf(VisualElement parent)
    {
        this.SetEnabled(false);
        parent.Remove(this);
    }

}
