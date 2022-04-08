using UnityEngine;

public class ModifierVisual : VisualWithTooltip
{
    public StatModifier Modifier;
    public Status Status;

    public ModifierVisual(StatModifier modifier)
    {
        BaseModifierVisual();
        Modifier = modifier;
        style.backgroundImage = modifier.Icon.texture;
    }

    public ModifierVisual(Status status)
    {
        BaseModifierVisual();
        Status = status;
        style.backgroundImage = status.Icon.texture;
    }

    void BaseModifierVisual()
    {
        AddToClassList("modifierIcon");
        RegisterTooltipCallbacks();
    }

    protected override void DisplayTooltip()
    {
        Debug.Log($"Modifier: {Modifier}");
        Debug.Log($"Status: {Status}");
        string text = "";

        if (Modifier != null)
            text = Modifier.name.Replace("(Clone)", "") + " for " + Modifier.NumberOfTurns + " turns";

        if (Status != null)
            text = Status.name.Replace("(Clone)", "") + " for " + Status.NumberOfTurns + " turns";

        _tooltip = new(this, text);
        base.DisplayTooltip();
    }

}
