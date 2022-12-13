using UnityEngine.UIElements;
public class AbilityTooltipElement : ElementWithTooltip
{
    Ability _ability;

    Label _name;
    Label _description;
    Label _baseDamage;
    Label _manaCost;
    Label _range;
    Label _aoe;
    StarRankElement _rank;
    VisualElement _modifierContainer;

    public AbilityTooltipElement(Ability ability)
    {
        style.alignSelf = Align.Stretch;
        style.alignItems = Align.Center;

        _ability = ability;

        _name = new(Helpers.ParseScriptableObjectCloneName(ability.name));
        _name.AddToClassList("textPrimary");
        _name.style.alignSelf = Align.Center;

        _description = new(ability.Description);
        _description.AddToClassList("textSecondary");
        _description.style.whiteSpace = WhiteSpace.Normal;

        _baseDamage = new("Base power: " + ability.BasePower);
        _baseDamage.AddToClassList("textSecondary");

        _manaCost = new("Mana cost: " + ability.ManaCost.ToString());
        _manaCost.AddToClassList("textSecondary");

        _range = new("Range: " + ability.Range);
        _range.AddToClassList("textSecondary");

        _aoe = new("AOE: " + ability.GetAOEDescription());
        _aoe.AddToClassList("textSecondary");

        _rank = new(ability.Rank, 0.5f);

        _modifierContainer = new();
        _modifierContainer.AddToClassList("modifierContainer");
        HandleModifiers(ability);

        Add(_name);
        Add(_rank);
        Add(_description);
        Add(_baseDamage);
        Add(_manaCost);
        Add(_range);
        Add(_aoe);
        Add(_modifierContainer);
    }

    void HandleModifiers(Ability ability)
    {
        if (ability.StatModifier != null)
        {
            ModifierElement mElement = new ModifierElement(ability.StatModifier);
            _modifierContainer.Add(mElement);
        }

        if (ability.Status != null)
        {
            ModifierElement mElement = new ModifierElement(ability.Status);
            _modifierContainer.Add(mElement);
        }
    }

}
