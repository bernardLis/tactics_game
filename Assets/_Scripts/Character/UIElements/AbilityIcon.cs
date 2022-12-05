using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
public class AbilityIcon : VisualWithTooltip
{
    Ability _ability;

    public AbilityIcon(Ability ability, string key = null) : base()
    {
        _ability = ability;
        AddToClassList("abilityButtonIcon");
        style.backgroundImage = ability.Icon.texture;

        if (key != null)
        {
            TextWithTooltip keyTooltip = new(key, "Hotkey");
            keyTooltip.style.position = Position.Absolute;
            keyTooltip.style.backgroundColor = Color.black;
            keyTooltip.style.left = 0;
            keyTooltip.style.bottom = 0;
            Add(keyTooltip);
        }
    }

    protected override void DisplayTooltip()
    {
        HideTooltip();
        AbilityTooltipVisual tooltip = new(_ability);
        _tooltip = new(this, tooltip);
        base.DisplayTooltip();
    }


}
