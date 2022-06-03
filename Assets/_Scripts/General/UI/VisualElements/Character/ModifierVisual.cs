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
        string text = "";

        if (Modifier != null)
            text = Modifier.GetDescription();

        if (Status != null)
            text = Status.GetDescription();

        _tooltip = new(this, text);
        base.DisplayTooltip();
    }

    public void RemoveSelf(VisualElement parent)
    {
        Debug.Log($"remove self status name: {Status.name}");
        this.SetEnabled(false);
        parent.Remove(this);
    }

}
