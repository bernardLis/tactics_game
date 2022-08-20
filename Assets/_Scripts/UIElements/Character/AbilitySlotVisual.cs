using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class AbilitySlotVisual : VisualElement
{
    AudioManager _audioManager;

    public AbilityButton AbilityButton;
    public Character Character;

    public AbilitySlotVisual(AbilityButton abilityButton = null)
    {
        _audioManager = AudioManager.Instance;
        RegisterCallback<MouseEnterEvent>((evt) => PlayClick());

        AddToClassList("abilitySlot");

        if (abilityButton == null)
            return;

        AbilityButton = abilityButton;
        Add(abilityButton);
    }

    public void AddButton(AbilityButton abilityButton)
    {
        AbilityButton = abilityButton;
        Add(abilityButton);
        PlayClick();
    }

    public void RemoveButton()
    {
        Clear();
        AbilityButton = null;
    }

    void PlayClick()
    {
        _audioManager.PlaySFX("uiClick", Vector3.zero);
    }
}
