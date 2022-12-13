using UnityEngine.UIElements;

public class AbilityNameWithTooltip : ElementWithTooltip
{
    Ability _ability;
    public AbilityNameWithTooltip(Ability ability)
    {
        _ability = ability;
        Add(new Label($"{Helpers.ParseScriptableObjectCloneName(ability.name)}"));
    }

    protected override void DisplayTooltip()
    {
        HideTooltip();
        AbilityTooltipElement tooltip = new(_ability);
        _tooltip = new(this, tooltip);
        base.DisplayTooltip();
    }

}
