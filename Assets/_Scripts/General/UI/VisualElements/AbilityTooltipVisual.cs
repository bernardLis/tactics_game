using UnityEngine.UIElements;
public class AbilityTooltipVisual : VisualWithTooltip
{
    Ability _ability;

    Label _name;
    Label _description;
    Label _manaCost;
    VisualElement _modifierContainer;

    public AbilityTooltipVisual(Ability ability)
    {
        _ability = ability;

        _name = new(Helpers.ParseScriptableObjectCloneName(ability.name));
        _name.AddToClassList("abilityName");

        _description = new(ability.Description);
        _description.AddToClassList("abilityDescription");

        _description = new(ability.Description);
        _description.AddToClassList("abilityDescription");

        _manaCost = new("Mana cost: " + ability.ManaCost.ToString());
        _manaCost.AddToClassList("abilityManaCost");

        _modifierContainer = new();
        _modifierContainer.AddToClassList("modifierContainer");
        HandleModifiers(ability);

        Add(_name);
        Add(_description);
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
