using UnityEngine.UIElements;
using UnityEngine;

public class AbilityButton : ElementWithSound
{
    AbilityIcon _icon;

    public string Key;
    public Ability Ability;

    const string _ussCommonTextPrimary = "common__text-primary";
    const string _ussCommonButtonBasic = "common__button-basic";

    const string _ussClassName = "ability-button__";
    const string _ussMain = _ussClassName + "main";
    const string _ussHighlight = _ussClassName + "highlight";

    const string _ussOverlay = _ussClassName + "overlay";

    OverlayTimerElement _cooldownTimer;

    public bool IsOnCooldown { get; private set; }

    public AbilityButton(Ability ability, string key = null) : base()
    {
        var commonStyles = GameManager.Instance.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.CommonStyles);
        if (commonStyles != null)
            styleSheets.Add(commonStyles);
        var ss = GameManager.Instance.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.AbilityButtonStyles);
        if (ss != null)
            styleSheets.Add(ss);

        Ability = ability;
        Ability.OnCooldownStarted += StartCooldown;
        Key = key;

        AddToClassList(_ussMain);
        AddToClassList(_ussCommonTextPrimary);
        AddToClassList(_ussCommonButtonBasic);

        _icon = new AbilityIcon(ability, key);
        Add(_icon);
    }

    void StartCooldown()
    {
        IsOnCooldown = true;
        SetEnabled(false);
        _cooldownTimer = new OverlayTimerElement(Ability.GetCooldown(), Ability.GetCooldown(), false, "");
        _icon.Add(_cooldownTimer);
        _cooldownTimer.OnTimerFinished += OnCooldownFinished;
    }

    void OnCooldownFinished()
    {
        IsOnCooldown = false;
        SetEnabled(true);
        _cooldownTimer.OnTimerFinished -= OnCooldownFinished; // TODO: is it necessary?
        _icon.Remove(_cooldownTimer);
    }

    public void Highlight()
    {
        AddToClassList(_ussHighlight);
    }
    public void ClearHighlight()
    {
        RemoveFromClassList(_ussHighlight);
    }
}
