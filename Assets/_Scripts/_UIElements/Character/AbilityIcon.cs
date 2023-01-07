using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
public class AbilityIcon : ElementWithTooltip
{
    Ability _ability;

    const string _ussClassName = "ability-icon";
    const string _ussMain = _ussClassName + "__main";
    const string _ussHotkey = _ussClassName + "__hotkey";

    public AbilityIcon(Ability ability, string key = null) : base()
    {
        var ss = GameManager.Instance.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.AbilityIconStyles);
        if (ss != null)
            styleSheets.Add(ss);

        _ability = ability;
        AddToClassList(_ussMain);
        style.backgroundImage = ability.Icon.texture;

        if (key != null)
        {
            TextWithTooltip keyTooltip = new(key, "Hotkey");
            keyTooltip.AddToClassList(_ussHotkey);
            Add(keyTooltip);
        }
    }

    protected override void DisplayTooltip()
    {
        HideTooltip();
        AbilityTooltipElement tooltip = new(_ability);
        _tooltip = new(this, tooltip);
        base.DisplayTooltip();
    }


}
