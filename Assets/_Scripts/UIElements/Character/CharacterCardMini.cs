using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System;

public class CharacterCardMini : VisualWithTooltip
{
    public Character Character;

    public event Action<CharacterCardMini> OnLocked;

    public CharacterCardMini(Character character)
    {
        Character = character;
        AddToClassList("characterCardMini");

        VisualElement content = new();
        content.style.width = Length.Percent(100);
        content.style.height = Length.Percent(100);
        content.style.backgroundImage = new StyleBackground(character.Portrait.Sprite);
        Add(content);

        if (character.IsUnavailable)
            Lock();
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
        OnLocked?.Invoke(this);
        AddLockOverlay();
    }

    void AddLockOverlay()
    {
        VisualElement el = new();
        el.style.position = Position.Absolute;
        el.style.width = Length.Percent(100);
        el.style.height = Length.Percent(100);
        el.style.backgroundColor = new Color(0, 0, 0, 0.3f);
        Label l = new($"{Character.UnavailabilityDuration}");
        l.AddToClassList("textSecondary");
        el.Add(l);
        Add(el);
    }

}
