using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System;

public class CharacterCardMini : ElementWithTooltip
{
    GameManager _gameManager;
    public Character Character;

    VisualElement _overlay;

    public bool IsLocked;

    public event Action<CharacterCardMini> OnLocked;
    public event Action<CharacterCardMini> OnUnlocked;

    const string ussClassName = "character-card-mini";
    const string ussMain = ussClassName + "__main";
    const string ussOverlay = ussClassName + "__overlay";

    const string ussCommonTextSecondary = "common__text-secondary";

    public CharacterCardMini(Character character)
    {
        _gameManager = GameManager.Instance;
        var ss = GameManager.Instance.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.CharacterPortraitStyles);
        if (ss != null)
            styleSheets.Add(ss);
        var common = GameManager.Instance.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.CommonStyles);
        if (ss != null)
            styleSheets.Add(ss);

        _gameManager.OnDayPassed += OnDayPassed;
        Character = character;

        AddToClassList(ussMain);

        Add(new CharacterPortraitElement(character));
        AddUnavailableOverlay();
        UpdateUnavailableOverlay();
        /*
                if (character.IsUnavailable)
                    Lock();
          */
    }

    void OnDayPassed(int day)
    {
        UpdateUnavailableOverlay();
        if (IsLocked)
            Unlock();
    }

    protected override void DisplayTooltip()
    {
        HideTooltip();
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
        _overlay.AddToClassList(ussOverlay);
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
        l.AddToClassList(ussCommonTextSecondary);
        _overlay.Add(l);
    }

}
