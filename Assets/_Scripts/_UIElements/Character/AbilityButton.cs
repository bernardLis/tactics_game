using UnityEngine.UIElements;
using UnityEngine;

public class AbilityButton : ElementWithSound
{
    AbilityIcon _icon;

    public string Key;
    public Ability Ability;

    const string _ussCommonTextPrimary = "common__text-primary";

    const string _ussClassName = "ability-button";

    const string _ussMain = _ussClassName + "__main";
    const string _ussStat = _ussClassName + "__stat";
    const string _ussContainer = _ussClassName + "__container";

    public AbilityButton(Ability ability, string key = null) : base()
    {
        var commonStyles = GameManager.Instance.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.CommonStyles);
        if (commonStyles != null)
            styleSheets.Add(commonStyles);
        var ss = GameManager.Instance.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.AbilityButtonStyles);
        if (ss != null)
            styleSheets.Add(ss);

        Ability = ability;
        Key = key;

        style.backgroundColor = ability.HighlightColor;
        AddToClassList(_ussMain);
        AddToClassList(_ussCommonTextPrimary);

        _icon = new AbilityIcon(ability, key);
        Add(_icon);

        TextWithTooltip baseDamage = new TextWithTooltip("" + Ability.BasePower, "Base damage");
        baseDamage.AddToClassList(_ussStat);

        TextWithTooltip manaCost = new TextWithTooltip("" + Ability.ManaCost, "Mana cost");
        manaCost.AddToClassList(_ussStat);

        VisualElement container = new();
        container.AddToClassList(_ussContainer);
        Add(container);

        container.Add(baseDamage);
        container.Add(manaCost);

        if (ability.StatModifier != null)
        {
            ModifierElement modifier = new(ability.StatModifier);
            container.Add(modifier);
        }
        if (ability.Status != null)
        {
            ModifierElement status = new(ability.Status);
            container.Add(status);
        }
    }

}
