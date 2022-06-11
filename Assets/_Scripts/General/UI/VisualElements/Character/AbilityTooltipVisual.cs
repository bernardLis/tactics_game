using UnityEngine.UIElements;
public class AbilityTooltipVisual : VisualWithTooltip
{
    Ability _ability;

    Label _name;
    Label _description;
    Label _range;
    Label _aoe;
    Label _manaCost;
    VisualElement _modifierContainer;

    public AbilityTooltipVisual(Ability ability)
    {
        style.alignSelf = Align.Stretch;

        _ability = ability;

        _name = new(Helpers.ParseScriptableObjectCloneName(ability.name));
        _name.AddToClassList("primaryText");
        _name.style.alignSelf = Align.Center;

        _description = new(ability.Description);
        _description.AddToClassList("secondaryText");
        _description.style.whiteSpace = WhiteSpace.Normal;

        _range = new("Range: " + ability.Range);
        _range.AddToClassList("secondaryText");

        _aoe = new("AOE: " + ability.GetAOEDescription());
        _aoe.AddToClassList("secondaryText");

        _manaCost = new("Mana cost: " + ability.ManaCost.ToString());
        _manaCost.AddToClassList("secondaryText");

        _modifierContainer = new();
        _modifierContainer.AddToClassList("modifierContainer");
        HandleModifiers(ability);

        Add(_name);
        Add(_description);
        Add(_range);
        Add(_aoe);
        Add(_manaCost);
        Add(_modifierContainer);
    }

    void HandleModifiers(Ability ability)
    {
        if (ability.StatModifier != null)
        {
            ModifierVisual mElement = new ModifierVisual(ability.StatModifier);
            _modifierContainer.Add(mElement);
        }

        if (ability.Status != null)
        {
            ModifierVisual mElement = new ModifierVisual(ability.Status);
            _modifierContainer.Add(mElement);
        }
    }

}
