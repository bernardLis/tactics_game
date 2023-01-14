using UnityEngine.UIElements;
public class AbilityTooltipElement : ElementWithTooltip
{
    Ability _ability;
    VisualElement _modifierContainer;

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

        Label range = new("Range: " + ability.Range);
        range.AddToClassList(_ussCommonTextSecondary);

        Label aoe = new("AOE: " + ability.GetAOEDescription());
        aoe.AddToClassList(_ussCommonTextSecondary);

        StarRankElement rank = new(ability.StarRank, 0.5f);

        _modifierContainer = new();
        _modifierContainer.AddToClassList("modifierContainer");
        HandleModifiers(ability);

        Add(container);
        Add(rank);
        Add(description);
        Add(baseDamage);
        Add(manaCost);
        Add(range);
        Add(aoe);
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
