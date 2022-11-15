using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System;

public class CharacterCardMini : VisualWithTooltip
{
    GameManager _gameManager;
    public Character Character;

    VisualElement _overlay;

    public bool IsLocked;

    public event Action<CharacterCardMini> OnLocked;
    public event Action<CharacterCardMini> OnUnlocked;

    public CharacterCardMini(Character character)
    {
        _gameManager = GameManager.Instance;
        _gameManager.OnDayPassed += OnDayPassed;

        Character = character;
        AddToClassList("characterCardMini");

        VisualElement content = new();
        content.style.width = Length.Percent(100);
        content.style.height = Length.Percent(100);
        content.style.backgroundImage = new StyleBackground(character.Portrait.Sprite);
        Add(content);
        AddUnavailableOverlay();

        if (character.IsUnavailable)
            Lock();
    }

    void OnDayPassed(int day)
    {
        UpdateUnavailableOverlay();
        if (IsLocked && !Character.IsUnavailable)
            Unlock();
    }

    protected override void DisplayTooltip()
    {
        HideTooltip();
        CharacterCardExtended tooltip = new CharacterCardExtended(Character);
        _tooltip = new(this, tooltip);
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
        _overlay.style.position = Position.Absolute;
        _overlay.style.width = Length.Percent(100);
        _overlay.style.height = Length.Percent(100);
        _overlay.style.backgroundColor = new Color(0, 0, 0, 0.3f);
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
        l.AddToClassList("textSecondary");
        _overlay.Add(l);
    }

}
