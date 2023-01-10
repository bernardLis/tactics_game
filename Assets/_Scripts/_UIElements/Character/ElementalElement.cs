using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ElementalElement : ElementWithTooltip
{
    Element _element;

    const string _ussClassName = "elemental-element__";
    const string _ussIcon = _ussClassName + "icon";

    public ElementalElement(Element element)
    {
        var ss = GameManager.Instance.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.ElementalElementStyles);
        if (ss != null)
            styleSheets.Add(ss);

        _element = element;

        Label icon = new();
        icon.AddToClassList(_ussIcon);
        icon.style.backgroundImage = new StyleBackground(element.Icon);

        Add(icon);
    }

    protected override void DisplayTooltip()
    {
        HideTooltip();
        Label tooltip = new(_element.Description);
        _tooltip = new(this, tooltip);
        base.DisplayTooltip();
    }

}
