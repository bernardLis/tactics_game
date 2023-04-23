using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System;

public class HeroCardMini : ElementWithTooltip
{
    const string _ussCommonTextSecondary = "common__text-secondary";
    const string _ussCommonTransitionBasic = "common__transition-basic";

    const string _ussClassName = "hero-card-mini__";
    const string _ussMain = _ussClassName + "main";
    const string _ussShadow = _ussClassName + "shadow";

    const string _ussTimerMain = _ussClassName + "timer-element-overlay-main";
    const string _ussTimerOverlayMask = _ussClassName + "timer-element-overlay-mask";
    const string _ussTimerLabelWrapper = _ussClassName + "timer-element-label-wrapper";

    GameManager _gameManager;
    public Hero Hero;

    HeroPortraitElement _portrait;
    VisualElement _shadow;

    Injury _injury;
    OverlayTimerElement _unavailabilityTimer;

    public bool IsLocked;

    public event Action<HeroCardMini> OnLocked;
    public event Action<HeroCardMini> OnUnlocked;
    public HeroCardMini(Hero hero)
    {
        _gameManager = GameManager.Instance;
        var common = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.CommonStyles);
        if (common != null)
            styleSheets.Add(common);
        var ss = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.HeroCardMiniStyles);
        if (ss != null)
            styleSheets.Add(ss);

        Hero = hero;
        hero.OnInjuryAdded += SetUnavailable;

        AddToClassList(_ussMain);

        _shadow = new();
        _shadow.AddToClassList(_ussShadow);
        _shadow.style.display = DisplayStyle.None;
        Add(_shadow);

        _portrait = new HeroPortraitElement(hero);
        Add(_portrait);

        if (hero.IsUnavailable())
            LoadUnavailability();

        RegisterCallback<PointerUpEvent>(OnPointerUp);
    }

    void OnPointerUp(PointerUpEvent evt)
    {
        if (evt.button != 0) return;
        Debug.Log($"showing hero card, not implemented yet");

        // TODO: show full hero card but I don't have the element done... 
    }

    void SetUnavailable(Injury injury)
    {
        Lock();
        _injury = injury;

        string injuryName = Helpers.ParseScriptableObjectCloneName(injury.name);
        _unavailabilityTimer = new(injury.GetTotalInjuryTimeInSeconds(), injury.GetTotalInjuryTimeInSeconds(), false, injuryName);
        _unavailabilityTimer.SetStyles(_ussTimerMain, _ussTimerOverlayMask, _ussTimerLabelWrapper);
        _unavailabilityTimer.OnTimerFinished += OnUnavailabilityTimerFinished;
        Add(_unavailabilityTimer);
    }

    void LoadUnavailability()
    {
        Injury injury = Hero.GetActiveInjury();
        if (injury == null)
            return;
        Lock();
        _injury = injury;
        int timeLeft = injury.GetTotalInjuryTimeInSeconds() - ((int)_gameManager.GetCurrentTimeInSeconds() - (int)injury.DateTimeStarted.GetTimeInSeconds());

        if (timeLeft <= 0)
        {
            injury.Healed();
            return;
        }

        string injuryName = Helpers.ParseScriptableObjectCloneName(injury.name);
        _unavailabilityTimer = new(timeLeft, injury.GetTotalInjuryTimeInSeconds(), false, injuryName);
        _unavailabilityTimer.SetStyles(_ussTimerMain, _ussTimerOverlayMask, _ussTimerLabelWrapper);
        _unavailabilityTimer.OnTimerFinished += OnUnavailabilityTimerFinished;
        Add(_unavailabilityTimer);
    }

    void OnUnavailabilityTimerFinished()
    {
        // TODO: I could play an effect even!
        _injury.Healed();
        Remove(_unavailabilityTimer);
        Unlock();
    }

    protected override void DisplayTooltip()
    {
        HeroStatsCard tooltip = new HeroStatsCard(Hero);
        _tooltip = new(this, tooltip, true);
        base.DisplayTooltip();
    }

    public void Lock()
    {
        IsLocked = true;
        OnLocked?.Invoke(this);
    }

    public void Unlock()
    {
        IsLocked = false;
        OnUnlocked?.Invoke(this);
    }
}
