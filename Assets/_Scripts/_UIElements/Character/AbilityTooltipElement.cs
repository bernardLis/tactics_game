using UnityEngine.UIElements;
public class AbilityTooltipElement : ElementWithTooltip
{
    Ability _ability;

    const string _ussCommonTextPrimary = "common__text-primary";
    const string _ussCommonTextSecondary = "common__text-secondary";

    const string _ussClassName = "ability-tooltip-element__";
    const string _ussMain = _ussClassName + "main";

    public AbilityTooltipElement(Ability ability)
    {
        var commonStyles = GameManager.Instance.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.CommonStyles);
        if (commonStyles != null)
            styleSheets.Add(commonStyles);
        var ss = GameManager.Instance.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.AbilityTooltipElementStyles);
        if (ss != null)
            styleSheets.Add(ss);

        _ability = ability;
        AddToClassList(_ussMain);

        VisualElement container = new();
        container.style.flexDirection = FlexDirection.Row;

        Label name = new(Helpers.ParseScriptableObjectCloneName(ability.name));
        name.AddToClassList(_ussCommonTextPrimary);
        name.style.alignSelf = Align.Center;
        container.Add(new ElementalElement(ability.Element));
        container.Add(name);

        Label description = new(ability.Description);
        description.AddToClassList(_ussCommonTextSecondary);
        description.style.whiteSpace = WhiteSpace.Normal;

        Label baseDamage = new("Base power: " + ability.BasePower);
        baseDamage.AddToClassList(_ussCommonTextSecondary);

        Label manaCost = new("Mana cost: " + ability.ManaCost.ToString());
        manaCost.AddToClassList(_ussCommonTextSecondary);

        StarRankElement rank = new(ability.StarRank, 0.5f);

        Add(container);
        Add(rank);
        Add(description);
        Add(baseDamage);
        Add(manaCost);
    }
}
