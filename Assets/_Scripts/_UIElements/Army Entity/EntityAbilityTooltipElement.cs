using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class EntityAbilityTooltipElement : VisualElement
{
    const string _ussClassName = "entity-ability-tooltip-element__";
    const string _ussMain = _ussClassName + "main";

    GameManager _gameManager;

    EntityAbility _ability;

    public EntityAbilityTooltipElement(EntityAbility ability)
    {
        _gameManager = GameManager.Instance;
        var ss = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.EntityAbilityTooltipStyles);
        if (ss != null)
            styleSheets.Add(ss);

        _ability = ability;

        AddToClassList(_ussMain);

        Label name = new($"<b>{_ability.name}</b>");
        Add(name);

        Label description = new(_ability.Description);
        description.style.whiteSpace = WhiteSpace.Normal;
        Add(description);

        Label cooldown = new($"Cooldown: {_ability.Cooldown}");
        Add(cooldown);
    }
}
