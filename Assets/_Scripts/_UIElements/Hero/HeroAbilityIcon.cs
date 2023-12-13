using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
public class HeroAbilityIcon : ElementWithTooltip
{
    Ability _ability;

    const string _ussClassName = "hero-ability-icon__";
    const string _ussMain = _ussClassName + "main";

    public HeroAbilityIcon(Ability ability) : base()
    {
        var ss = GameManager.Instance.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.HeroAbilityIconStyles);
        if (ss != null)
            styleSheets.Add(ss);

        _ability = ability;
        AddToClassList(_ussMain);
        style.backgroundImage = ability.Icon.texture;
    }

    protected override void DisplayTooltip()
    {
        HeroAbilityTooltipElement tooltip = new(_ability);
        _tooltip = new(this, tooltip);
        base.DisplayTooltip();
    }
}
