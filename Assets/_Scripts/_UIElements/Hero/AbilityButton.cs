using UnityEngine.UIElements;
using UnityEngine;
using System.Collections.Generic;

public class AbilityButton : ElementWithSound
{
    AbilityIcon _icon;

    public Ability Ability;

    const string _ussCommonTextPrimary = "common__text-primary";
    const string _ussCommonButtonBasic = "common__button-basic";

    const string _ussClassName = "ability-button__";
    const string _ussMain = _ussClassName + "main";
    const string _ussHighlight = _ussClassName + "highlight";
    const string _ussLevelDotEmpty = _ussClassName + "level-dot-empty";
    const string _ussLevelDotFull = _ussClassName + "level-dot-full";

    OverlayTimerElement _cooldownTimer;

    public bool IsOnCooldown { get; private set; }

    public AbilityButton(Ability ability, bool showLevel = false) : base()
    {
        var commonStyles = GameManager.Instance.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.CommonStyles);
        if (commonStyles != null)
            styleSheets.Add(commonStyles);
        var ss = GameManager.Instance.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.AbilityButtonStyles);
        if (ss != null)
            styleSheets.Add(ss);

        Ability = ability;
        Ability.OnCooldownStarted += StartCooldown;

        AddToClassList(_ussMain);
        AddToClassList(_ussCommonTextPrimary);
        AddToClassList(_ussCommonButtonBasic);

        _icon = new AbilityIcon(ability);
        Add(_icon);

        if (showLevel) AddLevelUpDots();
    }

    void AddLevelUpDots()
    {
        VisualElement dotContainer = new();
        dotContainer.style.flexDirection = FlexDirection.Row;
        dotContainer.style.position = Position.Absolute;
        dotContainer.style.top = Length.Percent(15);
        Add(dotContainer);
        List<VisualElement> dots = new();
        for (int i = 0; i < Ability.Levels.Count; i++)
        {
            VisualElement dot = new();
            dot.AddToClassList(_ussLevelDotEmpty);
            dots.Add(dot);
            dotContainer.Add(dot);
        }

        for (int i = 0; i < Ability.Level + 1; i++)
            dots[i].AddToClassList(_ussLevelDotFull);

        Ability.OnLevelUp += () =>
        {
            for (int i = 0; i < Ability.Level + 1; i++)
                dots[i].AddToClassList(_ussLevelDotFull);
        };
    }


    void StartCooldown()
    {
        IsOnCooldown = true;

        _icon.style.opacity = 0.5f;
        transform.scale = Vector3.one * 0.8f;

        if (_cooldownTimer != null) RemoveCooldownTimer();

        _cooldownTimer = new OverlayTimerElement(Ability.GetCooldown() - 0.5f, Ability.GetCooldown(), false, "");
        _cooldownTimer.style.width = Length.Percent(90);
        _cooldownTimer.style.height = Length.Percent(90);

        Add(_cooldownTimer);
        _cooldownTimer.OnTimerFinished += OnCooldownFinished;
    }

    void OnCooldownFinished()
    {
        _audioManager.PlayUI("Ability Available");
        IsOnCooldown = false;

        _icon.style.opacity = 1f;
        transform.scale = Vector3.one;

        RemoveCooldownTimer();
    }

    void RemoveCooldownTimer()
    {
        if (_cooldownTimer != null)
        {
            Remove(_cooldownTimer);
            _cooldownTimer.OnTimerFinished -= OnCooldownFinished;
            _cooldownTimer = null;
        }
    }

    public void Highlight() { AddToClassList(_ussHighlight); }

    public void ClearHighlight() { RemoveFromClassList(_ussHighlight); }
}
