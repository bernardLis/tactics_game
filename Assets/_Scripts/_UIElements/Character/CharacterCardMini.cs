using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System;

public class CharacterCardMini : ElementWithTooltip
{
    const string _ussCommonTextSecondary = "common__text-secondary";
    const string _ussCommonTransitionBasic = "common__transition-basic";

    const string _ussClassName = "character-card-mini__";
    const string _ussMain = _ussClassName + "main";
    const string _ussShadow = _ussClassName + "shadow";
    const string _ussPickedUp = _ussClassName + "picked-up";

    const string _ussTimerMain = _ussClassName + "timer-element-overlay-main";
    const string _ussTimerOverlayMask = _ussClassName + "timer-element-overlay-mask";
    const string _ussTimerLabelWrapper = _ussClassName + "timer-element-label-wrapper";

    GameManager _gameManager;
    public Character Character;

    CharacterPortraitElement _portrait;
    VisualElement _shadow;

    Injury _injury;
    OverlayTimerElement _unavailabilityTimer;

    public bool IsLocked;

    public event Action<CharacterCardMini> OnLocked;
    public event Action<CharacterCardMini> OnUnlocked;
    public CharacterCardMini(Character character)
    {
        _gameManager = GameManager.Instance;
        var common = GameManager.Instance.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.CommonStyles);
        if (common != null)
            styleSheets.Add(common);
        var ss = GameManager.Instance.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.CharacterCardMiniStyles);
        if (ss != null)
            styleSheets.Add(ss);

        Character = character;
        character.OnInjuryAdded += SetUnavailable;
        Debug.Log($"Character: {Character.CharacterName} unavailable: {Character.IsUnavailable()}");

        AddToClassList(_ussMain);

        _shadow = new();
        _shadow.AddToClassList(_ussShadow);
        _shadow.style.display = DisplayStyle.None;
        Add(_shadow);

        _portrait = new CharacterPortraitElement(character);
        Add(_portrait);

        if (character.IsUnavailable())
            LoadUnavailability();
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
        Injury injury = Character.GetActiveInjury();
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

    public void PickedUp()
    {
        _shadow.style.display = DisplayStyle.Flex;
        AddToClassList(_ussPickedUp);
        AudioManager.Instance.PlaySFX("CharacterCardDropped", Vector3.zero);
    }

    public void Dropped()
    {
        _shadow.style.display = DisplayStyle.None;
        RemoveFromClassList(_ussPickedUp);
        AudioManager.Instance.PlaySFX("CharacterCardDropped", Vector3.zero);
    }

    protected override void DisplayTooltip()
    {
        CharacterCard tooltip = new CharacterCard(Character);
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

    public void Slotted() { _portrait.Slotted(); }

    public void Unslotted() { _portrait.Unslotted(); }
}
