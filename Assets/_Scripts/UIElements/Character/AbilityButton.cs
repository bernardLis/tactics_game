using UnityEngine.UIElements;
using UnityEngine;

public class AbilityButton : Button
{
    VisualElement _icon;

    public string Key;
    public Ability Ability;

    public AbilityButton(Ability ability, string key, VisualElement root)
    {
        Ability = ability;
        Key = key;

        style.backgroundColor = ability.HighlightColor;
        AddToClassList("abilityButton");
        AddToClassList("textPrimary");

        _icon = new();
        _icon.AddToClassList("abilityButtonIcon");
        _icon.style.backgroundImage = ability.Icon.texture;
        Add(_icon);

        TextWithTooltip keyTooltip = new(key, "Hotkey");
        keyTooltip.AddToClassList("abilityButtonStat");

        TextWithTooltip baseDamage = new TextWithTooltip("" + Ability.BasePower, "Base damage");
        baseDamage.AddToClassList("abilityButtonStat");

        TextWithTooltip manaCost = new TextWithTooltip("" + Ability.ManaCost, "Mana cost");
        manaCost.AddToClassList("abilityButtonStat");

        VisualElement container = new();
        container.style.flexDirection = FlexDirection.Row;
        Add(container);

        container.Add(keyTooltip);
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
