using UnityEngine.UIElements;

public class AbilityNameWithTooltip : VisualWithTooltip
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
        AbilityTooltipVisual tooltip = new(_ability);
        _tooltip = new(this, tooltip);
        base.DisplayTooltip();
    }

}
