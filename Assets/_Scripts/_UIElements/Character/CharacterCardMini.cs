using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System;

public class CharacterCardMini : ElementWithTooltip
{
    GameManager _gameManager;
    public Character Character;

    CharacterPortraitElement _portrait;
    VisualElement _overlay;
    VisualElement _shadow;

    TimerElement _unavailabilityTimer;

    public bool IsLocked;

    public event Action<CharacterCardMini> OnLocked;
    public event Action<CharacterCardMini> OnUnlocked;

    const string _ussCommonTextSecondary = "common__text-secondary";
    const string _ussCommonTransitionBasic = "common__transition-basic";

    const string _ussClassName = "character-card-mini__";
    const string _ussMain = _ussClassName + "main";
    const string _ussOverlay = _ussClassName + "overlay";
    const string _ussShadow = _ussClassName + "shadow";
    const string _ussPickedUp = _ussClassName + "picked-up";
    const string _ussSlotted = _ussClassName + "slotted";

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
        character.OnSetUnavailable += SetUnavailable;
        Debug.Log($"Character: {Character.CharacterName} unavailable: {Character.IsUnavailable}");
        AddUnavailableOverlay();

        if (character.IsUnavailable)
            LoadUnavailability();

        AddToClassList(_ussMain);

        _shadow = new();
        _shadow.AddToClassList(_ussShadow);
        _shadow.style.display = DisplayStyle.None;
        Add(_shadow);

        _portrait = new CharacterPortraitElement(character);
        Add(_portrait);

        UpdateUnavailableOverlay();
    }

    void SetUnavailable()
    {
        Lock();
        _unavailabilityTimer = new(Character.UnavailabilityDuration, Character.UnavailabilityDuration, false, "Sprained ankle");
        _unavailabilityTimer.OnTimerFinished += UnavailabilityTimerFinished;
        _overlay.Add(_unavailabilityTimer);
    }

    void LoadUnavailability()
    {
        Lock();
        int timeLeft = Character.UnavailabilityDuration - (int)_gameManager.GetCurrentTimeInSeconds() - (int)Character.DateTimeUnavailabilityStarted.GetTimeInSeconds();
        Debug.Log($"time left: {timeLeft}");

        if (timeLeft <= 0)
        {
            UnavailabilityTimerFinished();
            return;
        }

        _unavailabilityTimer = new(timeLeft, Character.UnavailabilityDuration, false, "Sprained ankle");
        _unavailabilityTimer.OnTimerFinished += UnavailabilityTimerFinished;
        _overlay.Add(_unavailabilityTimer);
    }


    void UnavailabilityTimerFinished()
    {
        Character.SetAvailable();
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
        UpdateUnavailableOverlay();
    }

    public void Unlock()
    {
        IsLocked = false;
        OnUnlocked?.Invoke(this);
        UpdateUnavailableOverlay();
    }

    void AddUnavailableOverlay()
    {
        _overlay = new();
        _overlay.AddToClassList(_ussOverlay);
        Add(_overlay);

        UpdateUnavailableOverlay();
    }

    void UpdateUnavailableOverlay()
    {
        _overlay.style.display = DisplayStyle.None;

        if (!Character.IsUnavailable)
            return;

        _overlay.style.display = DisplayStyle.Flex;
        _overlay.Clear();
        Label l = new($"{Character.UnavailabilityDuration}");
        l.AddToClassList(_ussCommonTextSecondary);
        _overlay.Add(l);
    }

    public void Slotted() { _portrait.Slotted(); }

    public void Unslotted() { _portrait.Unslotted(); }
}
