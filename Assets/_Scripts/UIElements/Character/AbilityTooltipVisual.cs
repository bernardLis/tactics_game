using UnityEngine.UIElements;
public class AbilityTooltipVisual : VisualWithTooltip
{
    Ability _ability;

    Label _name;
    Label _description;
    Label _baseDamage;
    Label _manaCost;
    Label _range;
    Label _aoe;
    VisualElement _modifierContainer;

    public AbilityTooltipVisual(Ability ability)
    {
        style.alignSelf = Align.Stretch;

        _ability = ability;

        _name = new(Helpers.ParseScriptableObjectCloneName(ability.name));
        _name.AddToClassList("textPrimary");
        _name.style.alignSelf = Align.Center;

        _description = new(ability.Description);
        _description.AddToClassList("textSecondary");
        _description.style.whiteSpace = WhiteSpace.Normal;

        _baseDamage = new("Base damage: " + ability.BasePower);
        _baseDamage.AddToClassList("textSecondary");

        _manaCost = new("Mana cost: " + ability.ManaCost.ToString());
        _manaCost.AddToClassList("textSecondary");

        _range = new("Range: " + ability.Range);
        _range.AddToClassList("textSecondary");

        _aoe = new("AOE: " + ability.GetAOEDescription());
        _aoe.AddToClassList("textSecondary");

        _modifierContainer = new();
        _modifierContainer.AddToClassList("modifierContainer");
        HandleModifiers(ability);

        Add(_name);
        Add(_description);
        Add(_baseDamage);
        Add(_manaCost);
        Add(_range);
        Add(_aoe);
        Add(_modifierContainer);
        Add(new Label());
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
