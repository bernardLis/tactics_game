using UnityEngine.UIElements;
using UnityEngine;

public class AbilityButton : VisualElementWithSound
{

    AbilityIcon _icon;

    public string Key;
    public Ability Ability;

    public AbilityButton(Ability ability, string key = null) : base()
    {
        Ability = ability;
        Key = key;

        style.backgroundColor = ability.HighlightColor;
        AddToClassList("abilityButton");
        AddToClassList("textPrimary");

        _icon = new AbilityIcon(ability, key);
        Add(_icon);

        TextWithTooltip baseDamage = new TextWithTooltip("" + Ability.BasePower, "Base damage");
        baseDamage.AddToClassList("abilityButtonStat");

        TextWithTooltip manaCost = new TextWithTooltip("" + Ability.ManaCost, "Mana cost");
        manaCost.AddToClassList("abilityButtonStat");

        VisualElement container = new();

        container.style.flexDirection = FlexDirection.Column;
        container.style.justifyContent = Justify.SpaceAround;
        Add(container);

        container.Add(baseDamage);
        container.Add(manaCost);

        if (ability.StatModifier != null)
        {
            ModifierVisual modifier = new(ability.StatModifier);
            container.Add(modifier);
        }
        if (ability.Status != null)
        {
            ModifierVisual status = new(ability.Status);
            container.Add(status);
        }
    }

}
