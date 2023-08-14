using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CreatureAbilityElement : ElementWithTooltip
{
    const string _ussCommonTextPrimary = "common__text-primary";

    const string _ussClassName = "creature-ability__";
    const string _ussMain = _ussClassName + "main";
    const string _ussTimerLabelWrapper = _ussClassName + "timer-label-wrapper";
    const string _ussLockedOverlay = _ussClassName + "locked-overlay";

    GameManager _gameManager;

    CreatureAbility _ability;

    bool _isLocked;

    VisualElement _lockedOverlay;
    OverlayTimerElement _timer;
    public CreatureAbilityElement(CreatureAbility ability, float currentCooldown = 0, bool isLocked = false)
    {
        _gameManager = GameManager.Instance;
        var ss = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.CreatureAbilityStyles);
        if (ss != null)
            styleSheets.Add(ss);

        _isLocked = isLocked;
        _ability = ability;

        AddToClassList(_ussMain);

        style.backgroundImage = _ability.Icon.texture;
        _ability.OnAbilityUsed += OnAbilityUsed;

        AddTimer();
        AddLockedOverlay();

        if (currentCooldown > 0)
        {
            _timer.UpdateTimerValues(currentCooldown, _ability.Cooldown);
            _timer.style.display = DisplayStyle.Flex;
        }
    }

    void AddTimer()
    {
        _timer = new(0, 0, false, "");
        _timer.style.display = DisplayStyle.None;
        _timer.OnTimerFinished += () => _timer.style.display = DisplayStyle.None;
        _timer.SetStyles(null, null, _ussTimerLabelWrapper);
        Add(_timer);
    }

    void AddLockedOverlay()
    {
        if (!_isLocked) return;

        _lockedOverlay = new();
        _lockedOverlay.AddToClassList(_ussLockedOverlay);
        Label label = new("Unlocked at level 6."); // TODO: hardcoded value
        label.style.whiteSpace = WhiteSpace.Normal;
        label.AddToClassList(_ussCommonTextPrimary);
        _lockedOverlay.Add(label);

        Add(_lockedOverlay);
    }

    public void Unlock()
    {
        _isLocked = false;
        if (_lockedOverlay != null)
            Remove(_lockedOverlay);
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
