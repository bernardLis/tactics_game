using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityAbilityElement : ElementWithTooltip
{
    const string _ussClassName = "entity-ability-element__";
    const string _ussMain = _ussClassName + "main";

    GameManager _gameManager;

    EntityAbility _ability;
    public EntityAbilityElement(EntityAbility ability)
    {
        _gameManager = GameManager.Instance;
        var ss = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.EntityAbilityStyles);
        if (ss != null)
            styleSheets.Add(ss);

        _ability = ability;

        AddToClassList(_ussMain);

        style.backgroundImage = _ability.Icon.texture;
    }

    protected override void DisplayTooltip()
    {
        EntityAbilityTooltipElement tooltip = new(_ability);
        _tooltip = new(this, tooltip);
        base.DisplayTooltip();
    }

}
