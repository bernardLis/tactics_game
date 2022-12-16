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
    VisualElement _shadow;

    public bool IsLocked;

    public event Action<CharacterCardMini> OnLocked;
    public event Action<CharacterCardMini> OnUnlocked;

    const string _ussClassName = "character-card-mini";
    const string _ussMain = _ussClassName + "__main";
    const string _ussOverlay = _ussClassName + "__overlay";
    const string _ussShadow = _ussClassName + "__shadow";
    const string _ussPickedUp = _ussClassName + "__picked-up";


    const string ussCommonTextSecondary = "common__text-secondary";

    public CharacterCardMini(Character character)
    {
        _gameManager = GameManager.Instance;
        var ss = GameManager.Instance.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.CharacterCardMiniStyles);
        if (ss != null)
            styleSheets.Add(ss);
        var common = GameManager.Instance.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.CommonStyles);
        if (ss != null)
            styleSheets.Add(ss);

        _gameManager.OnDayPassed += OnDayPassed;
        Character = character;

        AddToClassList(_ussMain);

        _shadow = new();
        _shadow.AddToClassList(_ussShadow);
        _shadow.style.display = DisplayStyle.None;
        Add(_shadow);


        Add(new CharacterPortraitElement(character));
        AddUnavailableOverlay();
        UpdateUnavailableOverlay();
    }

    void OnDayPassed(int day)
    {
        UpdateUnavailableOverlay();
        if (IsLocked)
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
        l.AddToClassList(ussCommonTextSecondary);
        _overlay.Add(l);
    }

}
