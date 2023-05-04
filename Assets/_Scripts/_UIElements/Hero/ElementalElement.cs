using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ElementalElement : ElementWithTooltip
{
    Element _element;

    Label _icon;

    const string _ussClassName = "elemental-element__";
    const string _ussIcon = _ussClassName + "icon";

    public ElementalElement(Element element)
    {
        var ss = GameManager.Instance.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.ElementalElementStyles);
        if (ss != null)
            styleSheets.Add(ss);

        _element = element;

        _icon = new();
        _icon.AddToClassList(_ussIcon);
        _icon.style.backgroundImage = new StyleBackground(element.Icon);

        Add(_icon);
    }

    public void PlayEffect()
    {
        Debug.Log($"play effect");
        Vector3 pos = this.worldTransform.GetPosition();
        pos.x = pos.x + this.resolvedStyle.width / 2;
        pos.y = Camera.main.pixelHeight - pos.y - this.resolvedStyle.height; // inverted, plus play on bottom of element
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(pos);
        worldPos.z = 0;

        EffectHolder instance = ScriptableObject.Instantiate(_element.VFXEffect);
        instance.PlayEffect(worldPos, _element.VFXEffect.VisualEffectPrefab.transform.localScale);
    }

    protected override void DisplayTooltip()
    {
        Label tooltip = new(_element.Description);
        _tooltip = new(this, tooltip);
        base.DisplayTooltip();
    }


}
