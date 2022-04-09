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
    }

    void BaseModifierVisual()
    {
        AddToClassList("modifierIcon");
    }

    protected override void DisplayTooltip()
    {
        string text = "";

        if (Modifier != null)
            text = Modifier.name.Replace("(Clone)", "") + " for " + Modifier.NumberOfTurns + " turns";

        if (Status != null)
            text = Status.name.Replace("(Clone)", "") + " for " + Status.NumberOfTurns + " turns";

        _tooltip = new(this, text);
        base.DisplayTooltip();
    }

}
