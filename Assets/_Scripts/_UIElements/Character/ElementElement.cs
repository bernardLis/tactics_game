using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ElementElement : ElementWithTooltip
{
    Element _element;

    const string _ussClassName = "element-element";
    const string _ussIcon = _ussClassName + "__icon";

    public ElementElement(Element element)
    {
        var ss = GameManager.Instance.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.ElementElementStyles);
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
