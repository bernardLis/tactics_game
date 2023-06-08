using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CreatureAbilityElement : ElementWithTooltip
{
    const string _ussClassName = "creature-ability-element__";
    const string _ussMain = _ussClassName + "main";
    const string _ussTimerLabelWrapper = _ussClassName + "timer-label-wrapper";

    GameManager _gameManager;

    CreatureAbility _ability;

    OverlayTimerElement _timer;
    public CreatureAbilityElement(CreatureAbility ability, float currentCooldown = 0)
    {
        _gameManager = GameManager.Instance;
        var ss = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.CreatureAbilityStyles);
        if (ss != null)
            styleSheets.Add(ss);

        _ability = ability;

        AddToClassList(_ussMain);

        style.backgroundImage = _ability.Icon.texture;
        _ability.OnAbilityUsed += OnAbilityUsed;

        _timer = new(0, 0, false, "");
        _timer.style.display = DisplayStyle.None;
        _timer.OnTimerFinished += () => _timer.style.display = DisplayStyle.None;
        _timer.SetStyles(null, null, _ussTimerLabelWrapper);
        Add(_timer);

        if (currentCooldown > 0)
        {
            _timer.UpdateTimerValues(currentCooldown, _ability.Cooldown);
            _timer.style.display = DisplayStyle.Flex;
        }
    }

    void OnAbilityUsed()
    {
        _timer.UpdateTimerValues(_ability.Cooldown, _ability.Cooldown);
        _timer.style.display = DisplayStyle.Flex;
    }

    protected override void DisplayTooltip()
    {
        CreatureAbilityTooltipElement tooltip = new(_ability);
        _tooltip = new(this, tooltip);
        base.DisplayTooltip();
    }

}
